namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class FileSizeEngine : IFileSizeEngine
{
	public decimal ConvertToKilobytes(long sizeInBytes)
		=> (decimal)sizeInBytes / OneKilobyteInBytes;

	public int ConvertToMegabytes(decimal sizeInKilobytes)
		=> (int)(sizeInKilobytes / OneMegabyteInKilobytes);
	
	#region Private
	
	private const decimal OneKilobyteInBytes = 1024;
	
	private const decimal OneMegabyteInKilobytes = 1024;
	
	#endregion
}
