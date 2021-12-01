using SynologyDotNet.Core.Helpers.Testing;

namespace SynologyDotNet.AudioStation.IntegrationTest
{
    public abstract class MyTestBase : TestBase
    {
        protected static MyConfig Config { get; } = LoadJsonFile("config.audiostation.json", c =>
        {

        }, new MyConfig());
    }
}
