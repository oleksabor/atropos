using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atropos.Common.Dto
{
	[DataContract(Namespace = Ns)]
	[XmlRoot(Namespace = Ns + "/curfew")]
	public class Curfew : IdDto
	{
		[DataMember(IsRequired = true)]
		public int UserId { get; set; }

		[DataMember(IsRequired = true)]
		public string WeekDay { get; set; }

		[DataMember(IsRequired = true)]
		public TimeSpan Time { get; set; }

		[DataMember]
		public TimeSpan Break { get; set; }
	}
}
