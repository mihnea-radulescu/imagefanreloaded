namespace ImageFanReloaded.CommonTypes.ImageHandling
{
	public record class ImageSize
	{
        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

		public ImageSize(double width, double height)
		{
			Width = (int)width;
			Height = (int)height;
		}

        public ImageSize(int squareLength)
        {
            Width = squareLength;
            Height = squareLength;
        }

        public int Width { get; }
		public int Height { get; }
	}
}
