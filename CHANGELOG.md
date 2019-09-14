# 0.2.0.0

## Features

* Physics bodies can be marked as triggers.
    * A body marked as a trigger will simply notify when a collision occurs, but not react as if two bodies had just collided.
* A component can now be marked with multiple layers within the editor (previously this could only be done from code)
* Layers can now be given names that will appear in the editor instead of the basic enumeration names.
* Cameras can have shaders applied to them via MonoGame's Effects class.
* The Layer Collision Matrix can be edited to specify which layers collide with which layers in the physics system.

## Refactors

* Project settings and asset management have been split into two separate tabs.
* Removed ICamera interface as it was an unnecessary abstraction.
* Removed 'AssetType' from Assets.

## Fixes

* Fixed a bug which caused layer names to throw an exception during deserialization.

---

# 0.1.2.0 and Earlier

I apologize, but changes were not well tracked prior to 0.2.0.0. Luckily for you, there's a 0% chance you were using Macabre2D at this time. It was barely functional and changed a lot!