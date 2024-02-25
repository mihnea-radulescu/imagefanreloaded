using System.Collections.Generic;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class LinuxDiscQueryEngine : UnixDiscQueryEngineBase
{
	public LinuxDiscQueryEngine(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, imageFileFactory)
	{
		_supportedDrivePrefixes = new List<string>
		{
			"/home/",
			"/media/",
			"/mnt/"
		};
	}

	protected override IReadOnlyCollection<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	#region Private

	private readonly IReadOnlyCollection<string> _supportedDrivePrefixes;

	#endregion
}
