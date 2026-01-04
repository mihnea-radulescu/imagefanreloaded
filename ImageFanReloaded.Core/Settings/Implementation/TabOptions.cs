using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public FileSystemEntryInfoOrdering FolderOrdering { get; set; }
	public FileSystemEntryInfoOrderingDirection FolderOrderingDirection { get; set; }

	public FileSystemEntryInfoOrdering ImageFileOrdering { get; set; }
	public FileSystemEntryInfoOrderingDirection ImageFileOrderingDirection { get; set; }

	public ImageViewDisplayMode ImageViewDisplayMode { get; set; }

	public ThumbnailSize ThumbnailSize { get; set; }

	public bool RecursiveFolderBrowsing { get; set; }
	public bool GlobalOrderingForRecursiveFolderBrowsing { get; set; }

	public bool ShowImageViewImageInfo { get; set; }
	public int PanelsSplittingRatio { get; set; }
	public SlideshowInterval SlideshowInterval { get; set; }
	public bool ApplyImageOrientation { get; set; }
	public bool ShowThumbnailImageFileName { get; set; }
	public KeyboardScrollThumbnailIncrement KeyboardScrollThumbnailIncrement { get; set; }
	public UpsizeFullScreenImagesUpToScreenSize UpsizeFullScreenImagesUpToScreenSize { get; set; }

	static TabOptions()
	{
		AppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		TabOptionsJsonTypeInfo = TabOptionsJsonContext.Default.TabOptions;
	}

	public static void LoadDefaultTabOptions(string settingsFolderName)
	{
		_settingsFolderPath = GetSettingsFolderPath(settingsFolderName);
		_settingsFilePath = GetSettingsFilePath(_settingsFolderPath);

		var tabOptions = default(ITabOptions?);

		try
		{
			tabOptions = GetTabOptions();
		}
		catch
		{
		}
		finally
		{
			_defaultTabOptions = tabOptions ?? new TabOptions();
		}
	}

	public TabOptions()
	{
		InitializeWithDefaultValues();
	}

	public async Task SaveDefaultTabOptions()
	{
		SetDefaultTabOptions();

		await PersistDefaultTabOptions();
	}

	private const string SettingsFileName = "DefaultTabOptions.json";

	private const FileSystemEntryInfoOrdering DefaultFolderOrdering = FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection DefaultFolderOrderingDirection =
		FileSystemEntryInfoOrderingDirection.Ascending;

	private const FileSystemEntryInfoOrdering DefaultImageFileOrdering = FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection DefaultImageFileOrderingDirection =
		FileSystemEntryInfoOrderingDirection.Ascending;

	private const ImageViewDisplayMode DefaultImageViewDisplayMode = ImageViewDisplayMode.FullScreen;

	private const ThumbnailSize DefaultThumbnailSize = ThumbnailSize.TwoHundredFiftyPixels;

	private const bool DefaultRecursiveFolderBrowsing = false;
	private const bool DefaultGlobalOrderingForRecursiveFolderBrowsing = false;

	private const bool DefaultShowImageViewImageInfo = false;
	private const int DefaultPanelsSplittingRatio = 15;
	private const SlideshowInterval DefaultSlideshowInterval = SlideshowInterval.OneSecond;
	private const bool DefaultApplyImageOrientation = false;
	private const bool DefaultShowThumbnailImageFileName = true;
	private const KeyboardScrollThumbnailIncrement DefaultKeyboardScrollThumbnailIncrement =
		KeyboardScrollThumbnailIncrement.Twelve;
	private const UpsizeFullScreenImagesUpToScreenSize DefaultUpsizeFullScreenImagesUpToScreenSize =
		UpsizeFullScreenImagesUpToScreenSize.Disabled;

	private static readonly string AppDataFolderPath;

	private static readonly JsonTypeInfo<TabOptions> TabOptionsJsonTypeInfo;

	private static string? _settingsFolderPath;
	private static string? _settingsFilePath;

	private static ITabOptions? _defaultTabOptions;

	private static string GetSettingsFolderPath(string settingsFolderName)
		=> Path.Combine(AppDataFolderPath, settingsFolderName);

	private static string GetSettingsFilePath(string settingsFolderPath)
		=> Path.Combine(settingsFolderPath, SettingsFileName);

	private static ITabOptions? GetTabOptions()
	{
		var tabOptions = default(ITabOptions?);

		if (!File.Exists(_settingsFilePath))
		{
			return tabOptions;
		}

		var jsonContent = File.ReadAllText(_settingsFilePath);
		tabOptions = JsonSerializer.Deserialize(jsonContent, TabOptionsJsonTypeInfo);

		if (tabOptions is null)
		{
			return tabOptions;
		}

		if (!IsValidEnumValue(tabOptions.FolderOrdering))
		{
			tabOptions.FolderOrdering = DefaultFolderOrdering;
		}

		if (!IsValidEnumValue(tabOptions.FolderOrderingDirection))
		{
			tabOptions.FolderOrderingDirection = DefaultFolderOrderingDirection;
		}

		if (!IsValidEnumValue(tabOptions.ImageViewDisplayMode))
		{
			tabOptions.ImageViewDisplayMode = DefaultImageViewDisplayMode;
		}

		if (!IsValidEnumValue(tabOptions.ImageFileOrdering))
		{
			tabOptions.ImageFileOrdering = DefaultImageFileOrdering;
		}

		if (!IsValidEnumValue(tabOptions.ImageFileOrderingDirection))
		{
			tabOptions.ImageFileOrderingDirection = DefaultImageFileOrderingDirection;
		}

		if (!IsValidEnumValue(tabOptions.ThumbnailSize))
		{
			tabOptions.ThumbnailSize = DefaultThumbnailSize;
		}

		if (!IsValidPanelsSplittingRatio(tabOptions.PanelsSplittingRatio))
		{
			tabOptions.PanelsSplittingRatio = DefaultPanelsSplittingRatio;
		}

		if (!IsValidEnumValue(tabOptions.SlideshowInterval))
		{
			tabOptions.SlideshowInterval = DefaultSlideshowInterval;
		}

		if (!IsValidEnumValue(tabOptions.KeyboardScrollThumbnailIncrement))
		{
			tabOptions.KeyboardScrollThumbnailIncrement = DefaultKeyboardScrollThumbnailIncrement;
		}

		if (!IsValidEnumValue(tabOptions.UpsizeFullScreenImagesUpToScreenSize))
		{
			tabOptions.UpsizeFullScreenImagesUpToScreenSize = DefaultUpsizeFullScreenImagesUpToScreenSize;
		}

		return tabOptions;
	}

	private void InitializeWithDefaultValues()
	{
		if (_defaultTabOptions is not null)
		{
			FolderOrdering = _defaultTabOptions.FolderOrdering;
			FolderOrderingDirection = _defaultTabOptions.FolderOrderingDirection;

			ImageFileOrdering = _defaultTabOptions.ImageFileOrdering;
			ImageFileOrderingDirection = _defaultTabOptions.ImageFileOrderingDirection;

			ImageViewDisplayMode = _defaultTabOptions.ImageViewDisplayMode;

			ThumbnailSize = _defaultTabOptions.ThumbnailSize;

			RecursiveFolderBrowsing = _defaultTabOptions.RecursiveFolderBrowsing;
			GlobalOrderingForRecursiveFolderBrowsing = _defaultTabOptions.GlobalOrderingForRecursiveFolderBrowsing;

			ShowImageViewImageInfo = _defaultTabOptions.ShowImageViewImageInfo;
			PanelsSplittingRatio = _defaultTabOptions.PanelsSplittingRatio;
			SlideshowInterval = _defaultTabOptions.SlideshowInterval;
			ApplyImageOrientation = _defaultTabOptions.ApplyImageOrientation;
			ShowThumbnailImageFileName = _defaultTabOptions.ShowThumbnailImageFileName;
			KeyboardScrollThumbnailIncrement = _defaultTabOptions.KeyboardScrollThumbnailIncrement;
			UpsizeFullScreenImagesUpToScreenSize = _defaultTabOptions.UpsizeFullScreenImagesUpToScreenSize;
		}
		else
		{
			FolderOrdering = DefaultFolderOrdering;
			FolderOrderingDirection = DefaultFolderOrderingDirection;

			ImageFileOrdering = DefaultImageFileOrdering;
			ImageFileOrderingDirection = DefaultImageFileOrderingDirection;

			ImageViewDisplayMode = DefaultImageViewDisplayMode;

			ThumbnailSize = DefaultThumbnailSize;

			RecursiveFolderBrowsing = DefaultRecursiveFolderBrowsing;
			GlobalOrderingForRecursiveFolderBrowsing = DefaultGlobalOrderingForRecursiveFolderBrowsing;

			ShowImageViewImageInfo = DefaultShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultPanelsSplittingRatio;
			SlideshowInterval = DefaultSlideshowInterval;
			ApplyImageOrientation = DefaultApplyImageOrientation;
			ShowThumbnailImageFileName = DefaultShowThumbnailImageFileName;
			KeyboardScrollThumbnailIncrement = DefaultKeyboardScrollThumbnailIncrement;
			UpsizeFullScreenImagesUpToScreenSize = DefaultUpsizeFullScreenImagesUpToScreenSize;
		}
	}

	private void SetDefaultTabOptions()
	{
		_defaultTabOptions!.FolderOrdering = FolderOrdering;
		_defaultTabOptions!.FolderOrderingDirection = FolderOrderingDirection;

		_defaultTabOptions!.ImageFileOrdering = ImageFileOrdering;
		_defaultTabOptions!.ImageFileOrderingDirection = ImageFileOrderingDirection;

		_defaultTabOptions!.ImageViewDisplayMode = ImageViewDisplayMode;

		_defaultTabOptions!.ThumbnailSize = ThumbnailSize;

		_defaultTabOptions!.RecursiveFolderBrowsing = RecursiveFolderBrowsing;
		_defaultTabOptions!.GlobalOrderingForRecursiveFolderBrowsing = GlobalOrderingForRecursiveFolderBrowsing;

		_defaultTabOptions!.ShowImageViewImageInfo = ShowImageViewImageInfo;
		_defaultTabOptions!.PanelsSplittingRatio = PanelsSplittingRatio;
		_defaultTabOptions!.SlideshowInterval = SlideshowInterval;
		_defaultTabOptions!.ApplyImageOrientation = ApplyImageOrientation;
		_defaultTabOptions!.ShowThumbnailImageFileName = ShowThumbnailImageFileName;
		_defaultTabOptions!.KeyboardScrollThumbnailIncrement = KeyboardScrollThumbnailIncrement;
		_defaultTabOptions!.UpsizeFullScreenImagesUpToScreenSize = UpsizeFullScreenImagesUpToScreenSize;
	}

	private static async Task PersistDefaultTabOptions()
	{
		try
		{
			var jsonContent = JsonSerializer.Serialize(_defaultTabOptions!, TabOptionsJsonTypeInfo);

			if (!Directory.Exists(_settingsFolderPath))
			{
				Directory.CreateDirectory(_settingsFolderPath!);
			}

			await File.WriteAllTextAsync(_settingsFilePath!, jsonContent);
		}
		catch
		{
		}
	}

	private static bool IsValidEnumValue<TEnum>(TEnum enumValue) where TEnum : Enum
		=> Enum.IsDefined(typeof(TEnum), enumValue);

	private static bool IsValidPanelsSplittingRatio(int panelsSplittingRatio)
		=> panelsSplittingRatio is >= 0 and <= 100;
}

[JsonSerializable(typeof(TabOptions))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class TabOptionsJsonContext : JsonSerializerContext;
