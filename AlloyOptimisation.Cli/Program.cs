
using AlloyOptimisation.Core;

/// <summary>
/// Simple CLI tool to use the alloy optimiser module.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            PrintUsage();
            return 1;
        }

        if (!decimal.TryParse(args[0], out var costConstraint))
        {
            Console.Error.WriteLine("Invalid cost constraint, please ensure you enter a decimal number.");
            return 1;
        }

        var alloyConfig = new NickelAlloyComponentConfigProvider().LoadConfig();
        var optimiser = new AlloyOptimiser(alloyConfig);
        var optimumAlloy = optimiser.GetOptimumAlloy(costConstraint);
        if (optimumAlloy == null)
        {
            Console.WriteLine("No valid alloy found under the given cost constraint.");
            return 0;
        }

        PrintAlloy(optimumAlloy);
        return 0;
    }

    static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  AlloyOptimisation.Cli <maxCost>");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  AlloyOptimisation.Cli 18");
    }

    static void PrintAlloy(Alloy alloy)
    {
        Console.WriteLine("Optimal alloy composition:");
        foreach (var component in alloy.GetComposition())
        {
            Console.WriteLine($"  {component.Element}: {component.AtomicPercentage:F2}");
        }

        Console.WriteLine();
        Console.WriteLine($"Creep Resistance: {alloy.GetCreepResistance():E}");
        Console.WriteLine($"Total Cost:       {alloy.GetCost():F2}");
    }
}
