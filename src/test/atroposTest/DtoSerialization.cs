using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Atropos.Common.Dto;
using System.IO;

namespace atroposTest.Common.Dto
{
	[TestFixture]
	public class DtoSerialization
	{
		[TestCase]
		public void UserSerialization()
		{
			var user = new User { Id = 123, Login = "abc", Name = "zxc" };
			
			using (var ms = new MemoryStream())
			{
				user.WriteTo(ms);

				ms.Position = 0;

				var user2 = new User();
				user2.MergeFrom(ms);

				Assert.AreEqual(user.Id, user2.Id);
				Assert.AreEqual(user.Name, user2.Name);
			}
		}

		[TestCase]
		public void CurfewSerialization()
		{
			var cf = new Curfew { Id = 123, UserId = 1234, Time = TimeSpan.FromHours(11), Break = TimeSpan.FromHours(1) };

			using (var ms = new MemoryStream())
			{
				cf.WriteTo(ms);

				ms.Position = 0;

				var cf2 = new Curfew();
				cf2.MergeFrom(ms);

				Assert.AreEqual(cf.Id, cf2.Id);
				Assert.AreEqual(cf.UserId, cf2.UserId);

				Assert.AreEqual(cf.Time, cf2.Time);
			}
		}

		[TestCase]
		public void UsageLogSerialization()
		{
			var ul = new UsageLog { Id = 123, UserId = 1234, Started = TimeSpan.FromHours(11), Used = TimeSpan.FromHours(1), Date = DateTime.Today };

			using (var ms = new MemoryStream())
			{
				ul.WriteTo(ms);

				ms.Position = 0;

				var ul2 = new UsageLog();
				ul2.MergeFrom(ms);

				Assert.AreEqual(ul.Id, ul2.Id);
				Assert.AreEqual(ul.UserId, ul2.UserId);

				Assert.AreEqual(ul.Date, ul2.Date);
				Assert.AreEqual(ul.Used, ul2.Used);
			}
		}
	}
}
