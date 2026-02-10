namespace AlloyOptimisation.Core;

/// <summary>
/// Record to hold the data for an alloy component.
/// </summary>
/// <param name="Element">The symbol of the element, e.g. Ni for Nickel</param>
/// <param name="AtomicPercentage">The atomic percentage of the element in the alloy</param>
public record AlloyComponent(string Element, decimal AtomicPercentage)
{
    /// <summary>
    /// Overriding Equals here, so that 2 AlloyComponent with the same element are equal,
    /// regardless of the atomic percentage.
    /// This is to facilitate replacing an element in a list.
    /// </summary>
    /// <param name="other">the other element to compare to</param>
    /// <returns>true if equal, false otherwise</returns>
    public virtual bool Equals(AlloyComponent? other)
        => other is not null && Element == other.Element;

    public override int GetHashCode() => Element.GetHashCode();
}
