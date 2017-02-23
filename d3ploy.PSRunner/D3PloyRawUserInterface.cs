using System;
using System.Management.Automation.Host;

namespace d3ploy.PSRunner
{
	internal class D3PloyRawUserInterface : PSHostRawUserInterface
  {
		private readonly Size _bufferSize = new Size(100, 200);
    /// <summary>
    /// Gets or sets the host buffer size adapted from the Console buffer 
    /// size members.
    /// </summary>
    public override Size BufferSize
    {
			get { return _bufferSize; }
			set
			{
				throw new NotImplementedException("Set BufferSize not implemented");
			}
    }

    /// <summary>
    /// Gets or sets the cursor position. In this example this 
    /// functionality is not needed so the property throws a 
    /// NotImplementException exception.
    /// </summary>
    public override Coordinates CursorPosition
    {
			get { return new Coordinates(0, 0); }
			set
			{
				throw new NotImplementedException("Set CursorPosition not implemented");
			}
    }

    /// <summary>
    /// Gets or sets the cursor size taken directly from the 
    /// Console.CursorSize property.
    /// </summary>
    public override int CursorSize
    {
			get { return 1; }
			set
			{
				throw new NotImplementedException("Set CursorSize not implemented");
			}
    }

    /// <summary>
    /// Gets or sets the foreground color of the text to be written.
    /// This maps to the corresponding Console.ForgroundColor property.
    /// </summary>
    public override ConsoleColor ForegroundColor
    {
			get { return ConsoleColor.Black; }
			set
			{
				throw new NotImplementedException("Set ForegroundColor not implemented");
			}
    }

		/// <summary>
		/// Gets or sets the background color of text to be written.
		/// This maps to the corresponding Console.Background property.
		/// </summary>
		public override ConsoleColor BackgroundColor
		{
			get { return ConsoleColor.White; }
			set
			{
				throw new NotImplementedException("Set BackgroundColor not implemented");
			}
		}

    /// <summary>
    /// Gets a value indicating whether a key is available. This maps to  
    /// the corresponding Console.KeyAvailable property.
    /// </summary>
    public override bool KeyAvailable
    {
			get
			{
				throw new NotImplementedException("Get KeyAvailable not implemented");
			}
    }

    /// <summary>
    /// Gets the maximum physical size of the window adapted from the  
    ///  Console.LargestWindowWidth and Console.LargestWindowHeight 
    ///  properties.
    /// </summary>
    public override Size MaxPhysicalWindowSize
    {
			get { return _bufferSize; }
    }

    /// <summary>
    /// Gets the maximum window size adapted from the 
    /// Console.LargestWindowWidth and console.LargestWindowHeight 
    /// properties.
    /// </summary>
    public override Size MaxWindowSize
    {
			get { return _bufferSize; }
    }

    /// <summary>
    /// Gets or sets the window position adapted from the Console window position 
    /// members.
    /// </summary>
    public override Coordinates WindowPosition
    {
			get { return new Coordinates(0, 0); }
			set
			{
				throw new NotImplementedException("Set WindowPosition not implemented");
			}
    }

    /// <summary>
    /// Gets or sets the window size adapted from the corresponding Console 
    /// calls.
    /// </summary>
    public override Size WindowSize
    {
			get { return _bufferSize; }
			set
			{
				throw new NotImplementedException("Set WindowSize not implemented");
			}
    }

    /// <summary>
    /// Gets or sets the title of the window mapped to the Console.Title 
    /// property.
    /// </summary>
    public override string WindowTitle
    {
			get { return "d3ploy"; }
			set
			{
				throw new NotImplementedException("Set WindowTitle not implemented");
			}
    }
        
    /// <summary>
    /// This API resets the input buffer. In this example this 
    /// functionality is not needed so the method returns nothing.
    /// </summary>
    public override void FlushInputBuffer()
    {
			throw new NotImplementedException("FlushInputBuffer not implemented");
    }

    /// <summary>
    /// This API returns a rectangular region of the screen buffer. In 
    /// this example this functionality is not needed so the method throws 
    /// a NotImplementException exception.
    /// </summary>
    /// <param name="rectangle">Defines the size of the rectangle.</param>
    /// <returns>Throws a NotImplementedException exception.</returns>
    public override BufferCell[,] GetBufferContents(Rectangle rectangle)
    {
			throw new NotImplementedException("GetBufferContents not implemented");
		}

    /// <summary>
    /// This API Reads a pressed, released, or pressed and released keystroke 
    /// from the keyboard device, blocking processing until a keystroke is 
    /// typed that matches the specified keystroke options. In this example 
    /// this functionality is not needed so the method throws a
    /// NotImplementException exception.
    /// </summary>
    /// <param name="options">Options, such as IncludeKeyDown,  used when 
    /// reading the keyboard.</param>
    /// <returns>Throws a NotImplementedException exception.</returns>
    public override KeyInfo ReadKey(ReadKeyOptions options)
    {
			throw new NotImplementedException("ReadKey not implemented");
		}

    /// <summary>
    /// This API crops a region of the screen buffer. In this example 
    /// this functionality is not needed so the method throws a
    /// NotImplementException exception.
    /// </summary>
    /// <param name="source">The region of the screen to be scrolled.</param>
    /// <param name="destination">The region of the screen to receive the 
    /// source region contents.</param>
    /// <param name="clip">The region of the screen to include in the operation.</param>
    /// <param name="fill">The character and attributes to be used to fill all cell.</param>
    public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
    {
			throw new NotImplementedException("ScrollBufferContents not implemented");
		}

     /// <summary>
    /// This API copies an array of buffer cells into the screen buffer 
    /// at a specified location. In this example this  functionality is 
    /// not needed si the method  throws a NotImplementedException exception.
    /// </summary>
    /// <param name="origin">The parameter is not used.</param>
    /// <param name="contents">The parameter is not used.</param>
    public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
    {
			throw new NotImplementedException("SetBufferContents not implemented");
    }

    /// <summary>
    /// This API Copies a given character, foreground color, and background 
    /// color to a region of the screen buffer. In this example this 
    /// functionality is not needed so the method throws a
    /// NotImplementException exception./// </summary>
    /// <param name="rectangle">Defines the area to be filled. </param>
    /// <param name="fill">Defines the fill character.</param>
    public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
    {
			throw new NotImplementedException("SetBufferContents not implemented");
		}
  }}
