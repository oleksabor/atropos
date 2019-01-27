using Atropos.Common.Logging;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.LibLog;
using Topshelf.StructureMap;

namespace Atropos.Server
{
	class Program
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			Log.Info("starting");
			using (var container = new Container())
			{
				HostFactory.Run(x =>
				{
					x.UseLibLog();
					x.UseAssemblyInfoForServiceInfo();

					x.UseStructureMap(container);

					x.EnableSessionChanged();
					x.Service(_ => new ServiceImpl(_.ServiceName, new Woodpecker(), new Accounter(new Instance(container))));

					x.RunAsPrompt()
							.DependsOnEventLog()
							.StartAutomatically();
				});
			}
			Log.Info("closing and going home");
		}
	}
}
