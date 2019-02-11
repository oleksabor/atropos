using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting
{
	public class CommunicationSettings : IServiceCommunicationSettings
	{
		public EndpointSettings Host { get; set; }

		public string FallbackBindingName { get; set; }

	}
	public class EndpointSettings
	{
		[System.Xml.Serialization.XmlAttribute("address")]
		public string Uri { get; set; }
		[System.Xml.Serialization.XmlAttribute("bindingConfiguration")]
		public string Binding { get; set; }
		[System.Xml.Serialization.XmlAttribute("behaviorConfiguration")]
		public string Behavior { get; set; }
	}

	public interface IServiceCommunicationSettings
	{
		EndpointSettings Host { get; set; }
		string FallbackBindingName { get; }

		//void Listen();
	}

}
