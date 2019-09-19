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

---

# 0.1.2.0 and Earlier

I apologize, but changes were not well tracked prior to 0.2.0.0. Luckily for you, there's a 0% chance you were using Macabre2D at this time. It was barely functional and changed a lot!