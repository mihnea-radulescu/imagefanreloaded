using Avalonia;
using Avalonia.Media;
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
			var accentColor = colorValues.AccentColor1;

			return accentColor;
		}
	}
}
