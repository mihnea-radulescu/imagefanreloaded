//#define FLATPAK_BUILD

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	public int ThumbnailSize { get; set; }
	public bool RecursiveFolderBrowsing { get; set; }
	public bool ShowImageViewImageInfo { get; set; }

	static TabOptions()
	{
		SettingsFolderPath = GetSettingsFolderPath();
		SettingsFilePath = GetSettingsFilePath(SettingsFolderPath);

		JsonSerializerOptions = BuildJsonSerializerOptions();

		ValidThumbnailSizes = BuildValidThumbnailSizes();

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

	private const int ThumbnailSizeLowerThreshold = 100;
	private const int ThumbnailSizeUpperThreshold = 400;
	private const int ThumbnailSizeIncrement = 50;

	private static readonly string SettingsFolderPath;
	private static readonly string SettingsFilePath;

	private static readonly JsonSerializerOptions JsonSerializerOptions;

	private static readonly HashSet<int> ValidThumbnailSizes;

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

			if (!IsValidFileSystemEntryInfoOrdering(loadedTabOptions.FileSystemEntryInfoOrdering))
			{
				return;
			}

			if (!IsValidThumbnailSize(loadedTabOptions.ThumbnailSize))
			{
				return;
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
		}
		else
		{
			FileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering.NameAscending;
			ThumbnailSize = 250;
			RecursiveFolderBrowsing = false;
			ShowImageViewImageInfo = false;
		}
	}

	private void SetDefaultTabOptions()
	{
		DefaultTabOptions!.FileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering;
		DefaultTabOptions!.ThumbnailSize = ThumbnailSize;
		DefaultTabOptions!.RecursiveFolderBrowsing = RecursiveFolderBrowsing;
		DefaultTabOptions!.ShowImageViewImageInfo = ShowImageViewImageInfo;
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

	private static HashSet<int> BuildValidThumbnailSizes()
	{
		var validThumbnailSizes = new HashSet<int>();

		for (var thumbnailSize = ThumbnailSizeLowerThreshold;
			 thumbnailSize <= ThumbnailSizeUpperThreshold;
			 thumbnailSize += ThumbnailSizeIncrement)
		{
			validThumbnailSizes.Add(thumbnailSize);
		}

		return validThumbnailSizes;
	}

	private static bool IsValidFileSystemEntryInfoOrdering(FileSystemEntryInfoOrdering ordering)
		=> Enum.IsDefined(typeof(FileSystemEntryInfoOrdering), ordering);
	
	private static bool IsValidThumbnailSize(int thumbnailSize)
		=> ValidThumbnailSizes.Contains(thumbnailSize);

	#endregion
}
