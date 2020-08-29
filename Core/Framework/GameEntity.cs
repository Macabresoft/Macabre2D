namespace Macabresoft.MonoGame.Core {

    using System;

    /// <summary>
    /// Interface for a <see cref="ITransformable" /> descendent of <see cref="IScene" /> which
    /// holds a collection of <see cref="IGameComponent" />.
    /// </summary>
    public interface IGameEntity : ITransformable {
    }

    /// <summary>
    /// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a collection
    /// of <see cref="IGameComponent" />
    /// </summary>
    public sealed class GameEntity : ITransformable {

        /// <inheritdoc />
        public Transform Transform => throw new NotImplementedException();
    }
}