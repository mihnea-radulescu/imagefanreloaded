using ImageFanReloaded.Core.Keyboard;

namespace ImageFanReloaded.Keyboard;

public static class KeyModifiersExtensions
{
	public static ImageFanReloaded.Core.Keyboard.KeyModifiers ToCoreKeyModifiers(
		this Avalonia.Input.KeyModifiers keyModifiers)
		=> keyModifiers switch
		{
			Avalonia.Input.KeyModifiers.Alt => Core.Keyboard.KeyModifiers.Alt,
			Avalonia.Input.KeyModifiers.Control => KeyModifiers.Ctrl,
			
			_ => Core.Keyboard.KeyModifiers.None
		};
}
