namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A interface for defining a scene that the <see cref="MacabreGame"/> can interact with.
    /// </summary>
    public interface IScene {

        /// <summary>
        /// Occurs when component added.
        /// </summary>
        event EventHandler<BaseComponent> ComponentAdded;

        /// <summary>
        /// Occurs when component removed.
        /// </summary>
        event EventHandler<BaseComponent> ComponentRemoved;

        /// <summary>
        /// Occurs when [module added].
        /// </summary>
        event EventHandler<BaseModule> ModuleAdded;

        /// <summary>
        /// Occurs when [module removed].
        /// </summary>
        event EventHandler<BaseModule> ModuleRemoved;

        /// <summary>
        /// Gets or sets the background color. Any areas not covered by sprites will be this color.
        /// </summary>
        /// <value>The background color.</value>
        Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets the game.
        /// </summary>
        /// <value>The game.</value>
        IGame Game { get; }

        /// <summary>
        /// Adds a component as a child.
        /// </summary>
        /// <returns><c>true</c>, if component was added, <c>false</c> otherwise.</returns>
        /// <param name="component">The component.</param>
        bool AddChild(BaseComponent component);

        /// <summary>
        /// Draws this scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        void Draw(GameTime gameTime);

        /// <summary>
        /// Draws all <see cref="IDrawableComponent"/> game components in the provided cameras.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="cameras">The cameras.</param>
        void Draw(GameTime gameTime, params ICamera[] cameras);

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
        /// Gets all components of the specified type that are tracked by this scene.
        /// </summary>
        /// <typeparam name="T">A component.</typeparam>
        /// <returns>All components of the specified type that are tracked by this scene.</returns>
        List<T> GetAllComponentsOfType<T>() where T : BaseComponent;

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
        /// <param name="game">The game.</param>
        void Initialize(IGame game);

        /// <summary>
        /// Loads the content.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Queues an action to be executed at the end of the next frame.
        /// </summary>
        /// <param name="action">The action.</param>
        void QueueEndOfFrameAction(Action<GameTime> action);

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
        void Update(GameTime gameTime);
    }
}