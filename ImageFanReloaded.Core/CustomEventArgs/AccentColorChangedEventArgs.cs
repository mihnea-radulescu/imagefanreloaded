using System;
using System.Drawing;

namespace ImageFanReloaded.Core.CustomEventArgs;

public class AccentColorChangedEventArgs : EventArgs
{
	public AccentColorChangedEventArgs(Color accentColor)
	{
		AccentColor = accentColor;
	}

	public Color AccentColor { get; }
}
