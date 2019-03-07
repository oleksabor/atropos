using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Atropos.Common
{
	public class UserPrivilege
	{
		public bool IsAdmin()
		{
			using (var wi = WindowsIdentity.GetCurrent())
				return IsAdministratorsAccountName(wi.Name, Environment.MachineName);
		}

		public bool IsAdministratorsAccountName(string user, string machine)
		{
			//http://www.seirer.net/blog/2013/9/12/how-to-deal-with-localized-or-renamed-administrators-in-net
			DirectoryEntry localMachine = new DirectoryEntry($"WinNT://{machine}");
			string adminsSID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).ToString();

			string localizedAdmin = new SecurityIdentifier(adminsSID).Translate(typeof(NTAccount)).ToString();

			localizedAdmin = localizedAdmin.Replace(@"BUILTIN\", "");

			DirectoryEntry admGroup = localMachine.Children.Find(localizedAdmin, "group");

			var adminmembers = (IEnumerable)admGroup.Invoke("members", null);

			var regex = new Regex(user.Replace("\\", "/"), RegexOptions.IgnoreCase);

			//Retrieve each user name.
			foreach (object groupMember in adminmembers)
			{
				DirectoryEntry member = new DirectoryEntry(groupMember);

				string sidAsText = member.Path ?? member.Username ?? member.Name;

				if (regex.IsMatch(sidAsText))
					return true;
			}
			return false;
		}
	}
}
