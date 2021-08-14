namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;

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

        private readonly HashSet<IValueControl> _childEditors = new();
        private readonly ObservableCollectionExtended<Type> _derivedTypes = new();
        private readonly IValueControlService _valueControlService = Resolver.Resolve<IValueControlService>();

        private ValueControlCollection _controlCollection;
        private Type _selectedType;

        public ColliderEditor() {
            this.InitializeComponent();
        }

        public IReadOnlyCollection<Type> DerivedTypes => this._derivedTypes;

        public Type SelectedType {
            get => this._selectedType;
            set {
                if (value != null) {
                    this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value);
                    Dispatcher.UIThread.Post(() => { this.SetEditorValue(this.Value, Activator.CreateInstance(value) as Collider); });
                }
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

        protected override void OnValueChanged() {
            base.OnValueChanged();
            this.ResetColliderEditors();
        }

        private void ClearEditors() {
            if (this._controlCollection != null) {
                this._controlCollection.OwnedValueChanged -= this.ColliderControlValueChanged;
            }

            if (this._childEditors.Any()) {
                this.Collection.RemoveControls(this._childEditors);
                this._childEditors.Clear();
                this._valueControlService.ReturnControls(this._controlCollection);
                this._controlCollection = null;
            }
        }

        private void ColliderControlValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.RaiseValueChanged(sender, e);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetColliderEditors() {
            try {
                this.IgnoreUpdates = true;
                this.ClearEditors();

                if (this._valueControlService != null && this.Collection != null && this.Value is Collider value) {
                    this.SetAndRaise(SelectedTypeProperty, ref this._selectedType, value.GetType());
                    this._controlCollection = this._valueControlService.CreateControl(value, string.Empty);
                    if (this._controlCollection != null) {
                        this._controlCollection.OwnedValueChanged += this.ColliderControlValueChanged;
                        this._childEditors.Clear();
                        this._childEditors.AddRange(this._controlCollection.ValueControls);
                        this.Collection.AddControls(this._childEditors);
                    }
                }
            }
            finally {
                this.IgnoreUpdates = false;
            }
        }
    }
}