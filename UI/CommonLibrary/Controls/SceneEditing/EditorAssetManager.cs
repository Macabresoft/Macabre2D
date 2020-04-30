namespace Macabre2D.UI.CommonLibrary.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using System;

    internal sealed class EditorAssetManager : IAssetManager {
        private readonly IAssetManager _assetManager;

        public EditorAssetManager(IAssetManager assetManager) {
            this._assetManager = assetManager;
        }

        public void ClearMappings() {
            this._assetManager.ClearMappings();
        }

        public Guid GetId(string path) {
            return this._assetManager.GetId(path);
        }

        public string GetPath(Guid id) {
            return this._assetManager.GetPath(id);
        }

        public void Initialize(ContentManager contentManager) {
            this._assetManager.Initialize(contentManager);
            AssetManager.Instance = this;
        }

        public T Load<T>(string path) {
            T result;
            if (this._assetManager != null && typeof(T) != typeof(SoundEffect)) {
                result = this._assetManager.Load<T>(path);
            }
            else {
                result = default;
            }

            return result;
        }

        public T Load<T>(Guid id) {
            T result;
            if (this._assetManager != null && typeof(T) != typeof(SoundEffect)) {
                result = this._assetManager.Load<T>(id);
            }
            else {
                result = default;
            }

            return result;
        }

        public void SetMapping(Guid id, string contentPath) {
            this._assetManager.SetMapping(id, contentPath);
        }

        public void Unload() {
            this._assetManager.Unload();
        }
    }
}