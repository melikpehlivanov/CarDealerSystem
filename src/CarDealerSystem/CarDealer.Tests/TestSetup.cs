namespace CarDealer.Tests
{
    using AutoMapper;

    public class TestSetup
    {
        private static readonly object Sync = new object();
        private static bool MapperInitialized = false;

        public static void InitializeMapper()
        {
            lock (Sync)
            {
                if (!MapperInitialized)
                {
                    Mapper.Initialize(config =>
                    {
                        config.AddProfile<TestProfile>();
                    });

                    MapperInitialized = true;
                }
            }
        }
    }
}
