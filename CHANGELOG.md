# Change Log

## 0.3.12.0

### Features

* MacabreGame object now has GameSpeed property which can dictate how fast time dependent components and modules are running.

### Refactors

* Merged editor and framework back into a combined repository.
* Project autosaves now go into a special hidden folder.
* Time handled through new TimeFrame object, which will allow adjustments to game speed.

### Fixes

N/A

## 0.3.11.0

### Features

* Modules are no longer required to be updateable.
* Colliders can now override the layer of their parent.
* 'TileableBodyComponent' can override the layers of a collider based on which direction has the edge of a tile.

### Refactors

* Removed unused fields.
* Made 'BaseComponent' constructor protected.

## 0.3.10.0

### Refactors

* Removed dependency on .NET Framework in favor of using only .NET Standard 2.0.

## 0.3.9.0

### Features

* Added 'RandomTileMap' and 'RandomTileSet' to allow a binary tile map that chooses a random sprite from a weighted list.

### Fixes

* When a component is removed, it will now be both removed from the main collection of components and the collection for saving uninitialized components (just in case, this was causing issues in the editor).

## 0.3.8.0

### Features

* Added a component which draws every collider in the scene for diagnostic purposes.

### Refactors

* Raycast now directly uses layers provided as the layers it expects to hit.

### Fixes

* BoundingAreas now check for minimal difference between two floating point numbers instead of exact values. This fixes various physics interactions.

## 0.3.7.0

### Refactors

* Currently playing animation now exposed on SpriteAnimationComponents.

### Fixes

* SpriteAnimationComponent now runs its update synchronously so it does not have issues being altered in the middle of an update.

## 0.3.6.0

### Features

* Transform is now a struct.

### Refactors

* Pixel snapping now uses 'away from zero' rounding.
* Removing ISpriteRenderable as it was unused.
* Cleaned up some pixel snap code.
* Cleaned up transform code and reduced duplication.

## 0.3.5.0

### Fixes

* Raycasting from a 'SimplePhysicsModule' will now respect layers.

## 0.3.4.0

### Features

* 'SpriteAnimationComponent' can be inherited.
* 'SpriteAnimationComponent' can now have animations queued on it.

### Refactors

* Use of 'IsInitialized' on 'BaseComponent' has been reduced as it can cause confusion during inheritance.
* More components are appended with 'Component' at the end of their class name.
* 'SpriteAnimationComponent' no longer has a default animation on it and instead must have animations queued onto it.

## 0.3.3.0

### Refactors

* More internally accessed things have been made public.
* Repository link updated in NuGet package.

## 0.3.2.0

### Refactors

* Engine and Framework have been separated into different repositories.
* 'Component' appended to the end of drawer components.
* Several internally accessed things have been made public.

### Fixes

* Hiding inherited 'Components' property on 'MacabreGame' class as it is a MonoGame functionality not used by Macabre2D.

### Compatibility Breaks

* You now must install the League Mono font as described in the updated README file.

## 0.3.1.0

### Features

* Framework now uses a MonoGame PCL that can be used across multiple platforms.
* Framework is now a .NET Standard project that should be portable across platforms.
* Dialog for selecting an asset type or a component type now allows you to filter the available types by name.
* NuGet package created.

### Refactors

* UI tests separated out into their own project.
* Submodules removed in favor of NuGet packages for both MonoGame and WpfInterop.
* Ability to reload assets has been removed as it did not work and a solution may be too complicated.
    * Simply reload the editor if you need to reload assets.

### Fixes

* Changing project name and startup scene trigger the project as having changes.

## 0.3.0.0

### Features

* Can change the way a property appears in the editor by altering its name in the [DataMember] attribute.
* When selecting a sprite, the dialog will automatically select the previously selected sprite when first opened.
* Can flip sprites and text vertically and horizontally.
* Save data manager added to handle saving and loading saves.
    * Currently only works on Windows.
