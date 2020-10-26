namespace Macabresoft.Macabre2D.Editor.Framework.MonoGame {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Linq;

    internal class EditorRenderSystem : UpdateSystem {
        private readonly QuadTree<IGameRenderableComponent> _renderTree = new QuadTree<IGameRenderableComponent>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        public override SystemLoop Loop => SystemLoop.Render;

        public override void Update(FrameTime frameTime, InputState inputState) {
            base.Update(frameTime, inputState);

            if (this.Scene.Game is ISceneEditor sceneEditor && sceneEditor.SpriteBatch != null && sceneEditor.Camera != null) {
                this._renderTree.Clear();

                foreach (var component in sceneEditor.SceneToEdit.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                foreach (var component in sceneEditor.Scene.RenderableComponents) {
                    this._renderTree.Insert(component);
                }

                var potentialRenderables = this._renderTree.RetrievePotentialCollisions(sceneEditor.Camera.BoundingArea);
                if (potentialRenderables.Any()) {
                    sceneEditor.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, sceneEditor.Camera.SamplerState, null, RasterizerState.CullNone, sceneEditor.Camera.Shader?.Effect, sceneEditor.Camera.ViewMatrix);

                    foreach (var component in potentialRenderables) {
                        // As long as it doesn't equal Layers.None, at least one of the layers
                        // defined on the component are also to be rendered by LayersToRender.
                        if ((component.Entity.Layers & sceneEditor.Camera.LayersToRender) != Layers.None) {
                            component.Render(frameTime, sceneEditor.Camera.BoundingArea);
                        }
                    }

                    sceneEditor.SpriteBatch.End();
                }
            }
        }
    }
}