using Atropos.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common
{
	[ServiceContract(Namespace = IdDto.Ns)]
	public interface IDataService
	{
		[OperationContract]
		User[] GetUsers();
		[OperationContract]
		Curfew[] GetCurfews(int userId);
		[OperationContract]
		UsageLog[] GetUsageLog(int userId);
	}
}
