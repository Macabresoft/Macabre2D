namespace Macabresoft.Macabre2D.UI.SceneEditor.Controls.ValueEditors {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Library.Models;
    using Macabresoft.Macabre2D.UI.Library.Services;
    using Macabresoft.Macabre2D.Framework;

    public class ColliderEditor : ValueEditorControl<Collider> {
        public static readonly DirectProperty<ColliderEditor, IReadOnlyCollection<Type>> DerivedTypesProperty =
            AvaloniaProperty.RegisterDirect<ColliderEditor, IReadOnlyCollection<Type>>(
                nameof(DerivedTypes),
                editor => editor.DerivedTypes);

        public static readonly DirectProperty<ColliderEditor, Type> SelectedTypeProperty =
            AvaloniaProperty.RegisterDirect<ColliderEditor, Type>(
                nameof(SelectedType),
                editor => editor.SelectedType,
                (editor, value) => editor.SelectedType = value);

        private readonly IAssemblyService _assemblyService = Resolver.Resolve<IAssemblyService>();

        private readonly HashSet<IValueEditor> _childEditors = new();
        private readonly ObservableCollectionExtended<Type> _derivedTypes = new();
        private readonly IValueEditorService _valueEditorService = Resolver.Resolve<IValueEditorService>();

        private ValueEditorCollection _editorCollection;
        private Type _selectedType;

        public ColliderEditor() {
            this.InitializeComponent();
        }

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

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            var types = this._assemblyService.LoadTypes(typeof(Collider));
            this._derivedTypes.Reset(types);
            this._derivedTypes.Remove(typeof(PolygonCollider));

            base.Initialize(value, valueType, valuePropertyName, title, owner);

            this.ResetColliderEditors();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(IValueEditor.Collection)) {
                this.ResetColliderEditors();
            }
        }

        protected override void OnValueChanged(Collider updatedValue) {
            base.OnValueChanged(updatedValue);
            this.ResetColliderEditors();
        }

        private void ClearEditors() {
            if (this._editorCollection != null) {
                this._editorCollection.OwnedValueChanged -= this.ColliderEditor_ValueChanged;
            }

            if (this._childEditors.Any()) {
                this.Collection.RemoveEditors(this._childEditors);
                this._childEditors.Clear();
                this._valueEditorService.ReturnEditors(this._editorCollection);
                this._editorCollection = null;
            }
        }

        private void ColliderEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.RaiseValueChanged(sender, e);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetColliderEditors() {
            this.ClearEditors();

            if (this._valueEditorService != null && this.Collection != null && this.Value is Collider value) {
                this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value.GetType());
                this._editorCollection = this._valueEditorService.CreateEditor(value, string.Empty);
                if (this._editorCollection != null) {
                    this._editorCollection.OwnedValueChanged += this.ColliderEditor_ValueChanged;
                    this._childEditors.Clear();
                    this._childEditors.AddRange(this._editorCollection.ValueEditors);
                    this.Collection.AddEditors(this._childEditors);
                }
            }
        }
    }
}