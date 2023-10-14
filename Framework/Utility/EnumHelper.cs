namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Helper methods for <see cref="Enum" /> implementations.
/// </summary>
public static class EnumHelper {
    /// <summary>
    /// Tries to get all values for a flags enum.
    /// </summary>
    /// <param name="enumType">The enum type.</param>
    /// <param name="result">The resulting enum.</param>
    /// <returns>A value indicating whether or not all values could be gotten.</returns>
    public static bool TryGetAll(Type enumType, [NotNullWhen(true)] out object? result) {
        if (!enumType.IsEnum) {
            result = null;
        }
        else {
            try {
                var enums = Enum.GetValues(enumType).Cast<object>().Select(Convert.ToInt32);
                var all = enums.Aggregate(0, (current, value) => current | value);
                result = Enum.Parse(enumType, all.ToString());
            }
            catch {
                result = null;
            }
        }

        return result != null;
    }

    /// <summary>
    /// Gets the zero value for a <see cref="Enum" />.
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="result"></param>
    /// <returns>A value indicating whether or not a none value could be gotten.</returns>
    public static bool TryGetZero(Type enumType, [NotNullWhen(true)] out object? result) {
        if (!enumType.IsEnum) {
            result = null;
        }
        else {
            try {
                result = Enum.Parse(enumType, 0.ToString());
            }
            catch {
                result = null;
            }
        }

        return result != null;
    }
}