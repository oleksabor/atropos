using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting
{
	/// <summary>
	/// helper class to start WCF service host
	/// </summary>
	public interface IWcfHost
	{
		/// <summary>
		/// Adds the wcf host using instance of interface T and settings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value of wcf service host.</param>
		/// <param name="settings">The settings.</param>
		/// <returns></returns>
		T AddHost<T>(T value, IServiceCommunicationSettings settings);
		void AddHostType<T>(IServiceCommunicationSettings settings, IEnumerable<IServiceBehavior> customBehaviors = null);
		/// <summary>
		/// Closes the host.
		/// </summary>
		void CloseHost();
	}
}
