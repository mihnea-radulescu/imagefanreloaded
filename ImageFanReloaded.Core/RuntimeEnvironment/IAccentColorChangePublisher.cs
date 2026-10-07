using System;
using System.Drawing;
using ImageFanReloaded.Core.CustomEventArgs;

namespace ImageFanReloaded.Core.RuntimeEnvironment;

public interface IAccentColorChangePublisher
{
	Color GetAccentColor();

	event EventHandler<AccentColorChangedEventArgs>? AccentColorChanged;
}
