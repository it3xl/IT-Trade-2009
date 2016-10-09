using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using ITTrade.IT;
using ITTrade.ITTradeDataSetTableAdapters;
using System.Transactions;
using IT.WPF.ValidationRules;
using ITTradeControlLibrary;
using ITTradeUtils;

namespace ITTrade.Business
{
	public delegate void SetChangedViewDelegate(bool isChanged);


	public partial class ProductEditer
	{
		/// <summary>
		/// Режимы работы контроллера ProductEditer
		/// </summary>
		public enum EditMode
		{
			/// <summary>
			/// неопределенный режим работы контроллера
			/// </summary>
			Undefined,


			/// <summary>
			/// Создается новый еще не созраненный продукт. Важно после сохранения поменять статус.
			/// </summary>
			JustCreatedNotSaved,

			/// <summary>
			/// Редактруется найденный продукт
			/// </summary>
			Existed
		}

		private static ProductEditer current;

		public static ProductEditer Current
		{
			get
			{
				// ReSharper disable ConvertIfStatementToNullCoalescingExpression
				if (current == null)
				// ReSharper restore ConvertIfStatementToNullCoalescingExpression
				{
					current = new ProductEditer();

					// Tут нельзя сразу ставиль режим EditMode.JustCreated, тк WPF дергает события комманд CanExecute до инициализации ProductEditer
					// те ProductEditer.Init(...) вызывается после InitializeComponent() в конструкторе формы.
					// Чтобы избежать проблем, момент выcтавления рабочего режима вызовом метода ProductEditer.Current.ProductCreateNewForEdit() доверим самой форме 
					//__current.initProductEditer(EditMode.JustCreated, null, null);
				}

				return current;
			}
			private set
			{
				current = value;
			}
		}

		/// <summary>
		/// Режим работы контроллера (Новый продукт, найденный, неопределенно)
		/// </summary>
		public EditMode Mode { get; private set; }

		/// <summary>
		/// Объект хранящий все данные по текущему редактируемому продукту
		/// </summary>
		private ITTradeDataSet _dataSet;

		private ManagersProduct _managersProductCurrent;

		/// <summary>
		/// объект данных для продукта, привязываемый в UI
		/// </summary>
		public ManagersProduct ProductCurrent
		{
			get
			{
				//// DataTable поддерживает синхронизацию изменений, а ee строка нет, поэтому строка return dataSet.Products[0] будет нерабочей для 
				//// данных, которые меняются в коде.
				//// Из-за этого нужно учитывать индексацию при биндинге
				// --- Теперь попробую через обертку, класс Product.
				return _managersProductCurrent ?? (_managersProductCurrent = new ManagersProduct(this));
			}
		}

		/// <summary>
		/// Позволяет контроллеру сообщить View, что нужно перебиндить данные
		/// </summary>
		private static Action _bindDataForEditingOnView;

		private static Window _hostWindowForCenteringMessages;

		private ProductEditer()
		{
			// закроем доступ с помощью private
		}

		internal static void Init(
			Window hostWindowForCenteringMessages
			, Action bindDataForEditing
			//, SetChangedViewDelegate setChangedView
			)
		{
			// удалим оставшийся от прошлого окна объект
			Current = null;

			_hostWindowForCenteringMessages = hostWindowForCenteringMessages;
			_bindDataForEditingOnView = bindDataForEditing;
			//view_setChangedView = setChangedView;
		}

