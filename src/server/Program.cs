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

					Configure(container);

					x.EnableSessionChanged();
					x.EnablePauseAndContinue();
					x.Service(_ =>
					{
						container.Configure(c => c.For<ServiceOptions>().Use(new ServiceOptions { Name = _.ServiceName }));
						return container.GetInstance<ServiceImpl>();
					});

					x.RunAsPrompt()
							.DependsOnEventLog()
							.StartAutomatically();
				});
			}
			Log.Info("going home");
		}

		static void Configure(IContainer value)
		{
			value.Configure(_ =>
			{
				_.Scan(a =>
				{
					a.TheCallingAssembly();
					a.AssemblyContainingType<Atropos.Server.Db.IData>();
					a.WithDefaultConventions();
				});
			});
		}
	}


}
