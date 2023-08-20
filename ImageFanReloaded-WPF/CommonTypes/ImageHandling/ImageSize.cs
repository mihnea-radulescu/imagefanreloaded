namespace ImageFanReloaded.CommonTypes.ImageHandling
{
	public class ImageSize
	{
        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
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
