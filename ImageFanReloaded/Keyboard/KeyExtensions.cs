namespace ImageFanReloaded.Keyboard;

public static class KeyExtensions
{
	public static ImageFanReloaded.Core.Keyboard.Key ToCoreKey(this Avalonia.Input.Key key)
		=> key switch
		{
			Avalonia.Input.Key.Tab => ImageFanReloaded.Core.Keyboard.Key.Tab,
			Avalonia.Input.Key.Escape => ImageFanReloaded.Core.Keyboard.Key.Escape,
			Avalonia.Input.Key.Enter => ImageFanReloaded.Core.Keyboard.Key.Enter,

			Avalonia.Input.Key.W => ImageFanReloaded.Core.Keyboard.Key.W,
			Avalonia.Input.Key.A => ImageFanReloaded.Core.Keyboard.Key.A,
			Avalonia.Input.Key.Up => ImageFanReloaded.Core.Keyboard.Key.Up,
			Avalonia.Input.Key.Left => ImageFanReloaded.Core.Keyboard.Key.Left,
			Avalonia.Input.Key.Back => ImageFanReloaded.Core.Keyboard.Key.Backspace,
			Avalonia.Input.Key.PageUp => ImageFanReloaded.Core.Keyboard.Key.PageUp,

			Avalonia.Input.Key.S => ImageFanReloaded.Core.Keyboard.Key.S,
			Avalonia.Input.Key.D => ImageFanReloaded.Core.Keyboard.Key.D,
			Avalonia.Input.Key.Down => ImageFanReloaded.Core.Keyboard.Key.Down,
			Avalonia.Input.Key.Right => ImageFanReloaded.Core.Keyboard.Key.Right,
			Avalonia.Input.Key.Space => ImageFanReloaded.Core.Keyboard.Key.Space,
			Avalonia.Input.Key.PageDown => ImageFanReloaded.Core.Keyboard.Key.PageDown,

			_ => ImageFanReloaded.Core.Keyboard.Key.None
		};
}
