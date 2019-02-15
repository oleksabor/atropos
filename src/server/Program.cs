﻿using Atropos.Common.Logging;
using Atropos.Server.Db;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using com.Tools.WcfHosting;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.LibLog;
using Topshelf.ServiceConfigurators;

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
					x.UnhandledExceptionPolicy = Topshelf.Runtime.UnhandledExceptionPolicyCode.LogErrorAndStopService;

					x.OnException(_ =>
					{
						var message = _.Message;
						var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
						if (lines.Length > 1)
							message = lines[0];
						Log.ErrorFormat("failed to startrun a service '{0}'", message);
					});

					Configure(container);

					x.EnableSessionChanged();
					x.EnablePauseAndContinue();

					x.Service(_ => container.GetInstance<ServiceImpl>());

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

				_.For<IData>().Use(() => new Data("Db")).AlwaysUnique();
				_.For<IWcfHost>().Use<WcfHosting>();
				_.For<CommunicationSettings>().Use(context => new CommunicationSettings { Host = new EndpointSettings { Uri = "net.pipe://localhost/atropos", Binding = "atropos_binding" } });
				_.For<Instance>().Singleton();
				});
		}
	}


}
