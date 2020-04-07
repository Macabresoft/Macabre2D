namespace Macabre2D.UI.Library.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class ColliderEditor : NamedValueEditor<Collider>, ISeparatedValueEditor {

        public static readonly DependencyProperty ColliderTypesProperty = DependencyProperty.Register(
            nameof(ColliderTypes),
            typeof(ICollection<Type>),
            typeof(ColliderEditor),
            new PropertyMetadata(new List<Type>()));

        public static readonly DependencyProperty ShowBottomSeparatorProperty = DependencyProperty.Register(
            nameof(ShowBottomSeparator),
            typeof(bool),
            typeof(ColliderEditor),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTopSeparatorProperty = DependencyProperty.Register(
            nameof(ShowTopSeparator),
            typeof(bool),
            typeof(ColliderEditor),
            new PropertyMetadata(true));

        private readonly IAssemblyService _assemblyService = ViewContainer.Resolve<IAssemblyService>();

        private readonly IBusyService _busyService = ViewContainer.Resolve<IBusyService>();

        private readonly IValueEditorService _valueEditorService = ViewContainer.Resolve<IValueEditorService>();

        private DependencyObject _editor;

        private Type _selectedColliderType;

        public ColliderEditor() : base() {
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

        public bool ShowBottomSeparator {
            get { return (bool)this.GetValue(ShowBottomSeparatorProperty); }
            set { this.SetValue(ShowBottomSeparatorProperty, value); }
        }

        public bool ShowTopSeparator {
            get { return (bool)this.GetValue(ShowTopSeparatorProperty); }
            set { this.SetValue(ShowTopSeparatorProperty, value); }
        }

        public override async Task Initialize(object value, Type memberType, object owner, string propertName, string title) {
            await base.Initialize(value, memberType, owner, propertName, title);
            var colliderTypes = await this._assemblyService.LoadTypes(typeof(Collider));
            colliderTypes.Remove(typeof(PolygonCollider)); // TODO: Eventually allow PolygonCollider
            this.ColliderTypes = colliderTypes;
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
                await this._busyService.PerformTask(task, false);
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