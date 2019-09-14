namespace Macabre2D.UI.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Windows.Controls;

    public partial class LayersNameEditor : UserControl, INotifyPropertyChanged {
        private IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public LayersNameEditor() {
            this.Loaded += this.LayersNameEditor_Loaded;
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Layer01Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer01);
            }

            set {
                this.UpdateLayer(Layers.Layer01, value);
            }
        }

        public string Layer02Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer02);
            }

            set {
                this.UpdateLayer(Layers.Layer02, value);
            }
        }

        public string Layer03Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer03);
            }

            set {
                this.UpdateLayer(Layers.Layer03, value);
            }
        }

        public string Layer04Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer04);
            }

            set {
                this.UpdateLayer(Layers.Layer04, value);
            }
        }

        public string Layer05Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer05);
            }

            set {
                this.UpdateLayer(Layers.Layer05, value);
            }
        }

        public string Layer06Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer06);
            }

            set {
                this.UpdateLayer(Layers.Layer06, value);
            }
        }

        public string Layer07Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer07);
            }

            set {
                this.UpdateLayer(Layers.Layer07, value);
            }
        }

        public string Layer08Name {
            get {
                return GameSettings.Instance.Layers.GetLayerName(Layers.Layer08);
            }

            set {
                this.UpdateLayer(Layers.Layer08, value);
            }
        }

        private void LayersNameEditor_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this.Loaded -= this.LayersNameEditor_Loaded;
        }

        private void ProjectService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Layer01Name));
            this.RaisePropertyChanged(nameof(this.Layer02Name));
            this.RaisePropertyChanged(nameof(this.Layer03Name));
            this.RaisePropertyChanged(nameof(this.Layer04Name));
            this.RaisePropertyChanged(nameof(this.Layer05Name));
            this.RaisePropertyChanged(nameof(this.Layer06Name));
            this.RaisePropertyChanged(nameof(this.Layer07Name));
            this.RaisePropertyChanged(nameof(this.Layer08Name));
        }

        private void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateLayer(Layers layer, string name) {
            var originalName = GameSettings.Instance.Layers.GetLayerName(layer);
            var originalHasChanges = this._projectService.HasChanges;

            var undoCommand = new UndoCommand(() => {
                GameSettings.Instance.Layers.SetLayerName(layer, name);
                this._projectService.HasChanges = true;
            }, () => {
                GameSettings.Instance.Layers.SetLayerName(layer, originalName);
                this._projectService.HasChanges = originalHasChanges;
            });

            this._undoService.Do(undoCommand);
        }
    }
}