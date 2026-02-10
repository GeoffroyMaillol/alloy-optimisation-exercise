namespace AlloyOptimisation.Tests;

using AlloyOptimisation.Core;

public class AlloyTests
{
    private readonly List<AlloyComponentConfig> _config;

    public AlloyTests()
    {
        _config = GenerateTestConfig();
    }

    private List<AlloyComponentConfig> GenerateTestConfig()
    {
        return new List<AlloyComponentConfig>{
            new ("Cr", 2.0911350e+16m, 14m, 14.5m, 22.0m, 0.5m),
            new ("Co", 7.2380280e+16m, 80.5m, 0m, 25.0m, 1.0m),
            new ("Nb", 1.0352738e+16m, 42.5m, 0m, 1.5m, 0.1m),
            new ("Mo", 8.9124547e+16m, 16m, 1.5m, 6.0m, 0.5m),
            new ("Ni", null, 8.9m, 0m, 100m, 0m),
        };
    }

    [Fact]
    public void CompositionIsInitiallyOnlyBaseComponent()
    {
        var alloy = new Alloy(_config);
        Assert.Single(alloy.GetComposition());
        Assert.Equal("Ni", alloy.GetComposition()[0].Element);
        Assert.Equal(100, alloy.GetComposition()[0].AtomicPercentage);
    }

    [Fact]
    public void CanAddElement()
    {
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Cr", 20);
        Assert.NotEmpty(alloy.GetComposition());
        Assert.Equal("Cr", alloy.GetComposition()[0].Element);
        Assert.Equal(20, alloy.GetComposition()[0].AtomicPercentage);
        Assert.Equal("Ni", alloy.GetComposition()[1].Element);
        Assert.Equal(80, alloy.GetComposition()[1].AtomicPercentage);
    }

    [Fact]
    public void CannotAddElementNotInConfig()
    {
        var alloy = new Alloy(_config);
        var ex = Assert.Throws<AlloyException>(() => alloy.TryUpdateComponent("He", 5));

        Assert.Equal("Element not allowed in alloy composition", ex.Message);
    }

    [Fact]
    public void CannotAddElementWithRatioBelowMin()
    {
        var alloy = new Alloy(_config);
        var ex = Assert.Throws<AlloyException>(() => alloy.TryUpdateComponent("Cr", 13));

        Assert.Equal("Element atomic percentage is below allowed minimum in alloy composition", ex.Message);
    }

    [Fact]
    public void CannotAddElementWithRatioAboveMax()
    {
        var alloy = new Alloy(_config);
        var ex = Assert.Throws<AlloyException>(() => alloy.TryUpdateComponent("Cr", 25));

        Assert.Equal("Element atomic percentage is above allowed maximum in alloy composition", ex.Message);
    }

    [Fact]
    public void CannotHaveDuplicateElementInComposition()
    {
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Cr", 20);
        alloy.TryUpdateComponent("Cr", 21);
        Assert.Equal(2, alloy.GetComposition().Count);
        Assert.Equal("Cr", alloy.GetComposition()[0].Element);
        Assert.Equal(21, alloy.GetComposition()[0].AtomicPercentage); 
    }

    [Fact]
    public void CannotAddBaseElement()
    {
        var alloy = new Alloy(_config);
        var ex = Assert.Throws<AlloyException>(() => alloy.TryUpdateComponent("Ni", 25));

        Assert.Equal("Element Ni is the base element of the alloy so its composition cannot be changed", ex.Message);
    }

    [Fact]
    public void SettingAtomicPercentageToZeroRemovesTheElementFromTheAlloy()
    {
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Co", 20);
        Assert.Equal(2, alloy.GetComposition().Count);
        alloy.TryUpdateComponent("Co", 0);
        Assert.Single(alloy.GetComposition());
        Assert.Equal("Ni", alloy.GetComposition()[0].Element);
        Assert.Equal(100, alloy.GetComposition()[0].AtomicPercentage);
    }

    [Fact]
    public void CanCalculateCreepResistance()
    {
        // Testing against provided values
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Cr", 15);
        alloy.TryUpdateComponent("Co", 10);
        alloy.TryUpdateComponent("Nb", 1);
        alloy.TryUpdateComponent("Mo", 2);
        Assert.Equal(1226074882000000000, alloy.GetCreepResistance());

        alloy.TryUpdateComponent("Cr", 20);
        alloy.TryUpdateComponent("Co", 0);
        alloy.TryUpdateComponent("Nb", 0);
        alloy.TryUpdateComponent("Mo", 1.5m);
        Assert.Equal(551913820500000000, alloy.GetCreepResistance());

        alloy.TryUpdateComponent("Cr", 22);
        alloy.TryUpdateComponent("Co", 25);
        alloy.TryUpdateComponent("Nb", 1.5m);
        alloy.TryUpdateComponent("Mo", 6);
        Assert.Equal(2819833089000000000, alloy.GetCreepResistance());
    }

    [Fact]
    public void CreepResistanceCalculationThrowsExceptionIfAlloyIsInvalid()
    {
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Co", 10);
        alloy.TryUpdateComponent("Nb", 1);
        alloy.TryUpdateComponent("Mo", 2);

        var ex = Assert.Throws<AlloyException>(() => alloy.GetCreepResistance());
        Assert.Equal("Alloy is invalid: Atomic percentage for element Cr is below allowed minimum", ex.Message);
    }

    [Fact]
    public void CanCalculateCost()
    {
        var alloy = new Alloy(_config);
        alloy.TryUpdateComponent("Cr", 15);
        alloy.TryUpdateComponent("Co", 10);
        alloy.TryUpdateComponent("Nb", 1);
        alloy.TryUpdateComponent("Mo", 2);
        Assert.Equal(17.303m, alloy.GetCost());
    }

    [Fact]
    public void CannotCreateAlloyWithElementRatioAboveOneHundredPercent()
    {
        var alloy = new Alloy(new List<AlloyComponentConfig>{
            new ("Cr", 2.0911350e+16m, 14m, 14.5m, 100.0m, 0.5m),
            new ("Co", 7.2380280e+16m, 80.5m, 0m, 100.0m, 1.0m),
            new ("Nb", 1.0352738e+16m, 42.5m, 0m, 100.0m, 0.1m),
            new ("Mo", 8.9124547e+16m, 16m, 1.5m, 100.0m, 0.5m),
            new ("Ni", null, 8.9m, 0m, 100m, 0m),
        });
        alloy.TryUpdateComponent("Cr", 50);
        var success = alloy.TryUpdateComponent("Co", 55);
        Assert.False(success);
    }
}
