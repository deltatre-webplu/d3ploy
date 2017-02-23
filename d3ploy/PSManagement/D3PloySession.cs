using d3ploy.Hubs;
using d3ploy.PSRunner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace d3ploy.PSManagement
{
	public class D3PloySession : IDisposable
	{
		private readonly object _lock = new object();
		private readonly HashSet<string> _clientConnections = new HashSet<string>();
		private readonly D3PloyHub _hub;
		private D3PloyPowerShell _powerShell;

		public string User { get; private set; }
		public DateTime CreateDate { get; private set; }
		public DateTime LastAccessDate { get; private set; }
		public SessionStatus Status { get; private set; }
		public D3PloyClientConsoleSession Console { get; private set; }

		public D3PloySession(string user, D3PloyHub hub, string clientConnection)
		{
			User = user;
			CreateDate = DateTime.Now;
			LastAccessDate = DateTime.Now;
			Status = SessionStatus.Closed;

			_hub = hub;
			_clientConnections.Add(clientConnection);
		}

		public void Open()
		{
			lock (_lock)
			{
				Console = new D3PloyClientConsoleSession(_hub, User);
				_powerShell = new D3PloyPowerShell(Console);
				_powerShell.Open();
				Status = SessionStatus.Idle;
				LastAccessDate = DateTime.Now;
			}
		}

		public void Close()
		{
			lock (_lock)
			{
				if (_powerShell != null)
				{
					_powerShell.Dispose();
					_powerShell = null;
				}
				Console = null;
				Status = SessionStatus.Closed;
			}
		}

		public Task InvokePSAsync(string script)
		{
			return Task.Run(() =>
			{
				lock (_lock)
				{
					if (Status != SessionStatus.Idle)
					{
						throw new Exception("Session is not in a valid state");
					}

					Status = SessionStatus.Executing;

					try
					{
						Console.ScriptExecutionStarted();

						_powerShell.Invoke(script);

						Console.ScriptExecutionCompleted();
					}
					catch (Exception ex)
					{
						Console.ScriptExecutionFailed(ex.ToString());
					}
					finally
					{
						Status = SessionStatus.Idle;
					}
				}
			});
		}

		public IEnumerable<string> GetClientConnections()
		{
			lock (_lock)
			{
				return _clientConnections.ToList();
			}
		}

		public void AddClientConnection(string clientConnection)
		{
			lock (_lock)
			{
				_clientConnections.Add(clientConnection);
			}
		}

		public bool RemoveClientConnection(string clientConnection)
		{
			lock (_lock)
			{
				_clientConnections.Remove(clientConnection);

				return _clientConnections.Count == 0;
			}
		}

		public void Dispose()
		{
			Close();
		}

		public void Reset()
		{
			Close();
			Open();

			Console.WriteMessage(new ConsoleMessage("PowerShell runspace recreated"));
		}
	}

	public enum SessionStatus
	{
		Closed,
		Idle,
		Executing
	}
}