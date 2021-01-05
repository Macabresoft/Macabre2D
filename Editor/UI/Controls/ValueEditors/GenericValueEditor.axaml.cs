namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System.Collections.Generic;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    public class GenericValueEditor : ValueEditorControl<object>, IParentValueEditor {
        public static readonly DirectProperty<GenericValueEditor, IReadOnlyCollection<IValueEditor>> ChildEditorsProperty =
            AvaloniaProperty.RegisterDirect<GenericValueEditor, IReadOnlyCollection<IValueEditor>>(
                nameof(ChildEditors),
                editor => editor.ChildEditors);

        private readonly ObservableCollectionExtended<IValueEditor> _childEditors = new();
        private IValueEditorService _valueEditorService;

        public GenericValueEditor() {
            this.InitializeComponent();
        }

        public IReadOnlyCollection<IValueEditor> ChildEditors => this._childEditors;

        public void Initialize(IValueEditorService valueEditorService, IAssemblyService assemblyService) {
            this._valueEditorService = valueEditorService;

            if (this.Value != null) {
                this.CreateEditors();
            }
        }

        protected override void OnValueChanged(object updatedValue) {
            base.OnValueChanged(updatedValue);

            if (updatedValue != null) {
                this.CreateEditors();
            }
            else {
                this.ClearEditors();
            }
        }

        private void ChildEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.RaiseValueChanged(sender, e);
        }

        private void ClearEditors() {
            foreach (var editor in this._childEditors) {
                editor.ValueChanged -= this.ChildEditor_ValueChanged;
            }

            this._childEditors.Clear();
        }

        private void CreateEditors() {
            this.ClearEditors();

            if (this.Value is Collider value && this._valueEditorService != null) {
                var childEditors = this._valueEditorService.CreateEditors(value);

                foreach (var editor in childEditors) {
                    editor.ValueChanged += this.ChildEditor_ValueChanged;
                }

                Dispatcher.UIThread.Post(() => this._childEditors.Reset(childEditors));
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}