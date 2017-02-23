
using System.Security;

namespace d3ploy.PSRunner
{
	public interface ID3PloyConsole
	{
		void WriteMessage(ConsoleMessage message);
		string ReadLine();
		SecureString ReadLineAsSecureString();
	}

	public class ConsoleMessage
	{
		public ConsoleMessage(string value)
		{
			Value = value;
			Mode = MessageMode.WriteLine;
			Level = MessageLevel.Default;
		}

		public string Value { get; private set; }
		public MessageMode Mode { get; set; }
		public MessageLevel Level { get; set; }
		public string BackgroundColor { get; set; }
		public string ForegroundColor { get; set; }
	}

	public enum MessageMode
	{
		Write,
		WriteLine
	}

	public enum MessageLevel
	{
		Default,
		Verbose,
		Debug,
		Warning,
		Error
	}
}
