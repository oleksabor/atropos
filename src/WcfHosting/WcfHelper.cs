using com.Tools.WcfHosting.Logging;
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
	/// <summary>
	/// wcf binding configuraion helper
	/// </summary>
	public static class WcfHelper
	{
		/// <summary>
		/// Gets the binding.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static Binding GetBinding(this IServiceCommunicationSettings settings, EndpointSettings value)
		{
			Binding binding = null;
			if (value.Binding == null)
				return binding;
			var config = GetSection();
			foreach (var bc in config.Bindings.BindingCollections)
				// In config we can have more configurated Bindings with this code we are forcing the CRM Binding to be first 
				foreach (IBindingConfigurationElement element in bc.ConfiguredBindings)
					if (element.Name.Equals(value.Binding, StringComparison.OrdinalIgnoreCase))
					{
						binding = (Binding)Activator.CreateInstance(bc.BindingType);
						binding.Name = element.Name;
						element.ApplyConfiguration(binding);
						//Log.Debug("value {0} configured with {1}", value.Uri, element.Name);
						return binding;
					}
			return new WSHttpBinding() { Name = settings.FallbackBindingName };
		}

		/// <summary>
		/// Gets the behavior.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static IEnumerable<IServiceBehavior> GetBehavior(this EndpointSettings value)
		{
			var behaviors = new Collection<IServiceBehavior>();
			var config = GetSection();
			if (!string.IsNullOrEmpty(value.Behavior))
			{
				foreach (ServiceBehaviorElement bhe in config.Behaviors.ServiceBehaviors)
					if (bhe.Name.Equals(value.Behavior, StringComparison.OrdinalIgnoreCase))
						foreach (var sbh in bhe)
						{
							var b = sbh.GetType().InvokeMember("CreateBehavior", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, sbh, null) as IServiceBehavior;
							if (b != null)
								behaviors.Add(b);
						}
			}
			return behaviors;
		}

		static ServiceModelSectionGroup GetSection()
		{
			var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var config = ServiceModelSectionGroup.GetSectionGroup(appConfig);
			return config;
		}
	}
}