		/// <summary>
		/// Монстровый метод, подготавливающий контроллер к редактированию существующего продукта или к созданию нового
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="productId"></param>
		/// <param name="productBarcode"></param>
		private void InitProductEditer(EditMode mode, long? productId, String productBarcode)
		{
			Mode = mode;

			_dataSet = new ITTradeDataSet();
			if (Mode == EditMode.JustCreatedNotSaved)
			{
				// создание нового продукта
				// Специально не защищиаем от исключения, тк если нет возможности беспрепятственно создать новый продукт, то 
				// будет нарушно правило кода, что если недоступен или не найден существующий продукт, то сразу переходим к редактирования нового продукта

				// наполним dataSet новыми пустыми данными


				var newProductRow = _dataSet.Product.NewProductRow();
				newProductRow[_dataSet.Product.CreationDateColumn] = DateTime.Now;
				newProductRow[_dataSet.Product.ModifyDateColumn] = DateTime.Now;
				//productRow.ProductGroupID = getFirstProductGroup
				newProductRow[_dataSet.Product.ProductGroupNameColumn] = String.Empty;
				_dataSet.Product.AddProductRow(newProductRow);

				var newPurchaseRow = _dataSet.Purchase.NewPurchaseRow();
				newPurchaseRow[_dataSet.Purchase.CreationDateColumn] = DateTime.Now;
				newPurchaseRow[_dataSet.Purchase.ModifyDateColumn] = DateTime.Now;
				newPurchaseRow[_dataSet.Purchase.ProductIDColumn] = newProductRow.ProductID;
				_dataSet.Purchase.AddPurchaseRow(newPurchaseRow);
			}
			else if (Mode == EditMode.Existed)
			{
				// инициализация редактирование существующего продукта
				try
				{
					// ищем данные в базе
					var productAdapter = new ProductTableAdapter();
					var purchaseAdapter = new PurchaseTableAdapter();
					if (productId != null)
					{
						// в метод передали ID продукта
						productAdapter.FillByProductID(_dataSet.Product, (long)productId);
						purchaseAdapter.FillByProductID(_dataSet.Purchase, (long)productId);
					}
					else if (productBarcode != null)
					{
						// используется для сканера штрихкода

						// в метод передали barcode
						productAdapter.FillByProductBarcode(_dataSet.Product, productBarcode);
						if (_dataSet.Product.Rows.Count == 1)
						{
							purchaseAdapter.FillByProductID(_dataSet.Purchase,
								//(long)productRow[_dataSet.Product.ProductIDColumn]
								ProductCurrent.ProductId
								);
						}
						else
						{
							throw new Exception("Ошибка! Наличие двух товаров с одинаковым штрихкодом.");
						}
					}
					else
					{
						throw new ArgumentException("Нужно передать штрихкод или Id продукта.");
					}

					// В коде есть проверка, на то чтоб запускалось редактирование только найденных продуктов, но, чтобы исключить проблемы маштабирования
					// и внезапные изменения в базе, выполним эту базовую проверку здесь
					// Также эта проверка необходима, тк оператор может просканировать непринадлежащий базе штрихкод, по которому поиск не вернет результатов
					if (_dataSet.Product.Rows.Count == 0)
					{
						// продукт не найден

						// предупредим оператора
						string mess = "Указанный продукт не найден.";
						if (productId != null)
						{
							mess += "\n\n\tProductId=\"" + productId + "\"";
						}
						// ReSharper disable ConditionIsAlwaysTrueOrFalse
						else if (productBarcode != null)
						// ReSharper restore ConditionIsAlwaysTrueOrFalse
						{
							mess += "\n\n\tBarcode=\"" + productBarcode + "\"";
						}

						MessageWindow.Show(
							new UniversalMessageContext
							{
								Content = mess + "\n\nПовторите поиск.",
								WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
							}
						);

						// были случаи, когда продукт не найден, а код этого не ожидает и падает при обращении к данным продукта, поэтому,
						// чтобы упростить логику, сделаем, что если продукт не найден, то сразу переходим к созданию нового продукта
						CreateNewProductForEditUnsafely();
						goto FAULT;
					}
				}
				catch (Exception ex)
				{
					Logger.Write(ex);
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = "Ошибка при получении данных о продукте!\n\n" + ex.Message,
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
						}
					);

					// чтобы упростить логику, сделаем, что если продукт не найден, то сразу переходим к созданию нового продукта
					CreateNewProductForEditUnsafely();
					goto FAULT;
				}
			}
			else
			{
				// прекратим работу приложения, тк в нем серьезное нарушение логики работы
				throw new Exception("Недопустимое состояние инициализации данных о продукте.");
			}

