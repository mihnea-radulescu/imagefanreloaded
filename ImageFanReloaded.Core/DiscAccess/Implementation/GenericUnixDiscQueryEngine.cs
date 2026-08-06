using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class GenericUnixDiscQueryEngine : UnixDiscQueryEngineBase
{
	public GenericUnixDiscQueryEngine(
		IGlobalParameters globalParameters, IImageFileFactory imageFileFactory)
			: base(globalParameters, imageFileFactory)
	{
		_supportedDrivePrefixes = ["/media/", "/mnt/", "/Volumes/"];
	}

	protected override IReadOnlyList<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;
}
