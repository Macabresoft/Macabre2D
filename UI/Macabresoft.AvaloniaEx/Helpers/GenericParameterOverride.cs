namespace Macabresoft.AvaloniaEx;

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Resolution;

/// <summary>
/// A generic parameter override that simply tries to provide parameters in order.
/// </summary>
public class GenericParameterOverride : ResolverOverride, IResolve, IEquatable<ParameterInfo> {
    private readonly Queue<object> _parameters = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericParameterOverride" /> class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    public GenericParameterOverride(params object[] parameters) : base(null) {
        foreach (var parameter in parameters) {
            this._parameters.Enqueue(parameter);
        }
    }

    /// <inheritdoc />
    public bool Equals(ParameterInfo other) {
        return this._parameters.Count > 0;
    }

    /// <inheritdoc />
    public object Resolve<TContext>(ref TContext context) where TContext : IResolveContext {
        return this._parameters.TryDequeue(out var parameter) ? parameter : null;
    }
}