using d3ploy.Hubs;
using d3ploy.PSRunner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;

namespace d3ploy.PSManagement
{
	public class D3PloyClientConsoleSession : ID3PloyConsole
	{
		private readonly ManualResetEvent _waitReadLine = new ManualResetEvent(false);
		private string _readLineValue = null;
		private static readonly TimeSpan ReadLineTimeOut = TimeSpan.FromSeconds(60);

		private readonly string _user;
		private readonly D3PloyHub _hub;

		public D3PloyClientConsoleSession(D3PloyHub hub, string user)
		{
			_user = user;
			_hub = hub;
		}

		public string ReadLine()
		{
			_waitReadLine.Reset();
			_hub.Clients.User(_user).ReadLine();

			var signaled = _waitReadLine.WaitOne(ReadLineTimeOut);

			_hub.Clients.User(_user).EndReadLine();

			if (!signaled)
				throw new Exception("ReadLine timeout");

			return _readLineValue;
		}

		public SecureString ReadLineAsSecureString()
		{
			_waitReadLine.Reset();
			_hub.Clients.User(_user).ReadLine("password");

			var signaled = _waitReadLine.WaitOne(ReadLineTimeOut);

			_hub.Clients.User(_user).EndReadLine();

			if (!signaled)
				throw new Exception("ReadLine timeout");

			return ConvertToSecureString(_readLineValue);
		}

		public void WriteMessage(ConsoleMessage message)
		{
			_hub.Clients.User(_user).WriteMessage(message);
		}

		public void OnReadLine(string value)
		{
			_readLineValue = value;
			_waitReadLine.Set();
		}

		public void ScriptExecutionStarted()
		{
			_hub.Clients.User(_user).executionStarted();
		}

		public void ScriptExecutionFailed(string errorMessage)
		{
			_hub.Clients.User(_user).executionFailed(errorMessage);
		}

		public void ScriptExecutionCompleted()
		{
			_hub.Clients.User(_user).executionCompleted();
		}

		private SecureString ConvertToSecureString(string password)
		{
			if (password == null)
				throw new ArgumentNullException(nameof(password));

			var securePassword = new SecureString();

			foreach (var c in password)
				securePassword.AppendChar(c);

			securePassword.MakeReadOnly();
			return securePassword;
		}
	}
}