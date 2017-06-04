using ImageFanReloaded.Factories.Interface;

namespace ImageFanReloaded.Factories
{
    public class TypesFactoryResolver
    {
        public static ITypesFactory TypesFactoryInstance
        {
            get
            {
                if (TypesFactory == null)
                {
                    TypesFactory = new ProductionTypesFactory();
                }

                return TypesFactory;
            }
            set
            {
                if (value != null)
                {
                    TypesFactory = value;
                }
            }
        }


        #region Private

        private static ITypesFactory TypesFactory;

        #endregion
    }
}
