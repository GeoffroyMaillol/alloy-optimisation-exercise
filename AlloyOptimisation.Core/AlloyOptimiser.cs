namespace AlloyOptimisation.Core;

/// <summary>
/// Finds the alloy with the highest creep coefficient, while meeting a given cost constraint. 
/// </summary>
public class AlloyOptimiser
{
    private List<AlloyComponentConfig> _config;

    public AlloyOptimiser(List<AlloyComponentConfig> config)
    {
        _config = config;
    }

    /// <summary>
    /// Generates the alloy with the highest creep coefficient, while meeting the provided cost constraint.
    /// </summary>
    /// <param name="costConstraint">The maximum allowed cost of the alloy</param>
    /// <returns>The optimum alloy, or null if none were found to meet the cost constraint</returns>
    public Alloy GetOptimumAlloy(decimal costConstraint)
    {
        Alloy? optimumAlloy = null;
        GridSearch(AlloyComponentConfig.GetConfigWithoutBaseElement(_config), 0, new Alloy(_config), ref optimumAlloy, costConstraint);
        
        return optimumAlloy;
    }

    /// <summary>
    /// Brute force recursive method to select the optimum alloy by testing each combination.
    /// It's not optimised and would work poorly with a large amount of configuration data, but for the purpose of this exercise it was deemed sufficient.
    /// </summary>
    /// <param name="alloyComponents">The component data</param>
    /// <param name="index">Current index of the component data</param>
    /// <param name="currentAlloy">The alloy being currently investigated</param>
    /// <param name="optimumAlloy">The alloy currently meeting the constraints</param>
    /// <param name="costConstraint">The cost contraint to satisfy</param>
    static void GridSearch(List<AlloyComponentConfig> alloyComponents, int index, Alloy currentAlloy, ref Alloy optimumAlloy, decimal costConstraint)
    {
        if (index == alloyComponents.Count)
        {
            if (currentAlloy.GetCost() > costConstraint) { return; }

            if (optimumAlloy == null || currentAlloy.GetCreepResistance() > optimumAlloy.GetCreepResistance())
            {
                optimumAlloy = new Alloy(currentAlloy);
            }
            return;
        }

        var alloyComponent = alloyComponents[index];
        for (decimal atomicPercentage = alloyComponent.Min; atomicPercentage <= alloyComponent.Max; atomicPercentage += alloyComponent.Step)
        {
            currentAlloy.UpdateComponent(alloyComponent.Element, atomicPercentage);
            GridSearch(alloyComponents, index + 1, currentAlloy, ref optimumAlloy, costConstraint);
        }
    }
    
}