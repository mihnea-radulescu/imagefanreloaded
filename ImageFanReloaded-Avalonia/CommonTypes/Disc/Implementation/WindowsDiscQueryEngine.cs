﻿using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public class WindowsDiscQueryEngine
	: DiscQueryEngineBase
{
	public WindowsDiscQueryEngine(IImageFileFactory imageFileFactory)
		: base(imageFileFactory)
	{
	}

	protected override bool IsDrive(string driveName)
		=> true;
}
