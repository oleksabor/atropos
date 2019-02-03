using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atropos.Common.Dto
{
	public class IdDto
	{

		public const string Ns = "http://nowhere.com/Atropos/dto";

		[DataMember]
		public int Id { get; set; }
	}
}
