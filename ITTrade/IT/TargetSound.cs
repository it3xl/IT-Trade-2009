namespace ITTrade.IT
{
	public enum TargetSound
	{
		/// <summary>
		/// Соответствует звуку, когда товар не найден в базе по штрихкоду.
		/// </summary>
		UnknownProductBarcode,
		/// <summary>
		/// Соответствует звуку, когда товар не используется в текущем бизнес объекте. Например в накладной.
		/// </summary>
		UnusedProductBarcode,

		/// <summary>
		/// Звук означает, что осталось одна позиция чего-то.
		/// </summary>
		QuantityEqualsOne,

		/// <summary>
		/// Звук означает, что количество равно нулю.
		/// </summary>
		QuantityEqualsZero,
	}
}
