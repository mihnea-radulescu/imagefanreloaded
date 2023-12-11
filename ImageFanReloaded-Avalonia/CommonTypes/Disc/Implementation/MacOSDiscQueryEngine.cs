using System.Collections.Generic;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public class MacOSDiscQueryEngine
	: UnixDiscQueryEngineBase
{
	public MacOSDiscQueryEngine(IImageFileFactory imageFileFactory)
		: base(imageFileFactory)
	{
		_supportedDrivePrefixes = new List<string>
		{
			"/Volumes/"
		};
	}

	protected override IReadOnlyCollection<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	#region Private

	private readonly IReadOnlyCollection<string> _supportedDrivePrefixes;

	#endregion
}
