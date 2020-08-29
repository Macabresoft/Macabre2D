# Architecture Overview

## GameLoop

The `GameLoop` class is an extension of MonoGame's `Game` class. This class simply performs the basic game loop on the currently loaded scene, provides access to the `GameSettings` class, and terminates the game upon exit. The `GameLoop` does not have access to `GameEntity` and only interacts with `GameScene` by telling it to update and render.

## GameEntity

The `GameEntity` class is a fairly bare-bones object that stores transform information, a parent (either a `GameEntity` or a `GameScene`), and a list of children (these must be of type `GameEntity`). If a game entity has any updateables as children, it will update them in the order they are arranged in its children list. This update loop is independent of all other instances of `GameEntity` and should only interact with itself.

## GameService

The `GameService` class is a direct child of the `GameScene` class. When the scene's update loop is called, it will go through all the services underneath it and call their update. For instance, a `RenderService` will handle the draw calls for entities in the scene. An `UpdateService` will tell every entity to update. A `PhysicsService` will detect any collisions and react to them.

## GameComponent

The `GameComponent` class serves as a child of the `GameEntity` class and operates within the domain of that single `GameEntity`. Components can have updates, draw calls, store data, and perform any custom function a user can think up as long as it operates on the domain of a single entity. Nothing stops components from operating on other components, but the asynchronous nature of the default update makes this a lot more dangerous and is therefore heavily discouraged.