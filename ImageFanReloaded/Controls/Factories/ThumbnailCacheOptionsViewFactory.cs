using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ThumbnailCacheOptionsViewFactory
	: IThumbnailCacheOptionsViewFactory
{
	public ThumbnailCacheOptionsViewFactory(
		IGlobalParameters globalParameters,
		IThumbnailCacheOptions thumbnailCacheOptions,
		IDatabaseLogic databaseLogic)
	{
		_globalParameters = globalParameters;
		_thumbnailCacheOptions = thumbnailCacheOptions;

		_databaseLogic = databaseLogic;
	}

	public IThumbnailCacheOptionsView GetThumbnailCacheOptionsView()
	{
		IThumbnailCacheOptionsView thumbnailCacheOptionsView =
			new ThumbnailCacheOptionsWindow();

		thumbnailCacheOptionsView.GlobalParameters = _globalParameters;
		thumbnailCacheOptionsView.ThumbnailCacheOptions =
			_thumbnailCacheOptions;

		thumbnailCacheOptionsView.PopulateThumbnailCacheOptions();

		thumbnailCacheOptionsView.ThumbnailCacheSizeInMegabytes =
			_databaseLogic.GetThumbnailCacheSizeInMegabytes();

		return thumbnailCacheOptionsView;
	}

	private readonly IGlobalParameters _globalParameters;
	private readonly IThumbnailCacheOptions _thumbnailCacheOptions;
	private readonly IDatabaseLogic _databaseLogic;
}
