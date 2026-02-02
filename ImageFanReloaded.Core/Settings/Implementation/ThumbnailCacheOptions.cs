using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class ThumbnailCacheOptions : IThumbnailCacheOptions
{
	public ThumbnailCacheOptions(string configFolderPath)
	{
		_configFolderPath = configFolderPath;
		_thumbnailCacheOptionsConfigFilePath = Path.Combine(
			_configFolderPath, ThumbnailCacheOptionsConfigFileName);

		_thumbnailCacheOptionsDto = GetThumbnailCacheOptionsDto();
	}

	public bool EnableThumbnailCaching
	{
		get => _thumbnailCacheOptionsDto.EnableThumbnailCaching;
		set => _thumbnailCacheOptionsDto.EnableThumbnailCaching = value;
	}

	public bool ShouldClearThumbnailCache
	{
		get => _thumbnailCacheOptionsDto.ShouldClearThumbnailCache;
		set => _thumbnailCacheOptionsDto.ShouldClearThumbnailCache = value;
	}

	public async Task SaveThumbnailCacheOptions()
	{
		try
		{
			var jsonContent = JsonSerializer.Serialize(
				_thumbnailCacheOptionsDto,
				ThumbnailCacheOptionsDtoJsonTypeInfo);

			if (!Directory.Exists(_configFolderPath))
			{
				Directory.CreateDirectory(_configFolderPath);
			}

			await File.WriteAllTextAsync(
				_thumbnailCacheOptionsConfigFilePath, jsonContent);
		}
		catch
		{
		}
	}

	private const string ThumbnailCacheOptionsConfigFileName =
		"ThumbnailCacheOptions.json";

	private static readonly JsonTypeInfo<ThumbnailCacheOptionsDto>
		ThumbnailCacheOptionsDtoJsonTypeInfo =
			ThumbnailCacheOptionsDtoJsonContext
				.Default
				.ThumbnailCacheOptionsDto;

	private readonly string _configFolderPath;
	private readonly string _thumbnailCacheOptionsConfigFilePath;

	private readonly ThumbnailCacheOptionsDto _thumbnailCacheOptionsDto;

	private ThumbnailCacheOptionsDto GetThumbnailCacheOptionsDto()
	{
		ThumbnailCacheOptionsDto? thumbnailCacheOptionsDto;

		try
		{
			if (!File.Exists(_thumbnailCacheOptionsConfigFilePath))
			{
				return GetThumbnailCacheOptionsDtoWithDefaultValues();
			}

			var jsonContent = File.ReadAllText(
				_thumbnailCacheOptionsConfigFilePath);

			thumbnailCacheOptionsDto = JsonSerializer.Deserialize(
				jsonContent, ThumbnailCacheOptionsDtoJsonTypeInfo);

			if (thumbnailCacheOptionsDto is null)
			{
				return GetThumbnailCacheOptionsDtoWithDefaultValues();
			}

			return thumbnailCacheOptionsDto;
		}
		catch
		{
			return GetThumbnailCacheOptionsDtoWithDefaultValues();
		}
	}

	private static ThumbnailCacheOptionsDto
		GetThumbnailCacheOptionsDtoWithDefaultValues()
	{
		return new ThumbnailCacheOptionsDto
		{
			EnableThumbnailCaching = false,
			ShouldClearThumbnailCache = false
		};
	}
}

[JsonSerializable(typeof(ThumbnailCacheOptionsDto))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class ThumbnailCacheOptionsDtoJsonContext
	: JsonSerializerContext;
