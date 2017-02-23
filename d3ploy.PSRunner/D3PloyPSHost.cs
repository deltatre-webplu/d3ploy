using System;
using System.Globalization;
using System.Management.Automation.Host;

namespace d3ploy.PSRunner
{
	public class D3PloyPSHost : PSHost
	{
		private readonly Guid _instanceId = Guid.NewGuid();
		private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

		private readonly D3PloyPSHostUserInterface _hostUserInterface;

		public D3PloyPSHost(ID3PloyConsole console)
		{
			_hostUserInterface = new D3PloyPSHostUserInterface(console);
		}

		/// <summary>
    /// Gets the culture information to use. This implementation 
    /// returns a snapshot of the culture information of the thread 
    /// that created this object.
    /// </summary>
    public override CultureInfo CurrentCulture
    {
			get { return _cultureInfo; }
    }
        
    /// <summary>
    /// Gets the UI culture information to use. This implementation 
    /// returns a snapshot of the UI culture information of the thread 
    /// that created this object.
    /// </summary>
    public override CultureInfo CurrentUICulture
    {
			get { return _cultureInfo; }
    }

    /// <summary>
    /// Gets an identifier for this host. This implementation always 
    /// returns the GUID allocated at instantiation time.
    /// </summary>
    public override Guid InstanceId
    {
      get { return _instanceId; }
    }

    /// <summary>
    /// Gets a string that contains the name of this host implementation. 
    /// Keep in mind that this string may be used by script writers to
    /// identify when your host is being used.
    /// </summary>
    public override string Name
    {
			get { return "D3PloyPSHost"; }
    }

    /// <summary>
    /// Gets an instance of the implementation of the PSHostUserInterface
    /// class for this application. This instance is allocated once at startup time
    /// and returned every time thereafter.
    /// </summary>
    public override PSHostUserInterface UI
    {
			get { return _hostUserInterface; }
    }

    /// <summary>
    /// Gets the version object for this application. Typically this 
    /// should match the version resource in the application.
    /// </summary>
    public override Version Version
    {
      get { return GetType().Assembly.GetName().Version; }
    }
 
    /// <summary>
    /// This API Instructs the host to interrupt the currently running 
    /// pipeline and start a new nested input loop. In this example this 
    /// functionality is not needed so the method throws a 
    /// NotImplementedException exception.
    /// </summary>
    public override void EnterNestedPrompt()
    {
    }

    /// <summary>
    /// This API instructs the host to exit the currently running input loop. 
    /// In this example this functionality is not needed so the method 
    /// throws a NotImplementedException exception.
    /// </summary>
    public override void ExitNestedPrompt()
    {
    }

    /// <summary>
    /// This API is called before an external application process is 
    /// started. Typically it is used to save state so that the parent  
    /// can restore state that has been modified by a child process (after 
    /// the child exits). In this example this functionality is not  
    /// needed so the method returns nothing.
    /// </summary>
    public override void NotifyBeginApplication()
    {
		}

    /// <summary>
    /// This API is called after an external application process finishes.
    /// Typically it is used to restore state that a child process has
    /// altered. In this example, this functionality is not needed so  
    /// the method returns nothing.
    /// </summary>
    public override void NotifyEndApplication()
    {
		}


    /// <summary>
    /// Indicate to the host application that exit has
    /// been requested. Pass the exit code that the host
    /// application should use when exiting the process.
    /// </summary>
    /// <param name="exitCode">The exit code that the 
    /// host application should use.</param>
    public override void SetShouldExit(int exitCode)
    {
		}
	}
}
