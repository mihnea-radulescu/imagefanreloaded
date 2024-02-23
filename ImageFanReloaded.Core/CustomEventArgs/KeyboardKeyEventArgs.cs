using System;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class KeyboardKeyEventArgs : EventArgs
{
	public KeyboardKeyEventArgs(IKeyboardKey keyboardKey)
	{
		KeyboardKey = keyboardKey;
	}

	public IKeyboardKey KeyboardKey { get; }
}
