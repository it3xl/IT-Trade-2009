using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITTrade.IT
{
	internal class Confirmation
	{
		
	}
	public class ConfirmationsChain
	{
		private readonly Queue<Action<Action<Boolean?>>> _confirmActions = new Queue<Action<Action<bool?>>>();
		private bool _isStarted;
		public Action DoneAction { get; set; }
		public ConfirmationsChain Enqueue(Action<Action<Boolean?>> checkAction)
		{
			if (_isStarted)
			{
				throw new Exception(string.Format("{0} уже используется.", typeof(ConfirmationsChain).Name));
			}
			_confirmActions.Enqueue(checkAction);
			return this;
		}

		public void Start()
		{
			_isStarted = true;
			InvokeNextOrFinish();
		}

		private void InvokeNextOrFinish()
		{
			if (0 < _confirmActions.Count)
			{
				var action = _confirmActions.Dequeue();
				action(Confirm);
			}
			else
			{
				if (DoneAction != null)
				{
					DoneAction();
				}
			}
		}

		private void Confirm(Boolean? result)
		{
			if (result != true)
			{
				_confirmActions.Clear();
				return;
			}
			InvokeNextOrFinish();
		}
	}
}
