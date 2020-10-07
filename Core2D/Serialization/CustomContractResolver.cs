namespace Macabresoft.MonoGame.Core2D {

    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A custom contract resolver for serializer.
    /// </summary>
    /// <seealso cref="DefaultContractResolver"/>
    public sealed class CustomContractResolver : DefaultContractResolver {

        /// <inheritdoc/>
        protected override JsonContract CreateContract(Type objectType) {
            if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))) {
                return this.CreateArrayContract(objectType);
            }

            return base.CreateContract(objectType);
        }
    }
}