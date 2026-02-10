using AlloyOptimisation.Core;

namespace AlloyOptimisation.Tests;

public class MockAlloyComponentConfigProvider : AlloyComponentConfigProvider
{
    public override string GetConfigFileName()
    {
        return "TestAlloyComponentConfig.json";
    }
}

public class MissingFileAlloyComponentConfigProvider : AlloyComponentConfigProvider
{
    public override string GetConfigFileName()
    {
        return "NonExistentFile.json";
    }
}

public class AlloyComponentProviderTests
{
    [Fact]
    public void CanLoadConfig()
    {
        var configProvider = new MockAlloyComponentConfigProvider();
        var config = configProvider.LoadConfig();
        Assert.NotEmpty(config);

        // Only check 1st item, assuming that if 1 comes through the rest should follow (if it's prperly formatted of course!).
        Assert.Equal("Cr", config[0].Element);
        Assert.Equal(2.0911350E+16m, config[0].CreepCoefficient);
        Assert.Equal(14m, config[0].Cost);
        Assert.Equal(14.5m, config[0].Min);
        Assert.Equal(22m, config[0].Max);
        Assert.Equal(0.5m, config[0].Step);
    }

    [Fact]
    public void ThrowsExceptionIfFileDoesNotExist()
    {
        var configProvider = new MissingFileAlloyComponentConfigProvider();
        var ex = Assert.Throws<FileNotFoundException>(() => configProvider.LoadConfig());
    }
}
