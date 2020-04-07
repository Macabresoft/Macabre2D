namespace Macabre2D.UI.Library.Controls {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Controls.ValueEditors;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    public partial class LayersNameEditor : UserControl {
        private readonly Dictionary<Layers, StringEditor> _layerToStringEditor = new Dictionary<Layers, StringEditor>();
        private IProjectService _projectService = ViewContainer.Resolve<IProjectService>();
        private IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public LayersNameEditor() {
            this.Loaded += this.LayersNameEditor_Loaded;
            this.InitializeComponent();
        }

        private void Layers_LayerNameChanged(object sender, LayerNameChangedEventArgs e) {
            if (this._layerToStringEditor.TryGetValue(e.Layer, out var editor)) {
                editor.Value = e.Name;
            }
        }

        private void LayersNameEditor_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            var layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().OrderBy(x => (ushort)x).ToList();
            layers.Remove(Layers.None);
            layers.Remove(Layers.All);

            foreach (var layer in layers) {
                var stringEditor = new StringEditor {
                    Title = layer.ToString(),
                    Value = GameSettings.Instance.Layers.GetLayerName(layer),
                    ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<string>>(x => this.StringEditorValueChanged(layer, x.NewValue), true)
                };

                this._layerToStringEditor[layer] = stringEditor;
                this._layerList.Items.Add(stringEditor);
            }

            GameSettings.Instance.Layers.LayerNameChanged += this.Layers_LayerNameChanged;
            this.Loaded -= this.LayersNameEditor_Loaded;
        }

        private void StringEditorValueChanged(Layers layer, string newName) {
            this.UpdateLayer(layer, newName);
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