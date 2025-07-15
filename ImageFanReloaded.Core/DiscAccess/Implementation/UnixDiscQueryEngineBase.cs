using System;
using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class UnixDiscQueryEngineBase : DiscQueryEngineBase
{
	#region Protected
	
	protected UnixDiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
		_nameComparison = globalParameters.NameComparer.ToStringComparison();
	}

	protected override bool IsSupportedDrive(string driveName)
	{
		if (driveName.Equals(RootPath, _nameComparison))
		{
			return true;
		}

		var isSupportedDrive = SupportedDrivePrefixes.Any(
			aSupportedDrivePrefix => driveName.StartsWith(aSupportedDrivePrefix, _nameComparison));

		return isSupportedDrive;
	}

	protected abstract IReadOnlyList<string> SupportedDrivePrefixes { get; }
	
	#endregion

	#region Private

	private const string RootPath = "/";
	
	private readonly StringComparison _nameComparison;

	#endregion
}
