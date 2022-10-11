using System;

namespace MusicLibrarySuite.CatalogService.Data.Extensions.Specialized;

/// <summary>
/// Provides a set of database-related extension methods for all types.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Casts an <see cref="object" /> value to the specified type.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="value">The value to cast.</param>
    /// <returns>
    /// The value casted to the <typeparamref name="T" /> type if it is neither a <see langword="null" /> reference nor the <see cref="DBNull.Value" /> object,
    /// a default value for the destination type otherwise.
    /// </returns>
    public static T? As<T>(this object? value)
    {
        return value is not null && value != DBNull.Value ? (T)value : default(T);
    }

    /// <summary>
    /// Processes a nullable value so it can be used as a database value.
    /// </summary>
    /// <param name="value">The nullable value to process.</param>
    /// <returns>The value itself if it is not a <see langword="null" /> reference, the <see cref="DBNull.Value" /> object otherwise.</returns>
    public static object AsDbValue(this object? value)
    {
        return value ?? DBNull.Value;
    }
}
