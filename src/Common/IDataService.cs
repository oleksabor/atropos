using Atropos.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common
{
	[ServiceContract(Namespace = "https://github.com/oleksabor/atropos")]
	public interface IDataService : IDisposable
	{
		[OperationContract]
		User[] GetUsers();
		[OperationContract]
		Curfew[] GetCurfews(string login);
		[OperationContract]
		UsageLog[] GetUsageLog(string login, DateTime date);
		[OperationContract]
		void SaveCurfew(Curfew[] values, string login);
	}
}
