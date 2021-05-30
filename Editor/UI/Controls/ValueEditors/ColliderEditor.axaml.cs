namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Collections.Generic;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    public class ColliderEditor : ValueEditorControl<Collider>, IParentValueEditor {
        public static readonly DirectProperty<ColliderEditor, IReadOnlyCollection<IValueEditor>> ChildEditorsProperty =
            AvaloniaProperty.RegisterDirect<ColliderEditor, IReadOnlyCollection<IValueEditor>>(
                nameof(ChildEditors),
                editor => editor.ChildEditors);

        public static readonly DirectProperty<ColliderEditor, IReadOnlyCollection<Type>> DerivedTypesProperty =
            AvaloniaProperty.RegisterDirect<ColliderEditor, IReadOnlyCollection<Type>>(
                nameof(DerivedTypes),
                editor => editor.DerivedTypes);

        public static readonly DirectProperty<ColliderEditor, Type> SelectedTypeProperty =
            AvaloniaProperty.RegisterDirect<ColliderEditor, Type>(
                nameof(SelectedType),
                editor => editor.SelectedType,
                (editor, value) => editor.SelectedType = value);

        private readonly ObservableCollectionExtended<IValueEditor> _childEditors = new();
        private readonly ObservableCollectionExtended<Type> _derivedTypes = new();

        private ValueEditorCollection _editorCollection;
        private Type _selectedType;
        private IValueEditorService _valueEditorService;

        public ColliderEditor() {
            this.InitializeComponent();
        }

        public IReadOnlyCollection<IValueEditor> ChildEditors => this._childEditors;

        public IReadOnlyCollection<Type> DerivedTypes => this._derivedTypes;

        public Type SelectedType {
            get => this._selectedType;
            set {
                this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value);
                Dispatcher.UIThread.Post(() => {
                    if (value != null) {
                        this.SetEditorValue(this.Value, Activator.CreateInstance(value) as Collider);
                    }
                    else {
                        this.SetEditorValue(this.Value, null);
                    }
                });
            }
        }

        public void Initialize(IValueEditorService valueEditorService, IAssemblyService assemblyService) {
            this._valueEditorService = valueEditorService;

            var types = assemblyService.LoadTypes(typeof(Collider));
            this._derivedTypes.Reset(types);
            this._derivedTypes.Remove(typeof(PolygonCollider));

            if (this.Value != null) {
                this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, this.Value.GetType());
                this.CreateEditors();
            }
        }

        protected override void OnValueChanged(Collider updatedValue) {
            base.OnValueChanged(updatedValue);

            if (updatedValue != null) {
                this.CreateEditors();
            }
            else {
                this.ClearEditors();
            }
        }

        private void ClearEditors() {
            if (this._editorCollection != null) {
                this._editorCollection.OwnedValueChanged -= this.ColliderEditor_ValueChanged;
                this._valueEditorService.ReturnEditors(this._editorCollection);
                this._editorCollection = null;
            }

            this._childEditors.Clear();
        }

        private void ColliderEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.RaiseValueChanged(sender, e);
        }

        private void CreateEditors() {
            this.ClearEditors();

            if (this.Value is Collider value && this._valueEditorService != null) {
                this._editorCollection = this._valueEditorService.CreateEditor(value, string.Empty);
                if (this._editorCollection != null) {
                    this._editorCollection.OwnedValueChanged += this.ColliderEditor_ValueChanged;
                    Dispatcher.UIThread.Post(() => this._childEditors.Reset(this._editorCollection.ValueEditors));
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}