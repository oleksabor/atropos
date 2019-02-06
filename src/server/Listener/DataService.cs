using Atropos.Common;
using Atropos.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Server.Listener
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class DataService : IDataService
	{
		public Curfew[] GetCurfews(int userId)
		{
			throw new NotImplementedException();
		}

		public UsageLog[] GetUsageLog(int userId)
		{
			throw new NotImplementedException();
		}

		public User[] GetUsers()
		{
			throw new NotImplementedException();
		}
	}
}
