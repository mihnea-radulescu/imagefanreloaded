namespace ImageFanReloaded.Keyboard;

public static class KeyModifiersExtensions
{
	extension(Avalonia.Input.KeyModifiers keyModifiers)
	{
		public ImageFanReloaded.Core.Keyboard.KeyModifiers ToCoreKeyModifiers()
			=> keyModifiers switch
			{
				Avalonia.Input.KeyModifiers.Control
					=> Core.Keyboard.KeyModifiers.Ctrl,
				Avalonia.Input.KeyModifiers.Alt
					=> Core.Keyboard.KeyModifiers.Alt,
				Avalonia.Input.KeyModifiers.Shift
					=> Core.Keyboard.KeyModifiers.Shift,

				_ => Core.Keyboard.KeyModifiers.None
			};
	}
}
