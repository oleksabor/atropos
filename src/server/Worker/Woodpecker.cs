﻿using Atropos.Common.Logging;
using Atropos.Server.Event;
using Atropos.Server.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Atropos.Server.Worker
{
	public class Woodpecker : BackgroundTask
	{
		static ILog Log = LogProvider.GetCurrentClassLogger();
		SessionStatus _status;

		public Settings Config { get; }

		public Woodpecker(SessionStatus status, Settings config)
		{
			_status = status;
			Config = config;
		}

		public override void Start()
		{
			base.Start(Config.Interval.Woodpecker);
		}

		public override void Run()
		{
			var sd = _status.GetCurrent(this);
			if (sd != null)
				OnFound?.Invoke(sd);
		}

		public event SessionFound OnFound;
	}

	public delegate void SessionFound(SessionData data);
}
