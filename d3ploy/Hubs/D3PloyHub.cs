using System.Text;
using d3ploy.Assets;
using d3ploy.PSRunner;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using d3ploy.PSManagement;
using System.Linq;

namespace d3ploy.Hubs
{
	[Authorize]
	public class D3PloyHub : Hub
	{
		private readonly IAssetsFolderRepository _assetsFolderRepository;
		private readonly D3PloySessionManager _sessionManager;

		public D3PloyHub(D3PloySessionManager sessionManager, IAssetsFolderRepository assetsFolderRepository)
		{
			_sessionManager = sessionManager;
			_assetsFolderRepository = assetsFolderRepository;
		}

		public override Task OnConnected()
		{
			_sessionManager.CreateClientSession(this, Context.User.Identity.Name, Context.ConnectionId);

			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			_sessionManager.CloseClientSession(Context.User.Identity.Name, Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		public override Task OnReconnected()
		{
			_sessionManager.CreateClientSession(this, Context.User.Identity.Name, Context.ConnectionId);

			return base.OnReconnected();
		}

		public void ExecuteAsset(string assetPath)
		{
			var libraries = _assetsFolderRepository.GetScriptIncludes(assetPath);
			var sb = new StringBuilder();

			foreach (var library in libraries)
			{
				sb.AppendFormat("Write-Debug 'Loading {0}...'\n", library);
				sb.AppendLine(_assetsFolderRepository.GetFileContent(library));
			}

			var fileContent = _assetsFolderRepository.GetFileContent(assetPath);

			sb.AppendFormat("Write-Debug 'Executing {0}...'\n", assetPath);
			sb.AppendLine(fileContent);
			sb.AppendLine("Write-Debug 'Execution finished'");

			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			session.InvokePSAsync(sb.ToString());
		}

		public void Execute(string expression)
		{
			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			session.InvokePSAsync(expression);
		}

		public void OnReadLine(string value)
		{
			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			session.Console.OnReadLine(value);
		}

		public SessionStatus GetSessionStatus()
		{
			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			return session.Status;
		}

		public void ResetSession()
		{
			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			session.Reset();
		}

		public void PrintSessions()
		{
			var session = _sessionManager.GetClientSession(Context.User.Identity.Name);

			foreach (var s in _sessionManager.GetSessions())
			{
				var msg = string.Format("User: {0}, Connections: {1}", s.User, s.GetClientConnections().Count());
				session.Console.WriteMessage(new ConsoleMessage(msg));
			}
		}
	}
}