using com.Tools.WcfHosting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting.Log
{
	/// <summary>
	/// creates <see cref="LogBehavior"/> when applied on type that is hosted using WCF
	/// </summary>
	/// <seealso cref="com.Tools.WcfHosting.Log.InjectBehaviorAttribute" />
	public class InjectLogBehaviorAttribute : InjectBehaviorAttribute
	{
		protected override IOperationBehavior CreateBehavior(string name)
		{
			var log = LogProvider.GetLogger(name);
			return new LogBehavior(log);
		}
	}

}