			// для всех пользователей зададим текущим этот контроллер
			Current = this;

			SetView();

		FAULT: ;
			//IsChanged = false;

		}

		private void SetView()
		{
			// Обнуление _productCurrent приводит к созданию объекта заново на основе текущего состояния эдитора.
			_managersProductCurrent = null;
			// заставим UI привязать новый Current объект
			_bindDataForEditingOnView();
		}

		/// <summary>
		/// Объект данных, пердставляющий поиск по группе символов из какого-либо штрихкода
		/// </summary>
		/// <returns></returns>
		public DataView ProductSearchByBarcode(string productBarcodePart)
		{
			var productDataTable = new ITTradeDataSet.ProductDataTable();
			var productTableAdapter = new ProductTableAdapter();
			productTableAdapter.FillByProductBarcodePart(productDataTable, productBarcodePart);

			return productDataTable.DefaultView;
		}

		public void CreateNewProductForEditSafely()
		{
			//if (IsChanged)
			if (ProductCurrent.IsChanged)
			{
				var dialogContext = new UniversalMessageContext
				{
					Content = "Вы уже изменили текущий продукт.\nЕсли продолжить, то изменения будут потеряны.",
					Button1Context = new UniversalMessageButtonContext
					{
						Text = "Отмена",
					},
					Button2Context = new UniversalMessageButtonContext
					{
						Text = "Редактировать новый продукт",
						ClickHandler = CreateNewProductForEditUnsafely,
					},
					WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
				};

				MessageWindow.Show(dialogContext);
			}
			else
			{
				CreateNewProductForEditUnsafely();
			}
		}

		/// <summary>
		/// прямой запуск содания и редактирования нового продукта, без проверок состояния текущего контроллера
		/// </summary>
		private static void CreateNewProductForEditUnsafely()
		{
			var productEditer = new ProductEditer();
			productEditer.InitProductEditer(EditMode.JustCreatedNotSaved, null, null);
		}

		public void EditProductFromBarcode(String productBarcode)
		{
			productBarcode = productBarcode.ToUpper();
			var dialogContext = new UniversalMessageContext
			{
				Content = "Присвоить штрихкод текущему продукту...\nили\nначать редактирование продукта с таким штрихкодом?\n\n\tШтрихкод \"" + productBarcode + "\"",
				//CancelButtonAction=null,
				Button1Context = new UniversalMessageButtonContext
				{
					Text = "Присвоить текущему продукту",
					ClickHandler = () => { ProductCurrent.ProductBarcode = productBarcode; },
				},
				Button2Context = new UniversalMessageButtonContext
				{
					Text = "Редактировать продукт с таким штрихкодом",
					ClickHandler = () => ProductEditByBarcode(productBarcode),
				},
				WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
			};

			MessageWindow.Show(dialogContext);
		}

		private static void ProductEditByBarcode(String barcode)
		{
			var productEditer = new ProductEditer();
			productEditer.InitProductEditer(EditMode.Existed, null, barcode);
		}

		public void EditProductFromId(long productId)
		{
			if (ProductCurrent.IsChanged)
			{
				var dialogContext = new UniversalMessageContext
				{
					Content = "Вы уже изменили текущий продукт.\nЕсли продолжить, то изменения будут потеряны.",
					CancelButtonContext = null,
					Button1Context = new UniversalMessageButtonContext
					{
						Text = "Отмена",
					},
					Button2Context = new UniversalMessageButtonContext
					{
						Text = "Редактировать найденный продукт",
						ClickHandler = () => ProductEditExisted(productId)
					},
					WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
				};

				MessageWindow.Show(dialogContext);
			}
			else
			{
				ProductEditExisted(productId);
			}
		}

		private static void ProductEditExisted(long productId)
		{
			var productEditer = new ProductEditer();
			productEditer.InitProductEditer(EditMode.Existed, productId, null);
		}

		public void ProductUndoChangesSafely()
		{
			if (ProductCurrent.IsChanged)
			{
				var dialogContext = new UniversalMessageContext
				{
					Content = "Сделанные вами изменения не сохранены и будут потеряны.\n\tПродолжить?",
					CancelButtonContext = null,
					Button1Context = UniversalMessageButtonContext.GetDefaultCancelButtonContext(),
					Button2Context = new UniversalMessageButtonContext
					{
						Text = "Отменить изменения",
						ClickHandler = ProductUndoChangesUnsafely,
					},
					WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
				};

				MessageWindow.Show(dialogContext);
			}
			else
			{
				ProductUndoChangesUnsafely();
			}
		}

		private void ProductUndoChangesUnsafely()
		{
			switch (Mode)
			{
				case EditMode.JustCreatedNotSaved:
					CreateNewProductForEditUnsafely();
					break;
				case EditMode.Existed:
					ProductEditExisted(ProductCurrent.ProductId);
					break;
			}
		}

		public void CheckAndSave()
		{
			if (ProductCurrent.Purchases.Count == 0)
			{
				// подстраховка, тк на этом основывается безcбойная работа программы.
				Logger.Write("Попытка сохранения продукта, у которого нет закупок. ProductID=" + ProductCurrent.ProductId + ", Barcode=" + ProductCurrent.ProductBarcode);
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = "Нельзя сохранять продукт, если нет закупок.",
						WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,

					}
				);

				return;
			}

			new ConfirmationsChain
			{
				DoneAction = SaveProductAndPurchaseAfterValidation,
			}
			.Enqueue(
				confirm =>
				{
					if (ProductCurrent.Purchases.Any(el => ProductCurrent.CurrentWholesalePrice <= el.PurchasePrice))
					{
						MessageWindow.Show(
							new UniversalMessageContext
							{
								Content = "Оптовая цена меньше или равна одной из закупочных цен.\n\nСохранить неправильное значение?",
								WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
								Button1Context=UniversalMessageButtonContext.GetDefaultOkButtonContext(),
								CloseAction = confirm,
							});
						return;
					}
					confirm(true);
				}
			)
			.Enqueue(
				confirm =>
				{
					if (ProductCurrent.CurrentRetailPrice <= ProductCurrent.CurrentWholesalePrice)
					{
						MessageWindow.Show(
							new UniversalMessageContext
							{
								Content = "Розничная цена должна быть больше оптовой цены.\n\nСохранить неправильное значение?",
								WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
								Button1Context=UniversalMessageButtonContext.GetDefaultOkButtonContext(),
								CloseAction = confirm,
							});
						return;
					}
					confirm(true);
				}
			)
			.Start();
		}

		private void SaveProductAndPurchaseAfterValidation()
		{
			using (var updateTransaction = new TransactionScope())
			{
				try
				{
					ProductCurrent.Save();
					// после сохранения уже считаем, что редактируем существующий продукт и закупки.
					Mode = EditMode.Existed;
					updateTransaction.Complete();
					ProductCurrent.SetNoChanged();
				}
				catch (Exception ex)
				{
					Logger.Write(ex);
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = string.Format("Произошла ошибка при сохранении продукта.\n\n{0}", ex.Message),
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages
						}
					);
				}
			}
		}

		internal bool ProductGetCanDelete()
		{
			if (Mode == EditMode.JustCreatedNotSaved)
			{
				// Несохраненный продукт не имеет смысла удалять
				// Ничего не меняем и вернем false

				return false;
			}

			var canDelete = false;
			if (Mode == EditMode.Existed)
			{
				canDelete = ProductCurrent.CanDelete;
			}

			return canDelete;
		}

		internal void ProductDelete()
		{
			// логика данных построена так, что если удалять нельзя, то это на всегда и это состояние сохраняется в данных,
			// а вот признак, что удалить можно может измениться в любой момент и его нужно проверять перед удалением


			if (Mode == EditMode.Existed)
			{
				// это существующий продукт
				var productTableAdapter = new ProductTableAdapter();
				var canDelete = (bool)productTableAdapter.GetIsProductCanDelete(ProductCurrent.ProductId);
				if (canDelete)
				{
					// удалять можно, тк продукт не учавствовала в подажах
					try
					{
						bool? isDone = null;
						string cantDoneMessage = null;
						// метод позволяет за раз удалить и продукт и его закупки
						productTableAdapter.ProductDelete(
							ProductCurrent.ProductId,
							ref isDone,
							ref cantDoneMessage
							);

						CreateNewProductForEditUnsafely();
					}
					catch (Exception ex)
					{
						Logger.Write(ex);
						MessageWindow.Show(
							new UniversalMessageContext
							{
								Content = "Произошла ошибка при удалении продукта.\n\n" + ex.Message,
								WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
							}
						);
					}
				}
				else
				{
					ProductCurrent.SetCanDelete(false);

					// удалять нельзя, тк этот продукт уже учавствовала в продажах
					// Команда WPF сюда не пропустит, но для контроля от внезапного изменения оставлю
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = "Этот продукт удалить нельзя, т.к. он уже учавствовала в продажах.",
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
						}
					);
				}
			}
			else if (Mode == EditMode.JustCreatedNotSaved)
			{
				// это новый несохраненный продукт. Ничего не делаем, тк нет смысла и "команда" WPF сюда не пропустит
			}
		}

		/// <summary>
		/// Создать новую закупочную цену
		/// </summary>
		public void PurchaseCreateNew()
		{
			ITTradeDataSet.PurchaseRow purchaseRow = _dataSet.Purchase.NewPurchaseRow();
			purchaseRow[_dataSet.Purchase.CreationDateColumn] = DateTime.Now;
			purchaseRow[_dataSet.Purchase.ModifyDateColumn] = DateTime.Now;
			purchaseRow[_dataSet.Purchase.ProductIDColumn] = ProductCurrent.ProductId;
			_dataSet.Purchase.AddPurchaseRow(purchaseRow);

			ProductCurrent.Purchases.Add(new ManagersPurchase(_managersProductCurrent, purchaseRow));
		}

		/// <summary>
		/// можно ли удалить закупочную цену
		/// </summary>
		public bool PurchaseGetCanDelete(ManagersPurchase managersPurchase)
		{
			bool canDelete = false;

			if (Mode == EditMode.JustCreatedNotSaved)
			{
				// В случае новой строки ничего не делаем, тк при удалении последней строки, хочу, чтоб показывалось предупреждение,
				// что последнюю закупку нельзя удалять
				canDelete = true;
			}
			else if (Mode == EditMode.Existed)
			{
				canDelete = managersPurchase.CanDelete;
			}

			return canDelete;
		}

		/// <summary>
		/// Удалить закупочную цену
		/// </summary>
		public void DeletePurchaseSafely(ManagersPurchase managersPurchase)
		{
			if (ProductCurrent.Purchases.Count <= 1)
			{
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = "Должна остаться хотябы одна закупка.",
						WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
					}
				);

				return;
			}

			if (Mode == EditMode.Existed)
			{
				var dialogContext = new UniversalMessageContext
				{
					Content = "Удаление закупки нельзя отменить!",
					Button1Context = new UniversalMessageButtonContext
					{
						Text = "Удалить закупку безвозвратно",
						ClickHandler = () => DeletePurchaseUnsafely(managersPurchase)
					},
					WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
				};

				MessageWindow.Show(dialogContext);
			}
			else
			{
				DeletePurchaseUnsafely(managersPurchase);
			}
		}

		/// <summary>
		/// Удалить закупочную цену
		/// </summary>
		private void DeletePurchaseUnsafely(ManagersPurchase managersPurchase)
		{
			// удаляем не виртупльно в наборе данных, а сразу в базе, из-за того, что возможность удаления закупки может измениться в любой момен, если закупка начнет использоваться.

			// разделяем способы удаления по purchaseRow.ЗакупкаID. Если меньше 0, то закупка виртуальна и в базе еще не сохраненена,
			// в другом случае удаляем в базе. Также при удалении в баззе закупки с purchaseRow.ЗакупкаID<0 или не существующей закупки ошибки не возникнет,
			// тк так работает хранимая процедура удаления

			//ITTradeDataSet.PurchaseRow purchaseRow = (ITTradeDataSet.PurchaseRow)dataRow;

			if (managersPurchase.PurchaseId < 0)
			{
				// строка еще была не сохранена в базе
				_dataSet.Purchase.RemovePurchaseRow(managersPurchase.PurchaseRow);
				ProductCurrent.Purchases.Remove(managersPurchase);
			}
			else
			{
				var purchaseTableAdapter = new PurchaseTableAdapter();
				var canDelete = (bool)purchaseTableAdapter.GetIsPurchaseCanDelete(managersPurchase.PurchaseId);
				if (canDelete)
				{
					// удалять можно, тк закупка не учавствовала в продажах
					try
					{
						bool? isDone = null;
						string cantDoneMessage = null;
						// не разобрался, но при удалении через Update возникают какие-то ошибки
						purchaseTableAdapter.PurchaseDelete(
							managersPurchase.PurchaseId
							, ref isDone,
							ref cantDoneMessage
							);
						// только после удачного удаления в БД, физически удалим строку из таблицы
						_dataSet.Purchase.RemovePurchaseRow(managersPurchase.PurchaseRow);

						// также удалим из View
						ProductCurrent.Purchases.Remove(managersPurchase);
					}
					catch (Exception ex)
					{
						Logger.Write(ex);
						MessageWindow.Show(
							new UniversalMessageContext
							{
								Content = "Ошибка при удалении закупки.\n\n" + ex.Message,
								WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
							}
						);
					}
				}
				else
				{
					// запомним состояние локально
					managersPurchase.PurchaseRow.CanDelete = false;

					// удалять нельзя, тк эта закупка уже учавствовала в продажах
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = "Эту закупку удалить нельзя, т.к. она уже учавствовала в продажах.\n\nВы можете вручную перенести количество из этой закупки в другую.",
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
						}
					);
				}
			}
		}

		internal void BarcodeCheckUnique()
		{
			ProductCurrent.ProductBarcode = ProductCurrent.ProductBarcode
				.ToUpper()
				.Trim();

			string barcode = ProductCurrent.ProductBarcode;
			var validationRes = BarcodeValidationRule.GetIsBarcodeValid(barcode);

			if (validationRes.IsValid)
			{
				var productTableAdapter = new ProductTableAdapter();
				var res = productTableAdapter.GetIsProductBarcodeUnique(ProductCurrent.ProductBarcode);
				if ((bool)res)
				{
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = "Штрихкод не используется.\n\n\t" + ProductCurrent.ProductBarcode,
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
						}
					);
				}
				else
				{
					MessageWindow.Show(
						new UniversalMessageContext
						{
							Content = "Штрихкод уже используется.\n\n\t" + ProductCurrent.ProductBarcode,
							WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
						}
					);
				}
			}
			else
			{
				MessageWindow.Show(
					new UniversalMessageContext
					{
						Content = validationRes.ErrorContent.ToString(),
						WindowOwnerForCenteringOrNull = _hostWindowForCenteringMessages,
					}
				);
			}
		}

		internal bool GetIsBarcodeValid()
		{
			var productBarcode = ProductCurrent.ProductBarcode;
			var valRes = BarcodeValidationRule.GetIsBarcodeValid(productBarcode);

			return valRes.IsValid;
		}

		internal void BarcodeSetNewUniqueBarcode()
		{
			var productTableAdapter = new ProductTableAdapter();
			string newUniqueProductBarcode = null;
			productTableAdapter.GetNewUniqueProductBarcode(ref newUniqueProductBarcode);

			ProductCurrent.ProductBarcode = newUniqueProductBarcode
				// для будующих изменений в логике защитим
				.ToUpper();
		}
	}
}
