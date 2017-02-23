using d3ploy.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace d3ploy.PSManagement
{
	public class D3PloySessionManager
	{
		private readonly object _lock = new object();
		private readonly List<D3PloySession> _sessions = new List<D3PloySession>();

		public D3PloySession GetClientSession(string user)
		{
			lock (_lock)
			{
				var session = _sessions.FirstOrDefault(p => string.Equals(p.User, user, StringComparison.InvariantCultureIgnoreCase));
				if (session == null)
				{
					throw new Exception(string.Format("Session for user {0} not found", user));
				}

				return session;
			}
		}

		public D3PloySession CreateClientSession(D3PloyHub hub, string user, string connectionId)
		{
			lock (_lock)
			{
				var session = _sessions.FirstOrDefault(p => string.Equals(p.User, user, StringComparison.InvariantCultureIgnoreCase));
				if (session == null)
				{
					session = new D3PloySession(user, hub, connectionId);
					session.Open();
					_sessions.Add(session);
				}

				session.AddClientConnection(connectionId);

				return session;
			}
		}

		public void CloseClientSession(string user, string connectionId)
		{
			lock (_lock)
			{
				var session = _sessions.FirstOrDefault(p => string.Equals(p.User, user, StringComparison.InvariantCultureIgnoreCase));
				if (session != null)
				{
					var lastConnection = session.RemoveClientConnection(connectionId);
					if (lastConnection)
					{
						_sessions.Remove(session);
						session.Close();
					}
				}
			}
		}

		public IEnumerable<D3PloySession> GetSessions()
		{
			lock (_lock)
			{
				return _sessions.ToList();
			}
		}
	}
}