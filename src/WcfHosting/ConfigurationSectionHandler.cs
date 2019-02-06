using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml;

namespace com.Tools.WcfHosting
{
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			XPathNavigator navigator = section.CreateNavigator();
			string attributeName = "type";
			string typeName = (string)navigator.Evaluate("string(@" + attributeName + ")");
			if (typeName.Length == 0)
				throw new ConfigurationErrorsException(string.Format("Attribute \'{0}\' not found for section \'{1}\'", attributeName, section.Name));
			Type type = Type.GetType(typeName);
			if (type == null)
				throw new ConfigurationErrorsException(string.Format("Type \'{0}\' couldn\'t be found. Section \'{1}\'", typeName, section.Name));
			XmlSerializer ser = new XmlSerializer(type);
			XmlNodeReader nodeReader = new XmlNodeReader(section);
			return ser.Deserialize(nodeReader);
		}

		#endregion
	}
}