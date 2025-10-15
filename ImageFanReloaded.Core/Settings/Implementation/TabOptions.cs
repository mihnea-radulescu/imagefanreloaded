using System;
using System.IO;
using System.Text.Json;
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

	static TabOptions()
	{
		AppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

		JsonSerializerOptions = BuildJsonSerializerOptions();
	}

	public static void LoadDefaultTabOptions(string settingsFolderName)
	{
		SettingsFolderPath = GetSettingsFolderPath(settingsFolderName);
		SettingsFilePath = GetSettingsFilePath(SettingsFolderPath);

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
			DefaultTabOptions = tabOptions ?? new TabOptions();
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

	#region Private

	private const string SettingsFileName = "DefaultTabOptions.json";

	private const FileSystemEntryInfoOrdering DefaultFolderOrdering =
		FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection DefaultFolderOrderingDirection =
		FileSystemEntryInfoOrderingDirection.Ascending;

	private const FileSystemEntryInfoOrdering DefaultImageFileOrdering =
		FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection DefaultImageFileOrderingDirection =
		FileSystemEntryInfoOrderingDirection.Ascending;

	private const ImageViewDisplayMode DefaultImageViewDisplayMode =
		ImageViewDisplayMode.FullScreen;

	private const ThumbnailSize DefaultThumbnailSize = ThumbnailSize.TwoHundredFixtyPixels;

	private const bool DefaultRecursiveFolderBrowsing = false;
	private const bool DefaultGlobalOrderingForRecursiveFolderBrowsing = false;

	private const bool DefaultShowImageViewImageInfo = false;
	private const int DefaultPanelsSplittingRatio = 15;
	private const SlideshowInterval DefaultSlideshowInterval = SlideshowInterval.OneSecond;
	private const bool DefaultApplyImageOrientation = false;
	private const bool DefaultShowThumbnailImageFileName = true;
	private const KeyboardScrollThumbnailIncrement DefaultKeyboardScrollThumbnailIncrement =
		KeyboardScrollThumbnailIncrement.Twelve;

	private static readonly string AppDataFolderPath;

	private static readonly JsonSerializerOptions JsonSerializerOptions;

	private static string? SettingsFolderPath;
	private static string? SettingsFilePath;

	private static ITabOptions? DefaultTabOptions;

	private static string GetSettingsFolderPath(string settingsFolderName)
		=> Path.Combine(AppDataFolderPath, settingsFolderName);

	private static string GetSettingsFilePath(string settingsFolderPath)
		=> Path.Combine(settingsFolderPath, SettingsFileName);

	private static ITabOptions? GetTabOptions()
	{
		var tabOptions = default(ITabOptions?);

		if (!File.Exists(SettingsFilePath))
		{
			return tabOptions;
		}

		var jsonContent = File.ReadAllText(SettingsFilePath);
		tabOptions = JsonSerializer.Deserialize<TabOptions>(
			jsonContent, JsonSerializerOptions);

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

		return tabOptions;
	}

	private void InitializeWithDefaultValues()
	{
		if (DefaultTabOptions is not null)
		{
			FolderOrdering = DefaultTabOptions.FolderOrdering;
			FolderOrderingDirection = DefaultTabOptions.FolderOrderingDirection;

			ImageFileOrdering = DefaultTabOptions.ImageFileOrdering;
			ImageFileOrderingDirection = DefaultTabOptions.ImageFileOrderingDirection;

			ImageViewDisplayMode = DefaultTabOptions.ImageViewDisplayMode;

			ThumbnailSize = DefaultTabOptions.ThumbnailSize;

			RecursiveFolderBrowsing = DefaultTabOptions.RecursiveFolderBrowsing;
			GlobalOrderingForRecursiveFolderBrowsing =
				DefaultTabOptions.GlobalOrderingForRecursiveFolderBrowsing;

			ShowImageViewImageInfo = DefaultTabOptions.ShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultTabOptions.PanelsSplittingRatio;
			SlideshowInterval = DefaultTabOptions.SlideshowInterval;
			ApplyImageOrientation = DefaultTabOptions.ApplyImageOrientation;
			ShowThumbnailImageFileName = DefaultTabOptions.ShowThumbnailImageFileName;
			KeyboardScrollThumbnailIncrement = DefaultTabOptions.KeyboardScrollThumbnailIncrement;
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
			GlobalOrderingForRecursiveFolderBrowsing =
				DefaultGlobalOrderingForRecursiveFolderBrowsing;

			ShowImageViewImageInfo = DefaultShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultPanelsSplittingRatio;
			SlideshowInterval = DefaultSlideshowInterval;
			ApplyImageOrientation = DefaultApplyImageOrientation;
			ShowThumbnailImageFileName = DefaultShowThumbnailImageFileName;
			KeyboardScrollThumbnailIncrement = DefaultKeyboardScrollThumbnailIncrement;
		}
	}

	private void SetDefaultTabOptions()
	{
		DefaultTabOptions!.FolderOrdering = FolderOrdering;
		DefaultTabOptions!.FolderOrderingDirection = FolderOrderingDirection;

		DefaultTabOptions!.ImageFileOrdering = ImageFileOrdering;
		DefaultTabOptions!.ImageFileOrderingDirection = ImageFileOrderingDirection;

		DefaultTabOptions!.ImageViewDisplayMode = ImageViewDisplayMode;

		DefaultTabOptions!.ThumbnailSize = ThumbnailSize;

		DefaultTabOptions!.RecursiveFolderBrowsing = RecursiveFolderBrowsing;
		DefaultTabOptions!.GlobalOrderingForRecursiveFolderBrowsing =
			GlobalOrderingForRecursiveFolderBrowsing;

		DefaultTabOptions!.ShowImageViewImageInfo = ShowImageViewImageInfo;
		DefaultTabOptions!.PanelsSplittingRatio = PanelsSplittingRatio;
		DefaultTabOptions!.SlideshowInterval = SlideshowInterval;
		DefaultTabOptions!.ApplyImageOrientation = ApplyImageOrientation;
		DefaultTabOptions!.ShowThumbnailImageFileName = ShowThumbnailImageFileName;
		DefaultTabOptions!.KeyboardScrollThumbnailIncrement = KeyboardScrollThumbnailIncrement;
	}

	private static async Task PersistDefaultTabOptions()
	{
		try
		{
			var jsonContent = JsonSerializer.Serialize(DefaultTabOptions!, JsonSerializerOptions);

			if (!Directory.Exists(SettingsFolderPath))
			{
				Directory.CreateDirectory(SettingsFolderPath!);
			}

			await File.WriteAllTextAsync(SettingsFilePath!, jsonContent);
		}
		catch
		{
		}
	}

	private static JsonSerializerOptions BuildJsonSerializerOptions()
	{
		return new JsonSerializerOptions
		{
			WriteIndented = true
		};
	}

	private static bool IsValidEnumValue<TEnum>(TEnum enumValue) where TEnum : Enum
		=> Enum.IsDefined(typeof(TEnum), enumValue);

	private static bool IsValidPanelsSplittingRatio(int panelsSplittingRatio)
		=> 0 <= panelsSplittingRatio && panelsSplittingRatio <= 100;

	#endregion
}
