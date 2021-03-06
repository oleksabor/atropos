﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Atropos.Server.Event
{
	public class SessionData
	{
		public uint SessionID { get; set; }
		public string Domain { get; set; }
		public string User { get; set; }
		public Kind Reason { get; internal set; }
		WeakReference SenderObject;

		public bool IsLocked => Reason == Kind.Locked;

		public TimeSpan Spent { get; set; }

		public override string ToString()
		{
			return string.Format("id:{0} user:{1} reason:{2} sender:{3} spent:{4}", SessionID, User, Reason, Sender?.GetType()?.Name, Spent);
		}

		public object Sender { get { return SenderObject.IsAlive ? SenderObject.Target : null; } set { SenderObject = new WeakReference(value); } }

		public SessionData(uint id, Kind code, object sender)
		{
			SessionID = id;
			Reason = code;

			Sender = sender;
		}
	}
}
