using System;
using System.Globalization;
using System.ServiceModel;

namespace Farawla.Utilities
{
	public class PipeConfiguration
	{
		public string Uri { get; set; }		// e.g. net.pipe://localhost/Pipe
		public string Name { get; set; }	// e.g. foo
	}
	
	[ServiceContract(Namespace = "http://anaimi.com/")]
	public interface IPipeService
	{
		[OperationContract]
		string SendCommand(string action, string data);
	}
	
	#region Client
	
	public class PipeClient
	{
		private EndpointAddress address;
		private IPipeService proxy;

		public PipeClient(PipeConfiguration configuration)
		{
			address = new EndpointAddress(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", configuration.Uri, configuration.Name));
			proxy = ChannelFactory<IPipeService>.CreateChannel(new NetNamedPipeBinding(), address);
		}
		
		public string Send(string action, string data)
		{
			return proxy.SendCommand(action, data);
		}
	}
	
	#endregion
	
	#region Server
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class CommandService : IPipeService
	{
		public delegate void PipeHandler(string action, string data);
		
		private PipeHandler callback;

		public CommandService(PipeHandler callback)
		{
			this.callback = callback;
		}
		
		public string SendCommand(string action, string data)
		{
			if (callback != null)
			{
				callback(action, data);
			}
			
			return string.Empty;
		}
	}
	
	public class PipeServer
	{
		private ServiceHost host;
		private PipeConfiguration configuration;

		public PipeServer(PipeConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void Start(CommandService.PipeHandler callback)
		{
			var handler = new CommandService(callback);

			host = new ServiceHost(handler, new Uri(configuration.Uri));
			host.AddServiceEndpoint(typeof(IPipeService), new NetNamedPipeBinding(), configuration.Name);
			
			host.Open();
		}

		public void Stop()
		{
			if (host != null && host.State != CommunicationState.Closed)
			{
				host.Close();
				host = null;
			}
		}
	}
	
	#endregion
}
