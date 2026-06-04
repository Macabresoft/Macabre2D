namespace Macabre2D.Framework;

/// <summary>
/// A <see cref="SceneSystemTypeReference{TSystem}" /> which will create the system if it is not found.
/// </summary>
/// <typeparam name="TSystem"></typeparam>
public class SceneSystemTypeReferenceWithCreation<TSystem> : SceneSystemTypeReference<TSystem> where TSystem : class, ISceneSystem, new() {

    /// <inheritdoc />
    protected override TSystem? GetSystemFromScene(IScene scene) => scene.GetOrAddSystem<TSystem>();
}