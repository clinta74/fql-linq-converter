using System;

namespace Fql.Linq.Converter.Models;

[Serializable]
public class FilterItem
{
    public OperationTypes Operation { get; set; }

    public string Value { get; set; } = string.Empty;

    public bool IsPreset { get; set; }
}
