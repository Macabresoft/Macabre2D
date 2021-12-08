namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

/// <summary>
/// A custom contract resolver for serializer.
/// </summary>
/// <seealso cref="DefaultContractResolver" />
public sealed class CustomContractResolver : DefaultContractResolver {
    /// <inheritdoc />
    protected override JsonContract CreateContract(Type objectType) {
        if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) || i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))) {
            return this.CreateArrayContract(objectType);
        }

        return base.CreateContract(objectType);
    }
}