namespace AlloyOptimisation.Core;

/// <summary>
/// Generic exception for this module.
/// </summary>
public class AlloyException : Exception
{
    public AlloyException() { }

    public AlloyException(string message) : base(message) { }

    public AlloyException(string message, Exception innerException) : base(message, innerException) { }
}