* Projects are now auto-saved at a regular interval.
* Crashes are handled more gracefully.
    * If there is an error, it will be logged.
	* Settings will be saved, if possible.
* Can select an auto-save if the editor was shutdown incorrectly last session.
* Custom skull icon now used.
* Graphics settings can be saved, loaded, and applied.

### Refactors

* Now uses the [DataMember] attribute's order and name instead of the [Display] attribute, which will clean up both the UI and serialized objects.
* The value editor for sprites has been cleaned up to match other asset selections. No longer gives a preview of the sprite, but the editor window is literally right there showing it.
* The title of bool editors is now part of the check box, allowing users to click the text to toggle the bool.
* Serializer can now be accessed through a singleton instance, similar to game settings and asset manager.
* Provided components all have an order and name to their serializable properties where necessary.
* 'SceneWrapper' class has been merged with 'SceneAsset'.
* Metadata assets no longer require a metadata file (it is stored in the project file).
* Cleaned up image asset editor, margins on assets, and margins on modules.

### Fixes

* First time assets now load properly instead of resetting to their default state (whoops).
* Assets now get synchronized properly with components when loading a scene.
    * Previously, if the scene wasn't initialized, no components would get updated.
* Selecting a sprite renderer in the component hierarchy will no longer result in the scene having changes to save.
* Audio player component no longer causes a crash in the editor.
* Assets not loaded through the content pipeline no longer require physical files.
* All asset types are properly synced.
    * This means that when editing auto tile sets and sprite animations, auto tile maps and sprite animators will be updated appropriately.
* Auto tile sets now sync properly with their sprites in the editor.
* Content should build in both the editor and the game project.

### Compatibility Breaks

* The handling of [DataMember] attribute's name can cause objects created in previous versions to not deserialize properly going forward.


## 0.2.0.0

### Features

* Physics bodies can be marked as triggers.
    * A body marked as a trigger will simply notify when a collision occurs, but not react as if two bodies had just collided.
* A component can now be marked with multiple layers within the editor (previously this could only be done from code)
* Layers can now be given names that will appear in the editor instead of the basic enumeration names.
* Cameras can have shaders applied to them via MonoGame's Effects class.
* The Layer Collision Matrix can be edited to specify which layers collide with which layers in the physics system.
* A draggable splash screen shows when the project is loaded on startup.
* There are now 16 layers instead of 8.
* The sampler state can be changed on a camera for a sprite batch.
	* This allows for crisp pixel art.
* Camera can be snapped to pixels for pixel art games.
* Sprite Renderer can be snapped to pixels for pixel art games.
* Can edit processor parameters 'Texture Format' and 'Premultiply Alpha' for font assets.
* Text Renderer can be snapped to pixels for pixel art games.

### Refactors

* Project settings and asset management have been split into two separate tabs.
* Removed ICamera interface as it was an unnecessary abstraction.
* Removed 'AssetType' from Assets.
* Removed 'Component' from the end of classes that didn't need that distinction to be understood.
* Module list now uses '+' and '-' buttons instead of 'Add' and 'Remove'.

### Fixes

* Layers names will no longer cause an exception during deserialization.
* Sprite Renderer now uses the sprite's size when determining offset instead of the full image file it comes from.
* Gizmos now work the same way regardless of 'pixels per unit'.
* A component's bounding area will stay up to date when changing the 'pixels per unit' in the editor.
* Zooming the camera in the editor is a lot smoother now as the zoom amount is based on the current view height and not a flat value.
* Fixed a crash that occurred when loading assets (mostly fonts) before the AssetManager had an instance.
* Fixed a crash that occurred when the start up scene identifier is null in a project during loading.
* Content building now works without magical DLL copying hacks (it was always broken, my bad).

---

## 0.1.2.0 and Earlier

I apologize, but changes were not well tracked prior to 0.2.0.0. Luckily for you, there's a 0% chance you were using Macabre2D at this time. It was barely functional and changed a lot!