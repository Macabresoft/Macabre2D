namespace Macabre2D.Framework;

/// <summary>
/// A <see cref="SystemTypeReference{TSystem}" /> which will create the system if it is not found.
/// </summary>
/// <typeparam name="TSystem"></typeparam>
public class SystemTypeReferenceWithCreation<TSystem> : SystemTypeReference<TSystem> where TSystem : class, IGameSystem, new() {

    /// <inheritdoc />
    protected override TSystem? GetSystemFromScene(IScene scene) => scene.GetOrAddSystem<TSystem>();
}