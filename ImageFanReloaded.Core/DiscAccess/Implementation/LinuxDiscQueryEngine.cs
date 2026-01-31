using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class LinuxDiscQueryEngine : UnixDiscQueryEngineBase
{
	public LinuxDiscQueryEngine(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory,
		IFileSizeEngine fileSizeEngine)
		: base(globalParameters, imageFileFactory, fileSizeEngine)
	{
		_supportedDrivePrefixes = ["/media/", "/mnt/"];
	}

	protected override IReadOnlyList<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;
}
