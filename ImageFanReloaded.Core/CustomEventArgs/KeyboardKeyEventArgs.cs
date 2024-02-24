using System;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class KeyboardKeyEventArgs : EventArgs
{
	public KeyboardKeyEventArgs(Key key)
	{
		Key = key;
	}

	public Key Key { get; }
}
