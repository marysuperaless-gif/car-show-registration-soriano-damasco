namespace Application.Common;

public static class EnumNames
{
    /// <summary>Parses an enum by NAME only (case-insensitive). Numbers like "99" are rejected.</summary>
    public static bool TryParse<TEnum>(string? value, out TEnum result) where TEnum : struct, Enum
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var name = Enum.GetNames<TEnum>()
            .FirstOrDefault(n => n.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        if (name is null) return false;

        result = Enum.Parse<TEnum>(name);
        return true;
    }
}
