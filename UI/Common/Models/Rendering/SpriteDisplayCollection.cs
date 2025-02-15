﻿namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;

/// <summary>
/// A collection of <see cref="SpriteDisplayModel" />.
/// </summary>
public class SpriteDisplayCollection : PropertyChangedNotifier, IReadOnlyCollection<SpriteDisplayModel> {
    private readonly IImage _bitmap;
    private readonly ContentFile _file;
    private readonly ObservableCollectionExtended<SpriteDisplayModel> _sprites = new();
    private readonly SpriteSheet _spriteSheet;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteDisplayCollection" /> class.
    /// </summary>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="file">The file.</param>
    public SpriteDisplayCollection(SpriteSheet spriteSheet, ContentFile file) {
        this._spriteSheet = spriteSheet;
        this._file = file;

        var fileInfo = new FileInfo(file.GetFullPath());
        if (fileInfo.Exists) {
            this._bitmap = new Bitmap(fileInfo.FullName);
            this.ResetSprites();
        }

        this._spriteSheet.PropertyChanged += this.SpriteSheet_PropertyChanged;
        this._file.PropertyChanged += this.File_PropertyChanged;
    }

    /// <inheritdoc />
    public int Count => this._sprites.Count;

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string Name => this._file.Name;

    /// <summary>
    /// Gets the size of the sprite sheet.
    /// </summary>
    public Size Size => this._bitmap.Size;

    /// <summary>
    /// Gets the sprites.
    /// </summary>
    public IReadOnlyCollection<SpriteDisplayModel> Sprites => this._sprites;

    /// <inheritdoc />
    public IEnumerator<SpriteDisplayModel> GetEnumerator() => this._sprites.GetEnumerator();

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();

        this._sprites.Clear();
        this._spriteSheet.PropertyChanged -= this.SpriteSheet_PropertyChanged;
        this._file.PropertyChanged -= this.File_PropertyChanged;
    }

    private void File_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ContentFile.Name)) {
            this.RaisePropertyChanged(nameof(this.Name));
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    private void ResetSprites() {
        var sprites = new List<SpriteDisplayModel>();
        if (this._bitmap != null) {
            byte index = 0;
            for (var y = 0; y < this._spriteSheet.Rows; y++) {
                for (var x = 0; x < this._spriteSheet.Columns; x++) {
                    sprites.Add(new SpriteDisplayModel(this._bitmap, index, this._spriteSheet));
                    index++;
                }
            }
        }

        this._sprites.Reset(sprites);
    }

    private void SpriteSheet_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(SpriteSheet.Columns) or nameof(SpriteSheet.Rows)) {
            this.ResetSprites();
        }
    }
}