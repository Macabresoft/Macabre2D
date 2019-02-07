namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a sprite with the content path and the Texture2D.
    /// </summary>
    [DataContract]
    public sealed class Sprite : IDisposable, IIdentifiable {
        private bool _disposedValue = false;

        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        public Sprite() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="contentManager">The content manager.</param>
        public Sprite(string contentPath, ContentManager contentManager) {
            this.ContentPath = contentPath;
            this.Texture = contentManager.Load<Texture2D>(contentPath);
            this.Size = new Point(this.Texture.Width, this.Texture.Height);
            this.Location = Point.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Macabre2D.Sprite"/> class.
        /// </summary>
        /// <param name="contentPath">Content path.</param>
        /// <param name="contentManager">Content manager.</param>
        /// <param name="location">The location of this specific sprite on the <see cref="Texture2D"/>.</param>
        /// <param name="size">The size of this specific sprite on the <see cref="Texture2D"/>.</param>
        public Sprite(string contentPath, ContentManager contentManager, Point location, Point size) {
            this.ContentPath = contentPath;
            this.Texture = contentManager.Load<Texture2D>(contentPath);
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for dynamic sprite creation and will not be properly
        /// saved to a scene as there will be no content path.
        /// </remarks>
        /// <param name="texture">The texture.</param>
        public Sprite(Texture2D texture) : this(texture, Point.Zero, new Point(texture.Width, texture.Height)) {
        }

        /// <summary> Initializes a new instance of the <see cref="Sprite"/> class. <remarks> This
        /// constructor should be used for dynamic sprite creation and will not be properly saved to
        /// a scene as there will be no content path. </remarks> <param name="texture">The
        /// texture.</param> <param name="location">The location of this specific sprite on the <see
        /// cref="Texture2D"/>.</param> <param name="size">The size of this specific sprite on the
        /// <see cref="Texture2D"/>.</param>
        public Sprite(Texture2D texture, Point location, Point size) {
            this.Texture = texture;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Gets or sets the content path.
        /// </summary>
        /// <value>The content path.</value>
        [DataMember]
        public string ContentPath { get; set; }

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember]
        public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        [DataMember]
        public Point Size { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>The texture.</value>
        internal Texture2D Texture { get; set; }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        private void Dispose(bool disposing) {
            if (!this._disposedValue) {
                if (disposing) {
                    this.Texture.Dispose();
                }

                this._disposedValue = true;
            }
        }
    }
}