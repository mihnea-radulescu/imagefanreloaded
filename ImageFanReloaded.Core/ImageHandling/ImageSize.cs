namespace ImageFanReloaded.Core.ImageHandling;

public record ImageSize
{
	public ImageSize(int width, int height)
	{
		Width = width;
		Height = height;

		AspectRatio = (double)Width / (double)Height;
	}

	public ImageSize(double width, double height)
		: this((int)width, (int)height)
	{
	}

	public ImageSize(int squareLength)
		: this(squareLength, squareLength)
	{
	}

	public int Width { get; }
	public int Height { get; }

	public double AspectRatio { get; }

	public override string ToString() => $"{Width}x{Height}";
}
