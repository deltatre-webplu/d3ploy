using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace d3ploy.PSRunner
{
	internal class D3PloyPSHostUserInterface : PSHostUserInterface
	{
		private readonly ID3PloyConsole _console;

		/// <summary>
		/// An instance of the PSRawUserInterface object.
		/// </summary>
		private readonly D3PloyRawUserInterface _rawUi = new D3PloyRawUserInterface();


		public D3PloyPSHostUserInterface(ID3PloyConsole console)
		{
			_console = console;
		}

		/// <summary>
		/// Gets an instance of the PSRawUserInterface object for this host
		/// application.
		/// </summary>
		public override PSHostRawUserInterface RawUI
		{
			get { return _rawUi; }
		}

		/// <summary>
		/// Prompts the user for input. 
		/// </summary>
		/// <param name="caption">The caption or title of the prompt.</param>
		/// <param name="message">The text of the prompt.</param>
		/// <param name="descriptions">A collection of FieldDescription objects that 
		/// describe each field of the prompt.</param>
		/// <returns>A dictionary object that contains the results of the user 
		/// prompts.</returns>
		public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			Write(caption + "\n" + message + " ");

			var results = new Dictionary<string, PSObject>();
			foreach (var fd in descriptions)
			{
				var label = GetHotkeyAndLabel(fd.Label);
				
				WriteLine(label[1]);

				object userData;
				if (fd.ParameterTypeName == nameof(SecureString))
					userData = ReadLineAsSecureString();
				else
					userData = ReadLine();

				if (userData == null)
				{
					return null;
				}

				results[fd.Name] = PSObject.AsPSObject(userData);
			}

			return results;
		}

		/// <summary>
		/// Provides a set of choices that enable the user to choose a 
		/// single option from a set of options. 
		/// </summary>
		/// <param name="caption">Text that proceeds (a title) the choices.</param>
		/// <param name="message">A message that describes the choice.</param>
		/// <param name="choices">A collection of ChoiceDescription objects that describe 
		/// each choice.</param>
		/// <param name="defaultChoice">The index of the label in the Choices parameter 
		/// collection. To indicate no default choice, set to -1.</param>
		/// <returns>The index of the Choices parameter collection element that corresponds 
		/// to the option that is selected by the user.</returns>
		public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			WriteLine(caption);
			WriteLine(message);

			var index = 0;
			foreach (var choice in choices)
			{
				WriteLine(string.Format("{0}: {1}", index, choice.Label));
				index++;
			}

			var selected = ReadLine();

			if (string.IsNullOrWhiteSpace(selected))
				selected = defaultChoice.ToString();

			return int.Parse(selected);
		}

		/// <summary>
		/// Prompts the user for credentials with a specified prompt window caption, 
		/// prompt message, user name, and target name. In this example this 
		/// functionality is not needed so the method throws a 
		/// NotImplementException exception.
		/// </summary>
		/// <param name="caption">The caption for the message window.</param>
		/// <param name="message">The text of the message.</param>
		/// <param name="userName">The user name whose credential is to be prompted for.</param>
		/// <param name="targetName">The name of the target for which the credential is collected.</param>
		/// <returns>Throws a NotImplementedException exception.</returns>
		public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
		{
			throw new NotImplementedException("PromptForCredential not implemented");
		}

		/// <summary>
		/// Prompts the user for credentials by using a specified prompt window caption, 
		/// prompt message, user name and target name, credential types allowed to be 
		/// returned, and UI behavior options. In this example this functionality 
		/// is not needed so the method throws a NotImplementException exception.
		/// </summary>
		/// <param name="caption">The caption for the message window.</param>
		/// <param name="message">The text of the message.</param>
		/// <param name="userName">The user name whose credential is to be prompted for.</param>
		/// <param name="targetName">The name of the target for which the credential is collected.</param>
		/// <param name="allowedCredentialTypes">A PSCredentialTypes constant that 
		/// identifies the type of credentials that can be returned.</param>
		/// <param name="options">A PSCredentialUIOptions constant that identifies the UI 
		/// behavior when it gathers the credentials.</param>
		/// <returns>Throws a NotImplementedException exception.</returns>
		public override PSCredential PromptForCredential( string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
		{
			throw new NotImplementedException("PromptForCredential not implemented");
		}


		/// <summary>
		/// Reads characters that are entered by the user until a newline 
		/// (carriage return) is encountered.
		/// </summary>
		/// <returns>The characters that are entered by the user.</returns>
		public override string ReadLine()
		{
			return _console.ReadLine();
		}

		/// <summary>
		/// Reads characters entered by the user until a newline (carriage return) 
		/// is encountered and returns the characters as a secure string. 
		/// </summary>
		public override SecureString ReadLineAsSecureString()
		{
			return _console.ReadLineAsSecureString();
		}

		/// <summary>
		/// Writes characters to the output display of the host.
		/// </summary>
		/// <param name="value">The characters to be written.</param>
		public override void Write(string value)
		{
			_console.WriteMessage(new ConsoleMessage(value)
			{
				Mode = MessageMode.Write
			});
		}

		/// <summary>
		/// Writes characters to the output display of the host with possible 
		/// foreground and background colors. 
		/// </summary>
		/// <param name="foregroundColor">The color of the characters.</param>
		/// <param name="backgroundColor">The backgound color to use.</param>
		/// <param name="value">The characters to be written.</param>
		public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
			_console.WriteMessage(new ConsoleMessage(value) 
			{
				Mode = MessageMode.Write ,
				BackgroundColor = backgroundColor.ToString(),
				ForegroundColor = foregroundColor.ToString()
			});
		}

		/// <summary>
		/// Writes a line of characters to the output display of the host 
		/// with foreground and background colors and appends a newline (carriage return). 
		/// </summary>
		/// <param name="foregroundColor">The forground color of the display. </param>
		/// <param name="backgroundColor">The background color of the display. </param>
		/// <param name="value">The line to be written.</param>
		public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
			_console.WriteMessage(new ConsoleMessage(value)
			{
				BackgroundColor = backgroundColor.ToString(),
				ForegroundColor = foregroundColor.ToString()
			});
		}

		/// <summary>
		/// Writes a debug message to the output display of the host.
		/// </summary>
		/// <param name="message">The debug message that is displayed.</param>
		public override void WriteDebugLine(string message)
		{
			_console.WriteMessage(new ConsoleMessage(message)
			{
				Level = MessageLevel.Debug
			});
		}

		/// <summary>
		/// Writes an error message to the output display of the host.
		/// </summary>
		/// <param name="value">The error message that is displayed.</param>
		public override void WriteErrorLine(string value)
		{
			_console.WriteMessage(new ConsoleMessage(value)
			{
				Level = MessageLevel.Error
			});
		}

		/// <summary>
		/// Writes a newline character (carriage return) 
		/// to the output display of the host. 
		/// </summary>
		public override void WriteLine()
		{
			_console.WriteMessage(new ConsoleMessage(string.Empty)
			{
			});
		}

		/// <summary>
		/// Writes a line of characters to the output display of the host 
		/// and appends a newline character(carriage return). 
		/// </summary>
		/// <param name="value">The line to be written.</param>
		public override void WriteLine(string value)
		{
			_console.WriteMessage(new ConsoleMessage(value)
			{
			});
		}

		/// <summary>
		/// Writes a verbose message to the output display of the host.
		/// </summary>
		/// <param name="message">The verbose message that is displayed.</param>
		public override void WriteVerboseLine(string message)
		{
			_console.WriteMessage(new ConsoleMessage(message)
			{
				Level = MessageLevel.Verbose
			});
		}

		/// <summary>
		/// Writes a warning message to the output display of the host.
		/// </summary>
		/// <param name="message">The warning message that is displayed.</param>
		public override void WriteWarningLine(string message)
		{
			_console.WriteMessage(new ConsoleMessage(message)
			{
				Level = MessageLevel.Warning
			});
		}

		/// <summary>
		/// Writes a progress report to the output display of the host.
		/// </summary>
		/// <param name="sourceId">Unique identifier of the source of the record. </param>
		/// <param name="record">A ProgressReport object.</param>
		public override void WriteProgress(long sourceId, ProgressRecord record)
		{
		}



		//	/// <summary>
		///// This is a private worker function splits out the
		///// accelerator keys from the menu and builds a two
		///// dimentional array with the first access containing the
		///// accelerator and the second containing the label string
		///// with the &amp; removed.
		///// </summary>
		///// <param name="choices">The choice collection to process</param>
		///// <returns>
		///// A two dimensional array containing the accelerator characters
		///// and the cleaned-up labels</returns>
		//private static string[,] BuildHotkeysAndPlainLabels(
		//		Collection<ChoiceDescription> choices)
		//{
		//	// Allocate the result array.
		//	string[,] hotkeysAndPlainLabels = new string[2, choices.Count];
		//	for (int i = 0; i < choices.Count; ++i)
		//	{
		//		string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
		//		hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
		//		hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
		//	}

		//	return hotkeysAndPlainLabels;
		//}

    /// <summary>
    /// Parse a string containing a hotkey character.
    /// Take a string of the form
    ///    Yes to &amp;all
    /// and returns a two-dimensional array split out as
    ///    "A", "Yes to all".
    /// </summary>
    /// <param name="input">The string to process</param>
    /// <returns>
    /// A two dimensional array containing the parsed components.
    /// </returns>
    private static string[] GetHotkeyAndLabel(string input)
    {
      string[] result = new string[] { String.Empty, String.Empty };
      string[] fragments = input.Split('&');
      if (fragments.Length == 2)
      {
        if (fragments[1].Length > 0)
        {
          result[0] = fragments[1][0].ToString().
          ToUpper(CultureInfo.CurrentCulture);
        }

        result[1] = (fragments[0] + fragments[1]).Trim();
      }
      else
      {
        result[1] = input;
      }

      return result;
    }
	}
}
