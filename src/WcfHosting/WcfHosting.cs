using com.Tools.WcfHosting.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting
{
	/// <summary>
	/// helper class to start WCF service host
	/// </summary>
	/// <seealso cref="Quipu.Tools.WcfHosting.IWcfHost" />
	public class WcfHosting : IWcfHost
	{
		object _syncRoot = new object();

		IList<ServiceHost> _shList;

		//ILog log;

		//public WcfHosting(ILog logger)
		//{
		//	log = logger;
		//}

		/// <summary>
		/// Adds the wcf host using instance of interface T and settings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value of wcf service host.</param>
		/// <param name="settings">The settings.</param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException">
		/// no value to host
		/// or
		/// no value settings to host
		/// </exception>
		public T AddHost<T>(T value, IServiceCommunicationSettings settings)
		{
			if (value == null) throw new NullReferenceException("no value");
			if (settings == null) throw new NullReferenceException("no settings instance");
			if (settings.Host == null) throw new NullReferenceException("no settings.Host instance");
			return AddHost(value, GetBinding(settings), settings.Host.Uri, GetBehaviors(settings));
		}

		static ILog Log = LogProvider.GetCurrentClassLogger();

		/// <summary>
		/// Adds the wcf host using instance of interface T and settings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value of wcf service host.</param>
		/// <param name="binding">The binding.</param>
		/// <param name="url">The URL.</param>
		/// <param name="behaviors">The behaviors.</param>
		/// <returns></returns>
		/// <exception cref="ApplicationException"></exception>
		public T AddHost<T>(T value, Binding binding, string url, IEnumerable<IServiceBehavior> behaviors)
		{
			InitializeStorage();

			//log.InfoFormat("adding {0} host at {1}", value.GetType(), url);

			try
			{
				var sh = CreateHost(value, url);

				Add(sh, behaviors);

				AddServiceEndpoint<T>(url, sh, binding);

				Log.InfoFormat("hosting {0} using url:{1} with binding:{2} behaviour:{3}", value.GetType(), url, binding?.Name, string.Join(";", behaviors?.Select(_ => _.GetType().Name)));

				OpenHost(sh);

				Add(sh);

				return value;
			}
			catch (Exception e)
			{
				Log.WarnFormat("failed to host {0} using url:{1} with binding:{2} behaviour:{3}", value.GetType(), url, binding?.Name, string.Join(";", behaviors?.Select(_ => _.GetType().Name)));
				throw new ApplicationException(string.Format("failed to add host:{0} binding:{1}", url, binding?.Name), e);
			}
		}

		/// <summary>
		/// Adds the wcf host using type specified.
		/// </summary>
		/// <param name="type">The type.</param>
		public void AddHostType(Type type)
		{
			if (_shList == null)
				lock (_syncRoot)
					if (_shList == null)
						_shList = new List<ServiceHost>();

			//log.InfoFormat("adding {0} host", type.FullName);

			ServiceHost sh = new ServiceHost(type);

			OpenHost(sh);
			Add(sh);
		}

		/// <summary>
		/// Closes the host.
		/// </summary>
		/// <exception cref="ApplicationException"></exception>
		public void CloseHost()
		{
			if (_shList != null)
				lock (_syncRoot)
					foreach (ServiceHost sh in _shList)
						if (sh.State == CommunicationState.Opened)
							try
							{
								sh.Close();
								//log.DebugFormat("removed {0}", sh.BaseAddresses.First().AbsoluteUri);
							}
							catch (Exception ex)
							{
								throw new ApplicationException(string.Format("host {0} closing error", sh.Description.Name), ex);
							}
		}

		protected virtual void OpenHost(ServiceHost sh)
		{
			sh.Open();
		}

		protected virtual void AddServiceEndpoint<T>(string url, ServiceHost sh, Binding binding)
		{
			if (!string.IsNullOrEmpty(url))
				sh.AddServiceEndpoint(typeof(T), binding, new Uri(url));
		}

		protected virtual void Add(ServiceHost sh, IEnumerable<IServiceBehavior> behaviors)
		{
			if (behaviors != null)
			{
				var target = sh.Description.Behaviors;
				foreach (var b in behaviors)
				{
					var bType = b.GetType();
					if (target.Contains(bType))
						target.Remove(bType);
					target.Add(b);
				}
			}
		}

		protected virtual void Add(ServiceHost value)
		{
			lock (_syncRoot)
				_shList.Add(value);
		}

		protected virtual void InitializeStorage()
		{
			if (_shList == null)
				lock (_syncRoot)
					if (_shList == null)
						_shList = new List<ServiceHost>();
		}

		protected virtual ServiceHost CreateHost<T>(T value, string url)
		{
			Uri[] baseUri = null;
			if (!string.IsNullOrEmpty(url))
				baseUri = new Uri[] { new Uri(url) };

			return baseUri == null 
				? new ServiceHost(value) 
				: new ServiceHost(value, baseUri);
		}

		protected virtual Binding GetBinding(IServiceCommunicationSettings s)
		{
			return s.GetBinding(s.Host);
		}

		protected virtual IEnumerable<IServiceBehavior> GetBehaviors(IServiceCommunicationSettings s)
		{
			return s.Host.GetBehavior();
		}

	}
}
