//#define FLATPAK_BUILD

using System;
using System.IO;
using System.Text.Json;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	public ThumbnailSize ThumbnailSize { get; set; }
	public bool RecursiveFolderBrowsing { get; set; }
	public bool ShowImageViewImageInfo { get; set; }
	public int PanelsSplittingRatio { get; set; }

	static TabOptions()
	{
		SettingsFolderPath = GetSettingsFolderPath();
		SettingsFilePath = GetSettingsFilePath(SettingsFolderPath);

		JsonSerializerOptions = BuildJsonSerializerOptions();

		LoadDefaultTabOptions();
	}

	public TabOptions()
	{
		InitializeWithDefaultValues();
	}

	public void SaveDefaultTabOptions()
	{
		SetDefaultTabOptions();

		PersistDefaultTabOptions();
	}

	#region Private

	private const string SettingsFolderName = "ImageFanReloaded";
	private const string SettingsFileName = "DefaultTabOptions.json";

	private const FileSystemEntryInfoOrdering DefaultFileSystemEntryInfoOrdering =
		FileSystemEntryInfoOrdering.NameAscending;
	private const ThumbnailSize DefaultThumbnailSize = ThumbnailSize.TwoHundredAndFixtyPixels;
	private const bool DefaultRecursiveFolderBrowsing = false;
	private const bool DefaultShowImageViewImageInfo = false;
	private const int DefaultPanelsSplittingRatio = 15;

	private static readonly string SettingsFolderPath;
	private static readonly string SettingsFilePath;

	private static readonly JsonSerializerOptions JsonSerializerOptions;

	private static ITabOptions? DefaultTabOptions;

	private static string GetSettingsFolderPath()
	{
		var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

#if FLATPAK_BUILD
		var settingsFolderPath = appDataFolderPath;
#else
		var settingsFolderPath = Path.Combine(appDataFolderPath, SettingsFolderName);
#endif

		return settingsFolderPath;
	}

	private static string GetSettingsFilePath(string settingsFolderPath)
		=> Path.Combine(settingsFolderPath, SettingsFileName);

	private static void LoadDefaultTabOptions()
	{
		try
		{
			if (!File.Exists(SettingsFilePath))
			{
				return;
			}

			var jsonContent = File.ReadAllText(SettingsFilePath);
			var loadedTabOptions = JsonSerializer.Deserialize<TabOptions>(
				jsonContent, JsonSerializerOptions);

			if (loadedTabOptions is null)
			{
				return;
			}

			if (!IsValidEnumValue(loadedTabOptions.FileSystemEntryInfoOrdering))
			{
				loadedTabOptions.FileSystemEntryInfoOrdering = DefaultFileSystemEntryInfoOrdering;
			}

			if (!IsValidEnumValue(loadedTabOptions.ThumbnailSize))
			{
				loadedTabOptions.ThumbnailSize = DefaultThumbnailSize;
			}

			if (!IsValidPanelsSplittingRatio(loadedTabOptions.PanelsSplittingRatio))
			{
				loadedTabOptions.PanelsSplittingRatio = DefaultPanelsSplittingRatio;
			}

			DefaultTabOptions = loadedTabOptions;
		}
		catch
		{
		}
		finally
		{
			DefaultTabOptions ??= new TabOptions();
		}
	}

	private void InitializeWithDefaultValues()
	{
		if (DefaultTabOptions is not null)
		{
			FileSystemEntryInfoOrdering = DefaultTabOptions.FileSystemEntryInfoOrdering;
			ThumbnailSize = DefaultTabOptions.ThumbnailSize;
			RecursiveFolderBrowsing = DefaultTabOptions.RecursiveFolderBrowsing;
			ShowImageViewImageInfo = DefaultTabOptions.ShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultTabOptions.PanelsSplittingRatio;
		}
		else
		{
			FileSystemEntryInfoOrdering = DefaultFileSystemEntryInfoOrdering;
			ThumbnailSize = DefaultThumbnailSize;
			RecursiveFolderBrowsing = DefaultRecursiveFolderBrowsing;
			ShowImageViewImageInfo = DefaultShowImageViewImageInfo;
			PanelsSplittingRatio = DefaultPanelsSplittingRatio;
		}
	}

	private void SetDefaultTabOptions()
	{
		DefaultTabOptions!.FileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering;
		DefaultTabOptions!.ThumbnailSize = ThumbnailSize;
		DefaultTabOptions!.RecursiveFolderBrowsing = RecursiveFolderBrowsing;
		DefaultTabOptions!.ShowImageViewImageInfo = ShowImageViewImageInfo;
		DefaultTabOptions!.PanelsSplittingRatio = PanelsSplittingRatio;
	}

	private static void PersistDefaultTabOptions()
	{
		try
		{
			var jsonContent = JsonSerializer.Serialize(DefaultTabOptions!, JsonSerializerOptions);

			if (!Directory.Exists(SettingsFolderPath))
			{
				Directory.CreateDirectory(SettingsFolderPath);
			}

			File.WriteAllText(SettingsFilePath, jsonContent);
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
