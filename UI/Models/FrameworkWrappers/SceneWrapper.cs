namespace Macabre2D.UI.Models.FrameworkWrappers {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    public sealed class SceneWrapper : NotifyPropertyChanged, IParent<ComponentWrapper> {
        private readonly ObservableCollection<ComponentWrapper> _children = new ObservableCollection<ComponentWrapper>();
        private SceneAsset _sceneAsset;

        public SceneWrapper(SceneAsset sceneAsset) : this() {
            this.SceneAsset = sceneAsset;
            this.SceneAsset.PropertyChanged += this.SceneAsset_PropertyChanged;
            this.SceneAsset.Refresh(null);
            this.Load(this.SceneAsset.SavableValue as Scene);
        }

        public SceneWrapper(Scene scene) : this() {
            this.Load(scene);
        }

        private SceneWrapper() {
            this._children.CollectionChanged += this.Children_CollectionChanged;
            this.Modules.CollectionChanged += this.Modules_CollectionChanged;
        }

        public Color BackgroundColor {
            get {
                return this.Scene.BackgroundColor;
            }

            set {
                this.Scene.BackgroundColor = value;
                this.RaisePropertyChanged();
            }
        }

        public IReadOnlyCollection<ComponentWrapper> Children {
            get {
                return this._children;
            }
        }

        public ObservableCollection<ModuleWrapper> Modules { get; } = new ObservableCollection<ModuleWrapper>();

        public string Name {
            get {
                return this.SceneAsset != null ? Path.GetFileNameWithoutExtension(this.SceneAsset.Name) : "Unnamed Scene";
            }

            set {
                this.SceneAsset.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public Scene Scene { get; private set; } = new Scene();

        public SceneAsset SceneAsset {
            get {
                return this._sceneAsset;
            }

            internal set {
                this.Set(ref this._sceneAsset, value);
            }
        }

        public bool AddChild(ComponentWrapper child) {
            var result = false;
            if (!this._children.Any(x => x.Id == child.Id)) {
                this._children.Add(child);
                child.Parent = this;
                result = true;
            }

            return result;
        }

        public bool RemoveChild(ComponentWrapper child) {
            return this._children.Remove(child);
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var newItem in e.NewItems.OfType<ComponentWrapper>()) {
                    newItem.PropertyChanged += this.ComponentPropertyChanged;
                    this.Scene.AddComponent(newItem.Component);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var oldItem in e.OldItems.OfType<ComponentWrapper>()) {
                    oldItem.PropertyChanged -= this.ComponentPropertyChanged;
                    this.Scene.DestroyComponent(oldItem.Component);
                }
            }

            this.RaisePropertyChanged(nameof(this.Children));
        }

        private void ComponentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Children));
        }

        private void Load(Scene scene) {
            this.Scene = scene;

            foreach (var component in scene.ComponentsForSaving) {
                this.AddChild(new ComponentWrapper(component));
            }

            foreach (var module in scene.ModulesForSaving) {
                this.Modules.Add(new ModuleWrapper(module));
            }
        }

        private void ModulePropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Modules));
        }

        private void Modules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var newItem in e.NewItems.OfType<ModuleWrapper>()) {
                    newItem.PropertyChanged += this.ModulePropertyChanged;
                    this.Scene.AddModule(newItem.Module);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var oldItem in e.OldItems.OfType<ModuleWrapper>()) {
                    oldItem.PropertyChanged -= this.ModulePropertyChanged;
                    this.Scene.RemoveModule(oldItem.Module);
                }
            }

            this.RaisePropertyChanged(nameof(this.Modules));
        }

        private void SceneAsset_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.SceneAsset.Name)) {
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }
    }
}