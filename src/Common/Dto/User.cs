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
	[XmlRoot(Namespace = Ns + "/user")]
	public class User : IdDto
	{
		[DataMember]
		public string Login { get; set; }
		public string Name { get; set; }
	}
}
