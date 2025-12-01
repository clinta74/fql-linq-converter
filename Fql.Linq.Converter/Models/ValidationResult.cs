using System.Collections.Generic;
using System.Linq;

namespace Fql.Linq.Converter.Models;

/// <summary>
/// Represents the result of FQL validation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation was successful.
    /// </summary>
    public bool IsValid => !Errors.Any();

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public List<string> Errors { get; } = new List<string>();

    /// <summary>
    /// Gets the collection of validation warnings.
    /// </summary>
    public List<string> Warnings { get; } = new List<string>();

    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    public void AddError(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Adds a warning to the validation result.
    /// </summary>
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    /// <summary>
    /// Returns a summary of all errors and warnings.
    /// </summary>
    public override string ToString()
    {
        var messages = new List<string>();
        
        if (Errors.Any())
        {
            messages.Add($"Errors ({Errors.Count}):");
            messages.AddRange(Errors.Select(e => $"  - {e}"));
        }
        
        if (Warnings.Any())
        {
            messages.Add($"Warnings ({Warnings.Count}):");
            messages.AddRange(Warnings.Select(w => $"  - {w}"));
        }

        return messages.Any() ? string.Join("\n", messages) : "Validation passed with no errors or warnings.";
    }
}
