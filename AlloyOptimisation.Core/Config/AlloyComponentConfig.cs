namespace AlloyOptimisation.Core;

/// <summary>
/// Class to represent the parameters of an alloy component.
/// Designed to be loaded from a matching JSON file.
/// Also home of related utility methods. 
/// </summary>
/// <param name="Element">The element symbol of the component, e.g. Ni for Nickel</param>
/// <param name="CreepCoefficient">The creeo coefficient</param>
/// <param name="Cost">The cost of the component</param>
/// <param name="Min">The minimum atomic percentage allowed for the alloy</param>
/// <param name="Max">The maximum atomic percentage allowed for the alloy</param>
/// <param name="Step">The step by which to raise / decrease the atomic percentage when looking for an appropriate alloy composition</param>
public record AlloyComponentConfig(
    string Element, 
    decimal? CreepCoefficient,
    decimal Cost,
    decimal Min,
    decimal Max,
    decimal Step
)
{
    /// <summary>
    /// Provides the list of component configuration, without the configuration of the base element.
    /// This is because in some instances (like creep resistance calculation), the base element should be excluded.
    /// The base element is identified, by convention, by the lack of creep coefficient.
    /// </summary>
    /// <param name="config">the configuration</param>
    /// <returns>the configuration with the base element removed</returns>
    public static List<AlloyComponentConfig> GetConfigWithoutBaseElement(List<AlloyComponentConfig> config)
    {
        return [.. config.Where(e => !IsBaseElement(e))];
    }

    /// <summary>
    /// Check for the alloy's base element.
    /// Was extracted like this, because the condition is a convention and it's used in several places,
    /// so it's cleaner to refactor it in 1 place. 
    /// </summary>
    /// <param name="elementConfig"></param>
    /// <returns></returns>
    public static bool IsBaseElement(AlloyComponentConfig elementConfig)
    {
        // By convention the base element doesn't have a creep coefficient.
        return elementConfig.CreepCoefficient == null;
    }
}
