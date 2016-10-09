using System;
using System.Linq;
using System.Windows;
using IT.WPF;
using IT;
using ITTrade.IT;
using System.Windows.Threading;

namespace ITTrade
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			Lock l2;
			if (false == new[] { "itretyakov", "nata" }.Any(el => el.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase)))
			{
				Logger._isStreamWriterInited = true;
				Logger._streamWriter = null;
			}

			WpfSingleInstance.Make();

			LoadCompleted += (s, a) =>{IsLoaded = true;};

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Dispatcher.UnhandledException += Dispatcher_UnhandledException;
			Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;
			DispatcherUnhandledException += Current_DispatcherUnhandledException;

			Lock l;
			if(false == new[] { "itretyakov", "nata" }.Any(el=>el.Equals(Environment.MachineName,StringComparison.InvariantCultureIgnoreCase)))
			{
				Current.Dispatcher.BeginInvoke(new Action(()=>Current.Shutdown()));
			}

			base.OnStartup(e);
		}

		protected bool IsLoaded { get; set; }

		static void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Logger.Write(e.Exception, "From Current_DispatcherUnhandledException");
		}

		static void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
		{
			// тут можно метить исключения, как не отменяемые
			//e.RequestCatch = false;
		}

		static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Lock l;
			if (new[] { "itretyakov", "nata" }.Any(el => el.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase)))
			{
				e.Handled = true;
			}

			Logger.Write(e.Exception, "From Dispatcher_UnhandledException");
			MessageBox.Show(ExceptionUtils.GetMessageFromLastInnerException(e.Exception));
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Write(e.ExceptionObject as Exception, "From CurrentDomain_UnhandledException");
		}
	}
}
