namespace AlloyOptimisation.Core;

/// <summary>
/// Implementation of the AlloyComponentConfigProvider for a Ni-based alloy.
/// </summary>
public class NickelAlloyComponentConfigProvider : AlloyComponentConfigProvider
{
    public override string GetConfigFileName()
    {
        return "NickelAlloyComponentConfig.json";
    }

}
