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
        public IDiscQueryEngine DiscQueryEngineInstance
        {
            get { return new TestingDiscQueryEngine(); }
        }

        public IImageResizer ImageResizerInstance
        {
            get { return new TestingImageResizer(); }
        }

        public IMainView MainViewInstance
        {
            get { throw new NotImplementedException(); }
        }

        public IImageFile GetImageFile(string filePath)
        {
            return new TestingImageFile(filePath);
        }

        public IImageView GetImageView()
        {
            throw new NotImplementedException();
        }
    }
}
