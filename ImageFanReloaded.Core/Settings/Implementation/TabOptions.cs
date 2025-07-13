using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	public FileSystemEntryInfoOrderingDirection FileSystemEntryInfoOrderingDirection { get; set; }
	public ThumbnailSize ThumbnailSize { get; set; }
	public bool RecursiveFolderBrowsing { get; set; }
	public bool ShowImageViewImageInfo { get; set; }
	public int PanelsSplittingRatio { get; set; }
	public SlideshowInterval SlideshowInterval { get; set; }
	public bool ApplyImageOrientation { get; set; }

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

	private const FileSystemEntryInfoOrdering DefaultFileSystemEntryInfoOrdering =
		FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection DefaultFileSystemEntryInfoOrderingDirection =
		FileSystemEntryInfoOrderingDirection.Ascending;
	private const ThumbnailSize DefaultThumbnailSize = ThumbnailSize.TwoHundredAndFixtyPixels;
	private const bool DefaultRecursiveFolderBrowsing = false;
	private const bool DefaultShowImageViewImageInfo = false;
	private const int DefaultPanelsSplittingRatio = 15;
	private const SlideshowInterval DefaultSlideshowInterval = SlideshowInterval.OneSecond;
	private const bool DefaultApplyImageOrientation = false;

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

		if (!IsValidEnumValue(tabOptions.FileSystemEntryInfoOrdering))
		{
			tabOptions.FileSystemEntryInfoOrdering = DefaultFileSystemEntryInfoOrdering;
		}

		if (!IsValidEnumValue(tabOptions.FileSystemEntryInfoOrderingDirection))
		{
			tabOptions.FileSystemEntryInfoOrderingDirection =
				DefaultFileSystemEntryInfoOrderingDirection;
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

		return tabOptions;
	}

	private void InitializeWithDefaultValues()
	{
		if (DefaultTabOptions is not null)
		{
			FileSystemEntryInfoOrdering = DefaultTabOptions.FileSystemEntryInfoOrdering;
			FileSystemEntryInfoOrderingDirection =
				DefaultTabOptions.FileSystemEntryInfoOrderingDirection;
			ThumbnailSize = DefaultTabOptions.ThumbnailSize;
			RecursiveFolderBrowsing = DefaultTabOptions.RecursiveFolderBrowsing;
			ShowImageViewImageInfo = DefaultTabOptions.ShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultTabOptions.PanelsSplittingRatio;
			SlideshowInterval = DefaultTabOptions.SlideshowInterval;
			ApplyImageOrientation = DefaultTabOptions.ApplyImageOrientation;
		}
		else
		{
			FileSystemEntryInfoOrdering = DefaultFileSystemEntryInfoOrdering;
			FileSystemEntryInfoOrderingDirection = DefaultFileSystemEntryInfoOrderingDirection;
			ThumbnailSize = DefaultThumbnailSize;
			RecursiveFolderBrowsing = DefaultRecursiveFolderBrowsing;
			ShowImageViewImageInfo = DefaultShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultPanelsSplittingRatio;
			SlideshowInterval = DefaultSlideshowInterval;
			ApplyImageOrientation = DefaultApplyImageOrientation;
		}
	}

	private void SetDefaultTabOptions()
	{
		DefaultTabOptions!.FileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering;
		DefaultTabOptions!.FileSystemEntryInfoOrderingDirection =
			FileSystemEntryInfoOrderingDirection;
		DefaultTabOptions!.ThumbnailSize = ThumbnailSize;
		DefaultTabOptions!.RecursiveFolderBrowsing = RecursiveFolderBrowsing;
		DefaultTabOptions!.ShowImageViewImageInfo = ShowImageViewImageInfo;
		DefaultTabOptions!.PanelsSplittingRatio = PanelsSplittingRatio;
		DefaultTabOptions!.SlideshowInterval = SlideshowInterval;
		DefaultTabOptions!.ApplyImageOrientation = ApplyImageOrientation;
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
