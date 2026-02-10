namespace AlloyOptimisation.Tests;

using AlloyOptimisation.Core;

public class AlloyOptimiserTests
{
    private readonly List<AlloyComponentConfig> _config;

    public AlloyOptimiserTests()
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
    public void CanCalculateExpectedOptimumCreepResistance()
    {
        // Assuming that if it works for 1, it works for all.
        var testCostConstraint = 18;
        var alloyOptimiser = new AlloyOptimiser(_config);
        var alloy = alloyOptimiser.GetOptimumAlloy(testCostConstraint);
        Assert.Equal(1729987793800000000m, alloy.GetCreepResistance());
        Assert.True(testCostConstraint >= alloy.GetCost());
    }

    [Fact]
    public void ReturnsNullIfCostConstraintIsNotSatisfied()
    {
        var testCostConstraint = 0;
        var alloyOptimiser = new AlloyOptimiser(_config);
        var alloy = alloyOptimiser.GetOptimumAlloy(testCostConstraint);
        Assert.Null(alloy);
    }

    [Fact]
    public void ReturnsNullIfCostConstraintIsNegative()
    {
        var testCostConstraint = -1;
        var alloyOptimiser = new AlloyOptimiser(_config);
        var alloy = alloyOptimiser.GetOptimumAlloy(testCostConstraint);
        Assert.Null(alloy);
    }
}
