namespace AlloyOptimisation.Core;

/// <summary>
/// Class holding the main logic for defining an alloy.
/// </summary>
public class Alloy
{
    private List<AlloyComponentConfig> _config;
    public List<AlloyComponent> _composition = [];

    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="config">The alloy component config list, required to calculate the cost and creep resistance of the alloy.</param>
    public Alloy(List<AlloyComponentConfig> config)
    {
        _config = config;
        SetBaseElementRatio();
    }

    /// <summary>
    /// Constructor to deep clone an alloy object, required by the optimiser.
    /// </summary>
    /// <param name="baseAlloy">the source alloy to be cloned</param>
    public Alloy(Alloy sourceAlloy)
    {
        _config = sourceAlloy._config;
        foreach(var component in sourceAlloy.GetComposition())
        {
            // Don't validate the values as they should have been validated on the source alloy. 
            _composition.Add(new AlloyComponent(component.Element, component.AtomicPercentage));
        }
        SetBaseElementRatio();
    }

    /// <summary>
    /// Balances the atomic percentage total to 100% with the base element.
    /// </summary>
    private void SetBaseElementRatio()
    {
        var baseElement = GetBaseElement();
        if (_composition.Count == 0)
        {
            _composition.Add(new AlloyComponent(baseElement, 100));
            return;
        }

        _composition.Remove(new AlloyComponent(baseElement, 0));
        _composition.Add(new AlloyComponent(baseElement, GetCompositionBalance()));
    }

    /// <summary>
    /// Returns the base element of the alloy.
    /// </summary>
    /// <returns>the atomic symbol of the alloy's base element</returns>
    private string GetBaseElement()
    {
        return _config.First(AlloyComponentConfig.IsBaseElement).Element;
    }
    
    /// <summary>
    /// Get the composition's residual percentage.
    /// </summary>
    /// <returns>the composition's residual percentage</returns>
    private decimal GetCompositionBalance()
    {
        return 100 - _composition.Sum(e => e.AtomicPercentage);
    }
    
    /// <summary>
    /// Returns the alloy composition.
    /// </summary>
    /// <returns>the alloy composition</returns>
    public List<AlloyComponent> GetComposition()
    {
        return _composition;
    }
    
    /// <summary>
    /// Add a component to the alloy, or update an present component.
    /// The inputs are validated.
    /// </summary>
    /// <param name="componentElement">The atomic symbol of the element, e.g. "Ni" for nickel</param>
    /// <param name="atomicPercentage">The desired atomic percentage</param>
    public void UpdateComponent(string componentElement, decimal atomicPercentage)
    {
        ValidateNewComponent(componentElement, atomicPercentage);
        var existingElementComponent = _composition.Where(e => e.Element == componentElement).ElementAtOrDefault(0);
        if (existingElementComponent != null)
        {
            _composition.Remove(existingElementComponent);
        }

        if (atomicPercentage > 0)
        {
            _composition.Add(new AlloyComponent(componentElement, atomicPercentage));
        }
        SetBaseElementRatio();
    }

    /// <summary>
    /// Validate the inputs for adding an alloy component.
    /// The element needs to be part of the alloy configuration and atomic percentage within the Min/Max range.
    /// </summary>
    /// <param name="componentElement">The atomic symbol of the element, e.g. "Ni" for nickel</param>
    /// <param name="atomicPercentage">The desired atomic percentage</param>
    /// <exception cref="AlloyException">thrown if parameters aren't valid</exception>
    private void ValidateNewComponent(string componentElement, decimal atomicPercentage)
    {
        if (!_config.Any(e => e.Element == componentElement))
        {
            throw new AlloyException("Element not allowed in alloy composition");
        }

        var elementConfig = _config.First(e => e.Element == componentElement);
        if (AlloyComponentConfig.IsBaseElement(elementConfig))
        {
            throw new AlloyException($"Element {componentElement} is the base element of the alloy so its composition cannot be changed");
        }

        if (atomicPercentage < elementConfig.Min)
        {
            throw new AlloyException("Element atomic percentage is below allowed minimum in alloy composition");
        }

        if (atomicPercentage > elementConfig.Max)
        {
            throw new AlloyException("Element atomic percentage is above allowed maximum in alloy composition");
        }
    }

    /// <summary>
    /// Calculates the creep resistance of the alloy's current composition.
    /// The alloy must be valid according to its configuration.
    /// </summary>
    /// <returns>the creep resistance</returns>
    public decimal GetCreepResistance()
    {
        ValidateAlloy();
        decimal creepResistance = 0m;
        foreach (var component in _composition)
        {
            var componentConfig = _config.First(e => e.Element == component.Element);
            if (componentConfig.CreepCoefficient != null)
            {
                creepResistance += (decimal)(componentConfig.CreepCoefficient * component.AtomicPercentage);
            }
        }
        return creepResistance;
    }

    /// <summary>
    /// Validates the alloy's current composition against the configuration.
    /// </summary>
    /// <exception cref="AlloyException">thrown if the alloy is not valid</exception>
    private void ValidateAlloy()
    {
        var configWithoutBaseElement = AlloyComponentConfig.GetConfigWithoutBaseElement(_config);
        foreach (var componentConfig in configWithoutBaseElement)
        {
            var alloyComponent = _composition.FirstOrDefault(e => e.Element == componentConfig.Element);
            if (alloyComponent == null && componentConfig.Min > 0)
            {
                // This is handling the case where a required component is missing.
                // The rest should be handled when adding a component (famous last words).
                throw new AlloyException($"Alloy is invalid: Atomic percentage for element {componentConfig.Element} is below allowed minimum");
            }
        }
    }
    
    /// <summary>
    /// Calculates the cost of the alloy.
    /// </summary>
    /// <returns>the alloy's cost</returns>
    public decimal GetCost()
    {
        decimal cost = 0m;
        foreach (var component in _composition)
        {
            var componentConfig = _config.First(e => e.Element == component.Element);
            cost += componentConfig.Cost * component.AtomicPercentage / 100;
        }
        return cost;
    }
}