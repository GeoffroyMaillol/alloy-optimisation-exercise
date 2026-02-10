using System.Dynamic;
using System.Text.Json;

namespace AlloyOptimisation.Core;

/// <summary>
/// Provides the configuration for alloy components.
/// </summary>
public abstract class AlloyComponentConfigProvider
{
    /// <summary>
    /// Loads the configuration for an alloy, from JSON.
    /// </summary>
    /// <returns>The list of component configuration</returns>
    public List<AlloyComponentConfig> LoadConfig()
    {
        var configFilePath = Path.Combine(AppContext.BaseDirectory, GetConfigFileName());
        var jsonData = File.ReadAllText(configFilePath);
        var alloyConfig = JsonSerializer.Deserialize<List<AlloyComponentConfig>>(jsonData)!;
        return alloyConfig;
    }

    /// <summary>
    /// Defines the name of the configuration file containing the alloy configuration.
    /// </summary>
    /// <returns>The file name</returns>
    public abstract string GetConfigFileName();

}
