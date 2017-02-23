using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace d3ploy.PSRunner
{
	public class D3PloyPowerShell : IDisposable
	{
		private readonly D3PloyPSHost _psHost;
		private Runspace _runSpace;
		private System.IO.DirectoryInfo _runspaceDirectory;

		public D3PloyPowerShell(ID3PloyConsole console)
		{
			_psHost = new D3PloyPSHost(console);
		}

		public void Open()
		{
			Close();

			_runSpace = RunspaceFactory.CreateRunspace(_psHost);
			_runSpace.Open();

			CreateTempDirectory();
			_runSpace.SessionStateProxy.Path.SetLocation(_runspaceDirectory.FullName);
		}

		public void Invoke(string script)
		{
			using (var powershell = PowerShell.Create())
			{
				powershell.Runspace = _runSpace;

				powershell.AddScript(script);

				// Now add the default outputter to the end of the pipe and indicate
				// that it should handle both output and errors from the previous
				// commands. This will result in the output being written using the PSHost
				// and PSHostUserInterface classes instead of returning objects to the hosting
				// application.
				powershell.AddCommand("out-default");
				powershell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

				powershell.Invoke();
			}
		}

		public void Close()
		{
			if (_runSpace != null)
			{
				_runSpace.Dispose();
				_runSpace = null;
			}

			if (_runspaceDirectory != null)
			{
				if (_runspaceDirectory.Exists)
					_runspaceDirectory.Delete(true);

				_runspaceDirectory = null;
			}
		}

		public void Dispose()
		{
			Close();
		}

		private void CreateTempDirectory()
		{
			var temp = System.IO.Path.GetTempPath();
			temp = System.IO.Path.Combine(temp, "d3ploy", Guid.NewGuid().ToString());

			_runspaceDirectory = System.IO.Directory.CreateDirectory(temp);
		}
	}
}
