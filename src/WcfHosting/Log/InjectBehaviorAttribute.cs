using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting.Logging
{
	/// <summary>
	/// base class for behavior injection
	/// you have to override <see cref="CreateBehavior(string)"/> method to inject custom behavior
	/// </summary>
	/// <seealso cref="System.Attribute" />
	/// <seealso cref="System.ServiceModel.Description.IContractBehavior" />
	public abstract class InjectBehaviorAttribute : Attribute, IContractBehavior
	{
		IOperationBehavior _ob;

		public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
		{
			_ob = _ob ?? CreateBehavior(contractDescription.Name);
			if (_ob == null)
				throw new ArgumentException("no behavior instance was found to inject");
			foreach (OperationDescription opDescription in contractDescription.Operations)
				opDescription.Behaviors.Add(_ob);
		}

		public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
		}

		protected abstract IOperationBehavior CreateBehavior(string name);
	}


}
