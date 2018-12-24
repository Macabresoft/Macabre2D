namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Physics;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class ColliderEditor : NamedValueEditor<Collider> {

        public static readonly DependencyProperty ColliderTypesProperty = DependencyProperty.Register(
            nameof(ColliderTypes),
            typeof(ICollection<Type>),
            typeof(ColliderEditor),
            new PropertyMetadata(new List<Type>()));

        private readonly IBusyService _busyService;
        private readonly IValueEditorService _valueEditorService;
        private DependencyObject _editor;
        private Type _selectedColliderType;

        public ColliderEditor() {
            this._busyService = ViewContainer.Resolve<IBusyService>();
            this._valueEditorService = ViewContainer.Resolve<IValueEditorService>();
            this.InitializeComponent();
        }

        public ICollection<Type> ColliderTypes {
            get { return (ICollection<Type>)this.GetValue(ColliderTypesProperty); }
            set { this.SetValue(ColliderTypesProperty, value); }
        }

        public DependencyObject Editor {
            get {
                return this._editor;
            }

            set {
                this._editor = value;
                this.RaisePropertyChanged(nameof(this.Editor));
            }
        }

        public Type SelectedColliderType {
            get {
                return this._selectedColliderType;
            }

            set {
                if (this._selectedColliderType != value) {
                    this._selectedColliderType = value;

                    if (value == null) {
                        this.Value = null;
                    }
                    else if (this.Value?.GetType() != value) {
                        if (value == typeof(LineCollider)) {
                            this.Value = new LineCollider(Vector2.Zero, Vector2.One);
                        }
                        else if (value == typeof(RectangleCollider)) {
                            this.Value = new RectangleCollider(1f, 1f);
                        }
                        else {
                            this.Value = Activator.CreateInstance(this._selectedColliderType) as Collider;
                        }
                    }

                    this.RaisePropertyChanged(nameof(this.SelectedColliderType));
                }
            }
        }

        protected override void OnValueChanged(Collider newValue, Collider oldValue, DependencyObject d) {
            if (newValue != null) {
                this.SelectedColliderType = newValue.GetType();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        protected override async Task OnValueChangedAsync(Collider newValue, Collider oldValue, DependencyObject d) {
            if (newValue != null) {
                var task = this.GetEditor(newValue);
                await this._busyService.PerformTask(task);
            }
            else {
                this.Editor = null;
            }
        }

        private async Task GetEditor(object value) {
            this.Editor = await this._valueEditorService.CreateEditor(value, string.Empty, typeof(BaseComponent));
        }
    }
}