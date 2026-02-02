using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class TabOptions : ITabOptions
{
	public TabOptions(string configFolderPath)
	{
		_configFolderPath = configFolderPath;
		_tabOptionsConfigFilePath = Path.Combine(
			configFolderPath, TabOptionsConfigFileName);

		_tabOptionsDto = GetTabOptionsDto();
		_defaultTabOptions = new TabOptions(this);
	}

	public TabOptions(TabOptions defaultTabOptions)
	{
		_defaultTabOptions = defaultTabOptions;
		_configFolderPath = defaultTabOptions._configFolderPath;
		_tabOptionsConfigFilePath = defaultTabOptions._tabOptionsConfigFilePath;

		_tabOptionsDto = new TabOptionsDto();
		CopyFromDefaultTabOptions();
	}

	public FileSystemEntryInfoOrdering FolderOrdering
	{
		get => _tabOptionsDto.FolderOrdering;
		set => _tabOptionsDto.FolderOrdering = value;
	}

	public FileSystemEntryInfoOrderingDirection FolderOrderingDirection
	{
		get => _tabOptionsDto.FolderOrderingDirection;
		set => _tabOptionsDto.FolderOrderingDirection = value;
	}

	public FileSystemEntryInfoOrdering ImageFileOrdering
	{
		get => _tabOptionsDto.ImageFileOrdering;
		set => _tabOptionsDto.ImageFileOrdering = value;
	}

	public FileSystemEntryInfoOrderingDirection ImageFileOrderingDirection
	{
		get => _tabOptionsDto.ImageFileOrderingDirection;
		set => _tabOptionsDto.ImageFileOrderingDirection = value;
	}

	public ImageViewDisplayMode ImageViewDisplayMode
	{
		get => _tabOptionsDto.ImageViewDisplayMode;
		set => _tabOptionsDto.ImageViewDisplayMode = value;
	}

	public ThumbnailSize ThumbnailSize
	{
		get => _tabOptionsDto.ThumbnailSize;
		set => _tabOptionsDto.ThumbnailSize = value;
	}

	public bool RecursiveFolderBrowsing
	{
		get => _tabOptionsDto.RecursiveFolderBrowsing;
		set => _tabOptionsDto.RecursiveFolderBrowsing = value;
	}

	public bool GlobalOrderingForRecursiveFolderBrowsing
	{
		get => _tabOptionsDto.GlobalOrderingForRecursiveFolderBrowsing;
		set => _tabOptionsDto.GlobalOrderingForRecursiveFolderBrowsing = value;
	}

	public bool ShowImageViewImageInfo
	{
		get => _tabOptionsDto.ShowImageViewImageInfo;
		set => _tabOptionsDto.ShowImageViewImageInfo = value;
	}

	public int PanelsSplittingRatio
	{
		get => _tabOptionsDto.PanelsSplittingRatio;
		set => _tabOptionsDto.PanelsSplittingRatio = value;
	}

	public SlideshowInterval SlideshowInterval
	{
		get => _tabOptionsDto.SlideshowInterval;
		set => _tabOptionsDto.SlideshowInterval = value;
	}

	public bool ApplyImageOrientation
	{
		get => _tabOptionsDto.ApplyImageOrientation;
		set => _tabOptionsDto.ApplyImageOrientation = value;
	}

	public bool ShowThumbnailImageFileName
	{
		get => _tabOptionsDto.ShowThumbnailImageFileName;
		set => _tabOptionsDto.ShowThumbnailImageFileName = value;
	}

	public KeyboardScrollThumbnailIncrement KeyboardScrollThumbnailIncrement
	{
		get => _tabOptionsDto.KeyboardScrollThumbnailIncrement;
		set => _tabOptionsDto.KeyboardScrollThumbnailIncrement = value;
	}

	public UpsizeFullScreenImagesUpToScreenSize UpsizeFullScreenImagesUpToScreenSize
	{
		get => _tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize;
		set => _tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize = value;
	}

	public async Task SaveDefaultTabOptions()
	{
		CopyToDefaultTabOptions();

		try
		{
			var jsonContent = JsonSerializer.Serialize(
				_defaultTabOptions._tabOptionsDto,
				TabOptionsDtoJsonTypeInfo);

			if (!Directory.Exists(_configFolderPath))
			{
				Directory.CreateDirectory(_configFolderPath);
			}

			await File.WriteAllTextAsync(
				_tabOptionsConfigFilePath, jsonContent);
		}
		catch
		{
		}
	}

	private const string TabOptionsConfigFileName = "DefaultTabOptions.json";

	private const FileSystemEntryInfoOrdering DefaultFolderOrdering =
		FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection
		DefaultFolderOrderingDirection =
			FileSystemEntryInfoOrderingDirection.Ascending;

	private const FileSystemEntryInfoOrdering DefaultImageFileOrdering =
		FileSystemEntryInfoOrdering.Name;
	private const FileSystemEntryInfoOrderingDirection
		DefaultImageFileOrderingDirection =
			FileSystemEntryInfoOrderingDirection.Ascending;

	private const ImageViewDisplayMode DefaultImageViewDisplayMode =
		ImageViewDisplayMode.FullScreen;

	private const ThumbnailSize DefaultThumbnailSize =
		ThumbnailSize.TwoHundredFiftyPixels;

	private const bool DefaultRecursiveFolderBrowsing = false;
	private const bool DefaultGlobalOrderingForRecursiveFolderBrowsing = false;

	private const bool DefaultShowImageViewImageInfo = false;
	private const int DefaultPanelsSplittingRatio = 15;
	private const SlideshowInterval DefaultSlideshowInterval =
		SlideshowInterval.OneSecond;
	private const bool DefaultApplyImageOrientation = false;
	private const bool DefaultShowThumbnailImageFileName = true;
	private const KeyboardScrollThumbnailIncrement
		DefaultKeyboardScrollThumbnailIncrement =
			KeyboardScrollThumbnailIncrement.Twelve;
	private const UpsizeFullScreenImagesUpToScreenSize
		DefaultUpsizeFullScreenImagesUpToScreenSize =
			UpsizeFullScreenImagesUpToScreenSize.Disabled;

	private static readonly JsonTypeInfo<TabOptionsDto>
		TabOptionsDtoJsonTypeInfo = TabOptionsDtoJsonContext
			.Default
			.TabOptionsDto;

	private readonly string _configFolderPath;
	private readonly string _tabOptionsConfigFilePath;

	private readonly TabOptions _defaultTabOptions;

	private readonly TabOptionsDto _tabOptionsDto;

	private TabOptionsDto GetTabOptionsDto()
	{
		TabOptionsDto? tabOptionsDto;

		try
		{
			if (!File.Exists(_tabOptionsConfigFilePath))
			{
				return GetTabOptionsDtoWithDefaultValues();
			}

			var jsonContent = File.ReadAllText(_tabOptionsConfigFilePath);

			tabOptionsDto = JsonSerializer.Deserialize(
				jsonContent, TabOptionsDtoJsonTypeInfo);

			if (tabOptionsDto is null)
			{
				return GetTabOptionsDtoWithDefaultValues();
			}

			if (!IsValidEnumValue(tabOptionsDto.FolderOrdering))
			{
				tabOptionsDto.FolderOrdering = DefaultFolderOrdering;
			}

			if (!IsValidEnumValue(tabOptionsDto.FolderOrderingDirection))
			{
				tabOptionsDto.FolderOrderingDirection =
					DefaultFolderOrderingDirection;
			}

			if (!IsValidEnumValue(tabOptionsDto.ImageViewDisplayMode))
			{
				tabOptionsDto.ImageViewDisplayMode =
					DefaultImageViewDisplayMode;
			}

			if (!IsValidEnumValue(tabOptionsDto.ImageFileOrdering))
			{
				tabOptionsDto.ImageFileOrdering = DefaultImageFileOrdering;
			}

			if (!IsValidEnumValue(tabOptionsDto.ImageFileOrderingDirection))
			{
				tabOptionsDto.ImageFileOrderingDirection =
					DefaultImageFileOrderingDirection;
			}

			if (!IsValidEnumValue(tabOptionsDto.ThumbnailSize))
			{
				tabOptionsDto.ThumbnailSize = DefaultThumbnailSize;
			}

			if (!IsValidPanelsSplittingRatio(
					tabOptionsDto.PanelsSplittingRatio))
			{
				tabOptionsDto.PanelsSplittingRatio =
					DefaultPanelsSplittingRatio;
			}

			if (!IsValidEnumValue(tabOptionsDto.SlideshowInterval))
			{
				tabOptionsDto.SlideshowInterval = DefaultSlideshowInterval;
			}

			if (!IsValidEnumValue(
					tabOptionsDto.KeyboardScrollThumbnailIncrement))
			{
				tabOptionsDto.KeyboardScrollThumbnailIncrement =
					DefaultKeyboardScrollThumbnailIncrement;
			}

			if (!IsValidEnumValue(
					tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize))
			{
				tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize =
					DefaultUpsizeFullScreenImagesUpToScreenSize;
			}

			return tabOptionsDto;
		}
		catch
		{
			return GetTabOptionsDtoWithDefaultValues();
		}
	}

	private static TabOptionsDto GetTabOptionsDtoWithDefaultValues()
	{
		return new TabOptionsDto
		{
			FolderOrdering = DefaultFolderOrdering,
			FolderOrderingDirection = DefaultFolderOrderingDirection,

			ImageFileOrdering = DefaultImageFileOrdering,
			ImageFileOrderingDirection = DefaultImageFileOrderingDirection,

			ImageViewDisplayMode = DefaultImageViewDisplayMode,

			ThumbnailSize = DefaultThumbnailSize,

			RecursiveFolderBrowsing = DefaultRecursiveFolderBrowsing,
			GlobalOrderingForRecursiveFolderBrowsing =
				DefaultGlobalOrderingForRecursiveFolderBrowsing,

			ShowImageViewImageInfo = DefaultShowImageViewImageInfo,
			PanelsSplittingRatio = DefaultPanelsSplittingRatio,
			SlideshowInterval = DefaultSlideshowInterval,
			ApplyImageOrientation = DefaultApplyImageOrientation,
			ShowThumbnailImageFileName = DefaultShowThumbnailImageFileName,
			KeyboardScrollThumbnailIncrement =
				DefaultKeyboardScrollThumbnailIncrement,
			UpsizeFullScreenImagesUpToScreenSize =
				DefaultUpsizeFullScreenImagesUpToScreenSize
		};
	}

	private void CopyFromDefaultTabOptions()
	{
		_tabOptionsDto.FolderOrdering = _defaultTabOptions.FolderOrdering;
		_tabOptionsDto.FolderOrderingDirection =
			_defaultTabOptions.FolderOrderingDirection;

		_tabOptionsDto.ImageFileOrdering = _defaultTabOptions.ImageFileOrdering;
		_tabOptionsDto.ImageFileOrderingDirection =
			_defaultTabOptions.ImageFileOrderingDirection;

		_tabOptionsDto.ImageViewDisplayMode =
			_defaultTabOptions.ImageViewDisplayMode;

		_tabOptionsDto.ThumbnailSize = _defaultTabOptions.ThumbnailSize;

		_tabOptionsDto.RecursiveFolderBrowsing =
			_defaultTabOptions.RecursiveFolderBrowsing;
		_tabOptionsDto.GlobalOrderingForRecursiveFolderBrowsing =
			_defaultTabOptions.GlobalOrderingForRecursiveFolderBrowsing;

		_tabOptionsDto.ShowImageViewImageInfo =
			_defaultTabOptions.ShowImageViewImageInfo;
		_tabOptionsDto.PanelsSplittingRatio =
			_defaultTabOptions.PanelsSplittingRatio;
		_tabOptionsDto.SlideshowInterval = _defaultTabOptions.SlideshowInterval;
		_tabOptionsDto.ApplyImageOrientation =
			_defaultTabOptions.ApplyImageOrientation;
		_tabOptionsDto.ShowThumbnailImageFileName =
			_defaultTabOptions.ShowThumbnailImageFileName;
		_tabOptionsDto.KeyboardScrollThumbnailIncrement =
			_defaultTabOptions.KeyboardScrollThumbnailIncrement;
		_tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize =
			_defaultTabOptions.UpsizeFullScreenImagesUpToScreenSize;
	}

	private void CopyToDefaultTabOptions()
	{
		_defaultTabOptions.FolderOrdering = _tabOptionsDto.FolderOrdering;
		_defaultTabOptions.FolderOrderingDirection =
			_tabOptionsDto.FolderOrderingDirection;

		_defaultTabOptions.ImageFileOrdering = _tabOptionsDto.ImageFileOrdering;
		_defaultTabOptions.ImageFileOrderingDirection =
			_tabOptionsDto.ImageFileOrderingDirection;

		_defaultTabOptions.ImageViewDisplayMode =
			_tabOptionsDto.ImageViewDisplayMode;

		_defaultTabOptions.ThumbnailSize = _tabOptionsDto.ThumbnailSize;

		_defaultTabOptions.RecursiveFolderBrowsing =
			_tabOptionsDto.RecursiveFolderBrowsing;
		_defaultTabOptions.GlobalOrderingForRecursiveFolderBrowsing =
			_tabOptionsDto.GlobalOrderingForRecursiveFolderBrowsing;

		_defaultTabOptions.ShowImageViewImageInfo =
			_tabOptionsDto.ShowImageViewImageInfo;
		_defaultTabOptions.PanelsSplittingRatio =
			_tabOptionsDto.PanelsSplittingRatio;
		_defaultTabOptions.SlideshowInterval = _tabOptionsDto.SlideshowInterval;
		_defaultTabOptions.ApplyImageOrientation =
			_tabOptionsDto.ApplyImageOrientation;
		_defaultTabOptions.ShowThumbnailImageFileName =
			_tabOptionsDto.ShowThumbnailImageFileName;
		_defaultTabOptions.KeyboardScrollThumbnailIncrement =
			_tabOptionsDto.KeyboardScrollThumbnailIncrement;
		_defaultTabOptions.UpsizeFullScreenImagesUpToScreenSize =
			_tabOptionsDto.UpsizeFullScreenImagesUpToScreenSize;
	}

	private static bool IsValidEnumValue<TEnum>(TEnum enumValue)
		where TEnum : Enum
			=> Enum.IsDefined(typeof(TEnum), enumValue);

	private static bool IsValidPanelsSplittingRatio(int panelsSplittingRatio)
		=> panelsSplittingRatio is >= 0 and <= 100;
}

[JsonSerializable(typeof(TabOptionsDto))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class TabOptionsDtoJsonContext : JsonSerializerContext;
