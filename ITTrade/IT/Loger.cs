using System;
using System.Text;
using System.IO;
using System.Threading;

namespace ITTrade.IT
{
	/// <summary>
	/// писатель логов в файлы
	/// <para>Преимущества</para>
	/// Гарантированно пишет всегда, без потерь. Пишет в указанную папку, а не только в корень приложения. Тонкая настройка вывода.
	/// </summary>
	public static class Logger
	{

		public static bool _isStreamWriterInited;
		public static StreamWriter _streamWriter;
		private static StreamWriter StreamWriter
		{
			get
			{
				if (_isStreamWriterInited)
				{


					return _streamWriter;
				}


				if (_streamWriter == null)
				{
					_isStreamWriterInited = true;
					try
					{
						AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

						const String LogsDirectoryName = "!Logs";

						Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, LogsDirectoryName));

						_streamWriter = File.AppendText(Path.Combine(
							Environment.CurrentDirectory,
							LogsDirectoryName+"/Log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log"
							));
					}
// ReSharper disable EmptyGeneralCatchClause
					catch
// ReSharper restore EmptyGeneralCatchClause
					{
					}
				}

				return _streamWriter;
			}
		}


		public static bool Allowed
		{
			get
			{
				return StreamWriter != null;
			}
		}


		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			if (StreamWriter != null)
			{
				StreamWriter.Dispose();
			}
		}


		/// <summary>
		/// Записать лог
		/// </summary>
		public static void Write(string message)
		{
			ThreadPool.QueueUserWorkItem(state=>WriteInternaly(message));
		}

		/// <summary>
		/// Записать лог
		/// </summary>
		private static void WriteInternaly(string message)
		{
			if (Allowed==false)
			{
				return;
			}
			
			try
			{
				StreamWriter.Write(DateTime.Now.ToString("dd.mm.yy hh:mm:ss"));
				StreamWriter.Write(" - ");
				StreamWriter.WriteLine(message);
				// если не вызвать Flush, то при выгрузке приложения запись потеряется. Также, видимо, как и с классом Trace
				StreamWriter.Flush();
			}
// ReSharper disable EmptyGeneralCatchClause
			catch
			{
			}
// ReSharper restore EmptyGeneralCatchClause
		}

		public static void Write(Exception ex, String comment)
		{
			if (Allowed == false)
			{
				return;
			}

			try
			{
				var exceptionData = new StringBuilder();
				if (comment != null)
				{
					exceptionData.AppendLine(comment);
				}

				if (ex != null)
				{
					exceptionData.AppendLine(ex.Message);
					exceptionData.AppendLine(ex.StackTrace);
				}
				else
				{
					exceptionData.AppendLine("Вместо исключения передали null.");
				}

				WriteInternaly(exceptionData.ToString());

				if (ex != null
					&& ex.InnerException != null)
				{
					Write(ex.InnerException, "InnerException");
				}
			}
			catch
			{
				WriteInternaly("Ошибка при записи исключения в лог.");
			}
		}


		public static void Write(Exception ex)
		{
			Write(ex, null);
		}
	}
}
