namespace Macabresoft.MonoGame.Core.Framework {

    using Macabresoft.Core;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an object which is a parent of <see cref="IGameEntity" />.
    /// </summary>
    public interface IGameEntityParent {

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IReadOnlyCollection<IGameEntity> Children { get; }

        /// <summary>
        /// Adds a child of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// An object that implements <see cref="IGameEntity" /> and has an empty constructor.
        /// </typeparam>
        /// <returns>The added child.</returns>
        T AddChild<T>() where T : IGameEntity, new();

        /// <summary>
        /// Adds a child of this entity's default child type.
        /// </summary>
        /// <returns>The added child.</returns>
        IGameEntity AddChild();

        /// <summary>
        /// Adds an existing <see cref="IGameEntity" /> as a child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AddChild(IGameEntity entity);

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A value indicating whether or not the child was removed.</returns>
        bool RemoveChild(IGameEntity entity);
    }

    /// <summary>
    /// A basic object which is a parent of <see cref="IGameEntity" />.
    /// </summary>
    [DataContract]
    public class GameEntityParent : PropertyChangedNotifier, IGameEntityParent {

        [DataMember]
        private readonly ObservableCollection<IGameEntity> _children = new ObservableCollection<IGameEntity>();

        /// <inheritdoc />
        public IReadOnlyCollection<IGameEntity> Children => this._children;

        /// <inheritdoc />
        public T AddChild<T>() where T : IGameEntity, new() {
            var entity = new T();
            this._children.Add(entity);
            this.OnAddChild(entity);
            return entity;
        }

        /// <inheritdoc />
        public IGameEntity AddChild() {
            return this.AddChild<GameEntity>();
        }

        /// <inheritdoc />
        public void AddChild(IGameEntity entity) {
            if (this.CanAddChild(entity)) {
                entity.Parent?.RemoveChild(entity);
                this._children.Add(entity);
                this.OnAddChild(entity);
            }
        }

        /// <inheritdoc />
        public bool RemoveChild(IGameEntity entity) {
            return this._children.Remove(entity);
        }

        /// <summary>
        /// Determines whether this instance can add the specified child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if this instance can add the specified child.; otherwise, <c>false</c>.</returns>
        protected virtual bool CanAddChild(IGameEntity entity) {
            return entity != null;
        }

        /// <summary>
        /// Posts the add child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void OnAddChild(IGameEntity entity) { return; }
    }
}