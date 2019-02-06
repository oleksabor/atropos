using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atropos.Common.Dto
{
	[DataContract(Namespace = Ns)]
	[XmlRoot(Namespace = Ns + "/usageLog")]
	public class UsageLog : IdDto
	{
		[DataMember(IsRequired = true)]
		public int UserId { get; set; }
		[DataMember(IsRequired = true)]
		public TimeSpan Used { get; set; }
		[DataMember(IsRequired = true)]
		public TimeSpan Started { get; set; }
		[DataMember(IsRequired = true)]
		public DateTime Date { get; set; }
	}
}
