using System;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class KeyboardKeyEventArgs : EventArgs
{
	public KeyboardKeyEventArgs(KeyModifiers keyModifiers, Key key)
	{
		KeyModifiers = keyModifiers;
		Key = key;
	}

	public KeyModifiers KeyModifiers { get; }
	public Key Key { get; }
}
