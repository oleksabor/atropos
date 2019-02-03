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
		[DataMember]
		public int UserId { get; set; }
		[DataMember]
		public string WeekDay { get; set; }
		[DataMember]
		public TimeSpan Time { get; set; }
		[DataMember]
		public TimeSpan Break { get; set; }
	}
}
