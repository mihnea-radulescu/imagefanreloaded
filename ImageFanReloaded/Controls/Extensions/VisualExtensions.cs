using System.Drawing;
using Avalonia;
using Avalonia.VisualTree;

namespace ImageFanReloaded.Controls.Extensions;

public static class VisualExtensions
{
	extension(Visual element)
	{
		public Color? GetAccentColor()
		{
			var platformSettings = element.GetPlatformSettings();

			if (platformSettings is null)
			{
				return null;
			}

			var colorValues = platformSettings.GetColorValues();

			var avaloniaAccentColor = colorValues.AccentColor1;
			var systemAccentColor = avaloniaAccentColor.ToSystemColor();

			return systemAccentColor;
		}
	}
}
