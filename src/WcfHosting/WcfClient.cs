using com.Tools.WcfHosting.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace com.Tools.WcfHosting
{
	/// <summary>
	/// helper class to execute methods from remote wcf service
	/// opens connection and releases it
	/// </summary>
	/// <typeparam name="TRemoteInterface">The type of the remote interface.</typeparam>
	/// <seealso cref="System.IDisposable" />
	public class WcfClient<TRemoteInterface> : IDisposable
	{
		protected ChannelFactory<TRemoteInterface> _channel;
		protected TRemoteInterface _proxy;

		IServiceCommunicationSettings _settings;
		static ILog _log = LogProvider.GetCurrentClassLogger();

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url { get { return GetSettings().Host.Uri; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="WcfClient{TRemoteInterface}"/> class.
		/// </summary>
		/// <param name="remoteSettings">The remote service settings.</param>
		/// <param name="logger">The logger.</param>
		public WcfClient(IServiceCommunicationSettings remoteSettings)
		{
			_settings = remoteSettings;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public void Dispose()
		{
			if (_channel != null)
				try
				{
					var ch = _channel;
					_channel = null;

					if (ch.State == CommunicationState.Opened)
						ch.Close();
					else
						ch.Abort();
				}
				catch (Exception e)
				{
					_log.ErrorException("failed to dispose channel", e);
				}
		}

		/// <summary>
		/// Opens the channel if no one was opened.
		/// </summary>
		/// <exception cref="ConfigurationErrorsException">
		/// failed to get settings
		/// or
		/// failed to read Uri
		/// </exception>
		protected virtual void OpenChannel()
		{
			if (_channel == null)
			{
				var settings = GetSettings();
				if (settings == null || settings.Host == null) throw new ArgumentException("failed to get settings");
				if (string.IsNullOrEmpty(settings.Host.Uri)) throw new ArgumentException("failed to read Uri");

				var binding = settings.GetBinding(settings.Host);
				_log.Debug(() => string.Format("connecting to url:{0} binding:{1}", settings.Host.Uri, binding.Name));

				var endpoint = new EndpointAddress(settings.Host.Uri);
				_channel = new ChannelFactory<TRemoteInterface>(binding, endpoint);
				_channel.Open();
				_proxy = _channel.CreateChannel(endpoint);
			}
		}

		protected void AbortChanel(Exception ex)
		{
			_log.WarnFormat("Aborting chanel '{0}'", Url);
			Dispose();
		}

		/// <summary>
		/// Gets the settings. Can be overriden if there is no possibility to use constructor injection
		/// </summary>
		/// <returns></returns>
		public virtual IServiceCommunicationSettings GetSettings()
		{
			return _settings;
		}

		/// <summary>
		/// Gets result from remote service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="getResult">The get result.</param>
		/// <returns></returns>
		public T Get<T>(Func<TRemoteInterface, T> getResult)
		{
			try
			{
				OpenChannel();

				return getResult(_proxy);
			}
			catch (Exception ex)
			{
				_log.Error(() => string.Format("failed to get message {0}", ex.Message));
				AbortChanel(ex);
				throw;
			}
		}

		/// <summary>
		/// Executes a method on remote service.
		/// </summary>
		/// <param name="setValue">The set value.</param>
		public void Set(Action<TRemoteInterface> setValue)
		{
			try
			{
				OpenChannel();

				setValue(_proxy);
			}
			catch (Exception ex)
			{
				_log.Error(() => string.Format("failed to set message {0}", ex.Message));
				AbortChanel(ex);
				throw;
			}
		}
	}
}
