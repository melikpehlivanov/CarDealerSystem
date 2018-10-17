namespace CarDealer.Tests
{
    using AutoMapper;

    public class TestSetup
    {
        private static bool mapperInitialized = false;

        public static void InitializeMapper()
        {
            if (!mapperInitialized)
            {
                Mapper.Initialize(config =>
                {
                    config.AddProfile<TestProfile>();
                });

                mapperInitialized = true;
            }
        }
    }
}
