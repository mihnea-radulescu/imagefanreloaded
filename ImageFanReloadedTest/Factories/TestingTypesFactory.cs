using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Views.Interface;
using ImageFanReloadedTest.Stubs;
using System;

namespace ImageFanReloadedTest.Factories
{
    public class TestingTypesFactory
        : ITypesFactory
    {
        public IDiscQueryEngine GetDiscQueryEngine()
        {
            return new TestingDiscQueryEngine();
        }

        public IImageResizer GetImageResizer()
        {
            return new TestingImageResizer();
        }

        public IImageFile GetImageFile(string filePath)
        {
            return new TestingImageFile(filePath);
        }

        public IMainView GetMainView()
        {
            throw new NotImplementedException();
        }

        public IImageView GetImageView()
        {
            throw new NotImplementedException();
        }
    }
}
