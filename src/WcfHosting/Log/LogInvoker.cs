using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting.Logging
{
	/// <summary>
	/// custom invoker that will log method calls and parameters
	/// </summary>
	class LogInvoker : IOperationInvoker
	{
		private IOperationInvoker defaultInvoker;
		OperationDescription _opDescr;
		ILog _log;

		internal LogInvoker(IOperationInvoker defaultInvoker, OperationDescription opDescr, ILog logger)
		{
			this.defaultInvoker = defaultInvoker;
			_opDescr = opDescr;
			_log = logger;
		}

		public object[] AllocateInputs()
		{
			return defaultInvoker.AllocateInputs();
		}

		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			var instanceMethod = LogBeforeInvoke(instance, _opDescr, inputs);

			try
			{
				return defaultInvoker.Invoke(instance, inputs, out outputs);
			}
			catch (Exception ex)
			{
				var message = LogException(ex, instanceMethod);
				throw new FaultException(message);
			}
		}

		public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
		{
			LogBeforeInvoke(instance, _opDescr, inputs);
			return defaultInvoker.InvokeBegin(instance, inputs, callback, state);
		}

		string LogBeforeInvoke(object instance, OperationDescription descr, object[] inputs)
		{
			var instanceMethod = string.Format("{0} {1}", instance.GetType().Name, descr.SyncMethod);
			try
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("InvokeBegin {0}", instanceMethod);
				if (inputs.Any())
				{
					sb.Append(" parameters ");
					foreach (var obj in inputs)
						sb.AppendFormat("p:{0} ", obj);
				}
				_log.Debug(() => sb.ToString());
			}
			catch (Exception) { }
			return instanceMethod;
		}

		string LogException(Exception ex, string instanceMethod)
		{
			var exmessage = string.Format("failed to call {0}", instanceMethod);
			_log.ErrorException(exmessage, ex);

			var exstack = new StringBuilder();
			while (ex != null)
			{
				exstack.AppendFormat("{0}{1}", exstack.Length > 0 ? "||" : null, ex.Message);
				ex = ex.InnerException;
			}
			return exstack.ToString();
		}

		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		{
			return defaultInvoker.InvokeEnd(instance, out outputs, result);
		}

		public bool IsSynchronous
		{
			get { return defaultInvoker.IsSynchronous; }
		}
	}
}
