namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A interface for defining a scene that the <see cref="MacabreGame"/> can interact with.
    /// </summary>
    public interface IScene : IAsset {

        /// <summary>
        /// Occurs when a component has been created or added to the scene for the first time.
        /// </summary>
        event EventHandler<BaseComponent> ComponentCreated;

        /// <summary>
        /// Occurs when a component has been destroyed and completely removed from the scene.
        /// </summary>
        event EventHandler<BaseComponent> ComponentDestroyed;

        /// <summary>
        /// Occurs when a module is added.
        /// </summary>
        event EventHandler<BaseModule> ModuleAdded;

        /// <summary>
        /// Occurs when a module is removed.
        /// </summary>
        event EventHandler<BaseModule> ModuleRemoved;

        /// <summary>
        /// Gets or sets the background color. Any areas not covered by sprites will be this color.
        /// </summary>
        /// <value>The background color.</value>
        Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IReadOnlyCollection<BaseComponent> Components { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Adds a component as a child of this scene.
        /// </summary>
        /// <typeparam name="T">A component type.</typeparam>
        /// <returns>The added component.</returns>
        T AddComponent<T>() where T : BaseComponent, new();

        /// <summary>
        /// Adds a component as a child of this scene.
        /// </summary>
        /// <returns><c>true</c>, if component was added, <c>false</c> otherwise.</returns>
        /// <param name="component">The component.</param>
        bool AddComponent(BaseComponent component);

        /// <summary>
        /// Adds the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>A value indicating whether or not the module was added.</returns>
        bool AddModule(BaseModule module);

        /// <summary>
        /// Adds the fixed time step module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="timeStep">The time step.</param>
        /// <returns>A value indicating whether or not the module was added.</returns>
        bool AddModule(FixedTimeStepModule module, float timeStep);

        /// <summary>
        /// Creates a module of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of module.</typeparam>
        /// <returns>The added module.</returns>
        T CreateModule<T>() where T : BaseModule, new();

        /// <summary>
        /// Creates a fixed time step module of the specified type.
        /// </summary>
        /// <typeparam name="T">A type of module with a fixed time step.</typeparam>
        /// <param name="timeStep">The time step.</param>
        /// <returns>The added module.</returns>
        T CreateModule<T>(float timeStep) where T : FixedTimeStepModule, new();

        /// <summary>
        /// Destroys the component.
        /// </summary>
        /// <param name="component">The component.</param>
        void DestroyComponent(BaseComponent component);

        /// <summary>
        /// Draws this scene.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        void Draw(FrameTime frameTime);

        /// <summary>
        /// Draws all <see cref="IDrawableComponent"/> game components in the provided cameras.
        /// </summary>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="cameras">The cameras.</param>
        void Draw(FrameTime frameTime, params Camera[] cameras);

        /// <summary>
        /// Finds a component in the scene with the specified name. If multiple components have the
        /// same name, only one will be returned and there is no guarantee that it will be the same
        /// component every time.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="name">Name.</param>
        BaseComponent FindComponent(string name);

        /// <summary>
        /// Finds a component in the scene with the specified type. If multiple components with the
        /// same type exist, only one will be returned and there is no guarantee that it will be the
        /// same component every time.
        /// </summary>
        /// <returns>The component of type.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T FindComponentOfType<T>() where T : BaseComponent;

        /// <summary>
        /// Finds the module.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The module.</returns>
        BaseModule FindModule(string name);

        /// <summary>
        /// Gets all components of the specified type that are tracked by this scene.
        /// </summary>
        /// <typeparam name="T">A component.</typeparam>
        /// <returns>All components of the specified type that are tracked by this scene.</returns>
        List<T> GetAllComponentsOfType<T>();

        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <typeparam name="T">A type of module.</typeparam>
        /// <returns>The module of specified type.</returns>
        T GetModule<T>() where T : BaseModule;

        /// <summary>
        /// Gets the modules.
        /// </summary>
        /// <typeparam name="T">A type of module.</typeparam>
        /// <returns>All modules of the specified type.</returns>
        IEnumerable<T> GetModules<T>() where T : BaseModule;

        /// <summary>
        /// Gets the visible drawable components.
        /// </summary>
        /// <returns>The visible drawable components.</returns>
        IEnumerable<IDrawableComponent> GetVisibleDrawableComponents();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads the content.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Queues an action to be executed at the end of the next frame.
        /// </summary>
        /// <param name="action">The action.</param>
        void QueueEndOfFrameAction(Action<FrameTime> action);

        /// <summary>
        /// Removes the module.
        /// </summary>
        /// <param name="module">The module.</param>
        void RemoveModule(BaseModule module);

        /// <summary>
        /// Resolves the dependency.
        /// </summary>
        /// <typeparam name="T">The type of the dependency.</typeparam>
        /// <returns>The dependency if it already exists or a newly created dependency.</returns>
        T ResolveDependency<T>() where T : new();

        /// <summary>
        /// Resolves the dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectFactory">The object factory.</param>
        /// <returns>
        /// The dependency if it already exists or a dependency created from the provided factory.
        /// </returns>
        T ResolveDependency<T>(Func<T> objectFactory);

        /// <summary>
        /// Updates this scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Update(FrameTime frameTime);
    }
}