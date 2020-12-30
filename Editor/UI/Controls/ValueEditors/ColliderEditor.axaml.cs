namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        private object _lock = new();
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
#pragma warning disable 4014
                this.CreateEditors();
#pragma warning restore 4014
            }
        }

        public async Task Initialize(IValueEditorService valueEditorService, IAssemblyService assemblyService) {
            this._valueEditorService = valueEditorService;

            var types = await assemblyService.LoadTypes(typeof(Collider));
            this._derivedTypes.Reset(types);

            if (this.Value != null) {
                await this.CreateEditors();
            }
        }

        private void ChildEditor_ValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.RaiseValueChanged(sender, e);
        }

        private async Task CreateEditors() {
            lock (this._lock) {
                foreach (var editor in this._childEditors) {
                    editor.ValueChanged -= this.ChildEditor_ValueChanged;
                }

                this._childEditors.Clear();
            }


            if (this.Value is Collider value && this._valueEditorService != null) {
                var childEditors = await this._valueEditorService.CreateEditors(value);

                lock (this._lock) {
                    foreach (var editor in childEditors) {
                        editor.ValueChanged += this.ChildEditor_ValueChanged;
                    }

                    Dispatcher.UIThread.Post(() => this._childEditors.Reset(childEditors), DispatcherPriority.Normal);
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}