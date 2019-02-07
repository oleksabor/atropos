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
	/// custom behavior adds IOperationInvoker to log WCF methods execution
	/// </summary>
	internal class LogBehavior : IOperationBehavior
	{
		ILog _logger;

		public LogBehavior(ILog logger)
		{
			_logger = logger;
		}

		public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
		{
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
		{
			dispatchOperation.Invoker = new LogInvoker(dispatchOperation.Invoker, operationDescription, _logger);
		}

		public void Validate(OperationDescription operationDescription)
		{
		}
	}
}
