using Avalonia.Input;
using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Keyboard;

public static class KeyboardKeyExtensions
{
	public static Key GetKey(this IKeyboardKey keyboardKey)
		=> keyboardKey.GetInstance<Key>();
}
