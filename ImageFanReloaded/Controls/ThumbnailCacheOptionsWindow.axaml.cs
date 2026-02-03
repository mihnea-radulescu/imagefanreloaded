using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class ThumbnailCacheOptionsWindow
	: Window, IThumbnailCacheOptionsView
{
	public ThumbnailCacheOptionsWindow()
	{
		InitializeComponent();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}

	public IGlobalParameters? GlobalParameters { get; set; }
	public IThumbnailCacheOptions? ThumbnailCacheOptions { get; set; }

	public int? ThumbnailCacheSizeInMegabytes
	{
		get => field;
		set
		{
			field = value;

			_hasInProgressOperation = false;
			EnableControls(!_hasInProgressOperation);

			_thumbnailCacheSizeInMegabytesTextBlock.Text = $"{field} MB";
		}
	}

	public event EventHandler<EnableThumbnailCachingEventArgs>?
		EnableThumbnailCachingChanged;
	public event EventHandler<ClearThumbnailCacheEventArgs>?
		ClearThumbnailCacheSelected;

	public void PopulateThumbnailCacheOptions()
	{
		SetEnableThumbnailCaching();

		RegisterThumbnailCacheOptionEvents();
	}

	public async Task ShowDialog(IMainView owner)
		=> await ShowDialog((Window)owner);

	private bool _hasInProgressOperation;

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();

			e.Handled = true;
		}
	}

	private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		if (_hasInProgressOperation)
		{
			e.Cancel = true;
		}
		else
		{
			UnregisterThumbnailCacheOptionEvents();
		}
	}

	private void OnEnableThumbnailCachingIsCheckedChanged(
		object? sender, RoutedEventArgs e)
	{
		var enableThumbnailCaching =
			_enableThumbnailCachingCheckBox.IsChecked!.Value;

		ThumbnailCacheOptions!.EnableThumbnailCaching = enableThumbnailCaching;

		EnableThumbnailCachingChanged?.Invoke(
			this, new EnableThumbnailCachingEventArgs(ThumbnailCacheOptions!));
	}

	private void OnClearThumbnailCacheButtonClicked(
		object? sender, RoutedEventArgs e)
	{
		_hasInProgressOperation = true;
		EnableControls(!_hasInProgressOperation);

		ClearThumbnailCacheSelected?.Invoke(
			this, new ClearThumbnailCacheEventArgs(this));
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private void SetEnableThumbnailCaching()
	{
		_enableThumbnailCachingCheckBox.IsChecked =
			ThumbnailCacheOptions!.EnableThumbnailCaching;
	}

	private void RegisterThumbnailCacheOptionEvents()
	{
		_enableThumbnailCachingCheckBox.IsCheckedChanged +=
			OnEnableThumbnailCachingIsCheckedChanged;

		_clearThumbnailCacheButton.Click +=
			OnClearThumbnailCacheButtonClicked;
	}

	private void UnregisterThumbnailCacheOptionEvents()
	{
		_enableThumbnailCachingCheckBox.IsCheckedChanged -=
			OnEnableThumbnailCachingIsCheckedChanged;

		_clearThumbnailCacheButton.Click -=
			OnClearThumbnailCacheButtonClicked;
	}

	private void EnableControls(bool isEnabled)
	{
		_enableThumbnailCachingCheckBox.IsEnabled = isEnabled;
		_clearThumbnailCacheButton.IsEnabled = isEnabled;
	}
}
