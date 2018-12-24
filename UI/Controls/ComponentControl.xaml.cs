namespace Macabre2D.UI.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;

    public partial class ComponentControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty ComponentProperty = DependencyProperty.Register(
            nameof(Component),
            typeof(ComponentWrapper),
            typeof(ComponentControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnComponentChanged)));

        private readonly IBusyService _busyService;
        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;

        public ComponentControl() {
            this._busyService = ViewContainer.Instance.Resolve<IBusyService>();
            this._undoService = ViewContainer.Instance.Resolve<IUndoService>();
            this._valueEditorService = ViewContainer.Instance.Resolve<IValueEditorService>();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ComponentWrapper Component {
            get { return (ComponentWrapper)this.GetValue(ComponentProperty); }
            set { this.SetValue(ComponentProperty, value); }
        }

        public int ComponentDrawOrder {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.DrawOrder;
                }

                return 0;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.DrawOrder), this.ComponentDrawOrder, value);
            }
        }

        public Layers ComponentLayer {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.Layers;
                }

                return Layers.None;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.Layers), this.ComponentLayer, value);
            }
        }

        public string ComponentName {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.Name;
                }

                return string.Empty;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.Name), this.ComponentName, value);
            }
        }

        public Vector2 ComponentPosition {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.LocalPosition;
                }

                return Vector2.Zero;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.LocalPosition), this.ComponentPosition, value);
            }
        }

        public float ComponentRotation {
            get {
                if (this?.Component?.Component != null) {
                    return MathHelper.ToDegrees(this.Component.Component.LocalRotation.Angle);
                }

                return 0;
            }

            set {
                var result = MathHelper.ToRadians(value);
                var propertyPath = $"{nameof(this.Component.Component.LocalRotation)}.{nameof(this.Component.Component.LocalRotation.Angle)}";
                this.UpdateComponentProperty(propertyPath, this.ComponentRotation, result);
            }
        }

        public Vector2 ComponentScale {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.LocalScale;
                }

                return Vector2.One;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.LocalScale), this.ComponentScale, value);
            }
        }

        public string ComponentTypeName {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.GetType().Name;
                }

                return typeof(BaseComponent).Name;
            }
        }

        public int ComponentUpdateOrder {
            get {
                if (this?.Component?.Component != null) {
                    return this.Component.Component.UpdateOrder;
                }

                return 0;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.UpdateOrder), this.ComponentUpdateOrder, value);
            }
        }

        public ObservableRangeCollection<DependencyObject> Editors { get; } = new ObservableRangeCollection<DependencyObject>();

        public bool IsComponentEnabled {
            get {
                if (this?.Component?.Component != null) {
                    var result = this.Component.Component.GetProperty("_isEnabled");
                    if (result is bool isEnabled) {
                        return isEnabled;
                    }
                }

                return false;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.IsEnabled), this.IsComponentEnabled, value);
            }
        }

        public bool IsComponentVisible {
            get {
                if (this?.Component?.Component != null) {
                    var result = this.Component.Component.GetProperty("_isVisible");
                    if (result is bool isVisible) {
                        return isVisible;
                    }
                }

                return false;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Component.IsVisible), this.IsComponentVisible, value);
            }
        }

        private static async void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ComponentControl control) {
                if (control != null && e.NewValue != null) {
                    var task = control.PopulateEditors();
                    await control._busyService.PerformTask(task);

                    if (e.NewValue is ComponentWrapper wrapper) {
                        control._drawableGrid.Visibility = Visibility.Collapsed;
                        control._updateableGrid.Visibility = Visibility.Collapsed;
                        control._updateableAsyncGrid.Visibility = Visibility.Collapsed;
                        control._baseComponentSeparator.Visibility = Visibility.Collapsed;
                        var showSeparator = false;

                        if (wrapper.Component is IDrawableComponent drawableComponent) {
                            control._drawableGrid.Visibility = Visibility.Visible;
                            showSeparator = true;
                        }

                        if (wrapper.Component is IUpdateableComponent updateableComponent) {
                            control._updateableGrid.Visibility = Visibility.Visible;
                            showSeparator = true;
                        }
                        else if (wrapper.Component is IUpdateableComponentAsync updateableComponentAsync) {
                            control._updateableAsyncGrid.Visibility = Visibility.Visible;
                            showSeparator = true;
                        }

                        if (showSeparator && control.Editors.Any()) {
                            control._baseComponentSeparator.Visibility = Visibility.Visible;
                        }
                    }

                    control.RaisePropertyChanged();
                }
                else {
                    control.Editors.Clear();
                }
            }
        }

        private async Task PopulateEditors() {
            var editors = await this._valueEditorService.CreateEditors(this.Component.Component, typeof(BaseComponent));
            this.Editors.Reset(editors);
        }

        private void RaisePropertyChanged() {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsComponentEnabled)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsComponentVisible)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentDrawOrder)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentUpdateOrder)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentScale)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentPosition)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentRotation)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentTypeName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ComponentLayer)));
        }

        private void UpdateComponentProperty(string propertyPath, object originalValue, object newValue, [CallerMemberName] string localPropertyName = "") {
            var undoCommand = new UndoCommand(
                () => {
                    this.Component.UpdateProperty(propertyPath, newValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                },
                () => {
                    this.Component.UpdateProperty(propertyPath, originalValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                });

            this._undoService.Do(undoCommand);
        }
    }
}