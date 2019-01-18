using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace accountTimer.Event
{
	public class SessionData
	{
		public uint SessionID { get; set; }
		public string Domain { get; set; }
		public string User { get; set; }
		public SessionChangeReasonCode Reason { get; internal set; }
		WeakReference Sender;

		public override string ToString()
		{
			return string.Format("id:{0} user:{1} reason:{2} sender:{3} ", SessionID, User, Reason, SenderO?.GetType()?.Name);
		}

		public object SenderO { get { return Sender.IsAlive ? Sender.Target : null; } }

		public SessionData(uint id, SessionChangeReasonCode code, object sender)
		{
			SessionID = id;
			Reason = code;

			Sender = new WeakReference(sender);
		}
	}
}
