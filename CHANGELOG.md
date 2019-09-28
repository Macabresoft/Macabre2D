# 0.2.1.0

## Features

* Can change the way a property appears in the editor by altering its name in the [DataMember] attribute.
* When selecting a sprite, the dialog will automatically select the previously selected sprite when first opened.
* Can flip sprites and text vertically and horizontally.
* Save data manager added to handle saving and loading saves.
    * Currenty only works on Windows.

## Refactors

* Now uses the [DataMember] attribute's order and name instead of the [Display] attribute, which will clean up both the UI and serialized objects.
* The value editor for sprites has been cleaned up to match other asset selections. No longer gives a preview of the sprite, but the editor window is literally right there showing it.
* The title of bool editors is now part of the check box, allowing users to click the text to toggle the bool.

## Fixes

* First time assets now load properly instead of resetting to their default state (whoops).
* Assets now get synchronized properly with components when loading a scene.
    * Previously, if the scene wasn't initialized, no components would get updated.
* Selecting a sprite renderer in the component hierarchy will no longer result in the scene having changes to save.

## Compatibility Breaks

* The handling of [DataMember] attribute's name can cause objects created in previous versions to not deserialize properly going forward.


# 0.2.0.0

## Features

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

## Refactors

* Project settings and asset management have been split into two separate tabs.
* Removed ICamera interface as it was an unnecessary abstraction.
* Removed 'AssetType' from Assets.
* Removed 'Component' from the end of classes that didn't need that distinction to be understood.
* Module list now uses '+' and '-' buttons instead of 'Add' and 'Remove'.

## Fixes

* Layers names will no longer cause an exception during deserialization.
* Sprite Renderer now uses the sprite's size when determining offset instead of the full image file it comes from.
* Gizmos now work the same way regardless of 'pixels per unit'.
* A component's bounding area will stay up to date when changing the 'pixels per unit' in the editor.
* Zooming the camera in the editor is a lot smoother now as the zoom amount is based on the current view height and not a flat value.
* Fixed a crash that occurred when loading assets (mostly fonts) before the AssetManager had an instance.
* Fixed a crash that occurred when the start up scene identifier is null in a project during loading.
* Content building now works without magical DLL copying hacks (it was always broken, my bad).

---

# 0.1.2.0 and Earlier

I apologize, but changes were not well tracked prior to 0.2.0.0. Luckily for you, there's a 0% chance you were using Macabre2D at this time. It was barely functional and changed a lot!