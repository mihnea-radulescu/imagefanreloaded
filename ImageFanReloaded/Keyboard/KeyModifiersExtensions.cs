namespace ImageFanReloaded.Keyboard;

public static class KeyModifiersExtensions
{
	public static ImageFanReloaded.Core.Keyboard.KeyModifiers ToCoreKeyModifiers(
		this Avalonia.Input.KeyModifiers keyModifiers)
		=> keyModifiers switch
		{
			Avalonia.Input.KeyModifiers.Control => Core.Keyboard.KeyModifiers.Ctrl,
			Avalonia.Input.KeyModifiers.Alt => Core.Keyboard.KeyModifiers.Alt,
			Avalonia.Input.KeyModifiers.Shift => Core.Keyboard.KeyModifiers.Shift,

			_ => Core.Keyboard.KeyModifiers.None
		};
}
