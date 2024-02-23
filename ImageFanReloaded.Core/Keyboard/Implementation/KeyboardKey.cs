using System;

namespace ImageFanReloaded.Core.Keyboard.Implementation;

public class KeyboardKey : IKeyboardKey
{
	public KeyboardKey(Enum keyboardKey)
	{
		_keyboardKey = keyboardKey;
	}

	public TKeyboardKey GetInstance<TKeyboardKey>() where TKeyboardKey : Enum
		=> (TKeyboardKey)_keyboardKey;
	
	#region Private

	private readonly Enum _keyboardKey;

	#endregion
}
