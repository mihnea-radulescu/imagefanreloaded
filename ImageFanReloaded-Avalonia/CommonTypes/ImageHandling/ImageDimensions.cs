namespace ImageFanReloaded.CommonTypes.ImageHandling
{
	public record class ImageDimensions
	{
        public ImageDimensions(int width, int height)
        {
            Width = width;
            Height = height;
        }

		public ImageDimensions(double width, double height)
		{
			Width = (int)width;
			Height = (int)height;
		}

		public int Width { get; }
		public int Height { get; }
	}
}
