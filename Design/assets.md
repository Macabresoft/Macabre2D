# Assets Design

## Content vs Asset

In Macabre2D, content will refer explicitly to the actual files being loaded into the game. This includes things like images, audio, and scenes.

Assets are a layer of abstraction above content. Assets are common utilizations of content. An example of assets built on top of an image would be sprite sheets, auto tilesets, and sprite animations. 

## Content Metadata

Every content file to be loaded will have a metadata file next to it in the Content hierarchy. These files will be formatted as such '{ContentName}.meta.m2d' where {ContentName} is the file name of the associated content without it's original file extension.

These metadata files will contain information to build the content and also a list of assets associated with the contnet.
