namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Collider representing a rectangle to be used by the physics engine.
    /// </summary>
    [Display(Name = "Rectangle Collider")]
    public sealed class RectangleCollider : PolygonCollider {
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollider" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleCollider(float width, float height) : base(CreateVertices(width, height)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollider" /> class.
        /// </summary>
        public RectangleCollider() : this(1f, 1f) {
        }

        /// <summary>
        /// Gets or sets the height. Setting this is fairly expensive and should be avoided during
        /// runtime if possible.
        /// </summary>
        /// <value>The height.</value>
        public float Height {
            get {
                if (this.Vertices.Count == 4) {
                    return Math.Abs(this.Vertices[2].Y - this.Vertices[0].Y);
                }

                return 0;
            }

            set {
                if (value != this.Height && value > 0) {
                    var width = this.Width;
                    this.ResetVertices(CreateVertices(width, value));
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width. Setting this is fairly expensive and should be avoided during
        /// runtime if possible.
        /// </summary>
        /// <value>The width.</value>
        public float Width {
            get {
                if (this.Vertices.Count == 4) {
                    return Math.Abs(this.Vertices[0].X - this.Vertices[2].X);
                }

                return 0;
            }

            set {
                if (value != this.Width && value > 0) {
                    var height = this.Height;
                    this.ResetVertices(CreateVertices(value, height));
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        protected override List<Vector2> GetNormals() {
            var normals = new List<Vector2>();

            // Since half the edges are parallel, we can optimize and only get half the normals.
            for (var i = 0; i < 2; i++) {
                normals.Add(this.GetNormal(this.WorldPoints.ElementAt(i), this.GetNextWorldPoint(i)));
            }

            return normals;
        }

        private static IEnumerable<Vector2> CreateVertices(float width, float height) {
            var halfWidth = 0.5f * width;
            var halfHeight = 0.5f * height;

            return new[] {
                new Vector2(-halfWidth, -halfHeight),
                new Vector2(-halfWidth, halfHeight),
                new Vector2(halfWidth, halfHeight),
                new Vector2(halfWidth, -halfHeight)
            };
        }
    }
}