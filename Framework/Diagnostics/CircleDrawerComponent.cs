﻿using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Draws a circle.
    /// </summary>
    [Display(Name = "Circle Drawer (Diagnostics)")]
    public sealed class CircleDrawerComponent : BaseDrawerComponent {
        private int _complexity;

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(-this.Radius, this.Radius);
            }
        }

        /// <summary>
        /// Gets or sets the complexity. This value determines the smoothness of the circle's edges.
        /// A larger value will look better, but perform worse.
        /// </summary>
        /// <remarks>
        /// Complexity is code for 'number of edges'. In reality, we can't make a perfect circle
        /// with pixels or polygons, so this is us faking it as usual. This value must be at least 3.
        /// </remarks>
        /// <value>The complexity.</value>
        [DataMember(Order = 3)]
        public int Complexity {
            get {
                return this._complexity;
            }

            set {
                if (value < 3) {
                    value = 3;
                }

                this.Set(ref this._complexity, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; set; }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.Radius > 0f && this.PrimitiveDrawer != null && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                this.PrimitiveDrawer.DrawCircle(
                    spriteBatch, 
                    this.Entity.Scene.Game.Project.Settings.PixelsPerUnit, 
                    this.Radius, 
                    this.Entity.Transform.Position,
                    this.Complexity, 
                    this.Color,
                    lineThickness);
            }
        }
    }
}