using System;
using System.Drawing;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ImageFanReloaded.Controls.Extensions;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.RuntimeEnvironment;

namespace ImageFanReloaded.RuntimeEnvironment;

public class AccentColorChangePublisher : IAccentColorChangePublisher
{
	public AccentColorChangePublisher(Window window)
	{
		_accentColor = Color.Black;

		Task.Run(async () =>
		{
			while (true)
			{
				await Dispatcher.UIThread.InvokeAsync(
					() => RefreshAccentColor(window));

				await Task
					.Delay(RefreshAccentColorInterval).ConfigureAwait(false);
			}
		});
	}

	public Color GetAccentColor() => _accentColor;

	public event EventHandler<AccentColorChangedEventArgs>? AccentColorChanged;

	private static readonly TimeSpan RefreshAccentColorInterval =
		TimeSpan.FromSeconds(0.5);

	private Color _accentColor;

	private void RefreshAccentColor(Window window)
	{
		var newAccentColor = window.GetAccentColor();

		if (newAccentColor is not null && newAccentColor != _accentColor)
		{
			_accentColor = newAccentColor.Value;

			AccentColorChanged?.Invoke(
				this, new AccentColorChangedEventArgs(_accentColor));
		}
	}
}
