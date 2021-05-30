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
        private ValueEditorCollection _editorCollection;
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
            if (this._editorCollection != null) {
                this._editorCollection.OwnedValueChanged -= this.ChildEditor_ValueChanged;
                this._valueEditorService.ReturnEditors(this._editorCollection);
                this._editorCollection = null;
            }

            this._childEditors.Clear();
        }

        private void CreateEditors() {
            this.ClearEditors();

            if (this.Value is Collider value && this._valueEditorService != null) {
                this._editorCollection = this._valueEditorService.CreateEditor(value, string.Empty);

                if (this._editorCollection != null) {
                    this._editorCollection.OwnedValueChanged += this.ChildEditor_ValueChanged;
                    Dispatcher.UIThread.Post(() => this._childEditors.Reset(this._editorCollection.ValueEditors));
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}