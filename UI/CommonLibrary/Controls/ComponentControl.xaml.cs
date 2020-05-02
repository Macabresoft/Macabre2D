namespace Macabre2D.UI.CommonLibrary.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
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
            typeof(BaseComponent),
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

        public BaseComponent Component {
            get { return (BaseComponent)this.GetValue(ComponentProperty); }
            set { this.SetValue(ComponentProperty, value); }
        }

        public int ComponentDrawOrder {
            get {
                return this.Component?.DrawOrder ?? 0;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.DrawOrder), this.ComponentDrawOrder, value);
            }
        }

        public Layers ComponentLayer {
            get {
                return this.Component?.Layers ?? Layers.None;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Layers), this.ComponentLayer, value);
            }
        }

        public string ComponentName {
            get {
                return this.Component?.Name ?? string.Empty;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.Name), this.ComponentName, value);
            }
        }

        public Vector2 ComponentPosition {
            get {
                return this.Component?.LocalPosition ?? Vector2.Zero;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.LocalPosition), this.ComponentPosition, value);
            }
        }

        public Vector2 ComponentScale {
            get {
                return this.Component?.LocalScale ?? Vector2.One;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.LocalScale), this.ComponentScale, value);
            }
        }

        public string ComponentTypeName {
            get {
                return this.Component != null ? this.Component.GetType().Name : typeof(BaseComponent).Name;
            }
        }

        public int ComponentUpdateOrder {
            get {
                return this.Component?.UpdateOrder ?? 0;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.UpdateOrder), this.ComponentUpdateOrder, value);
            }
        }

        public ObservableRangeCollection<DependencyObject> Editors { get; } = new ObservableRangeCollection<DependencyObject>();

        public bool IsComponentEnabled {
            get {
                return this.Component?.IsEnabled == true;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.IsEnabled), this.IsComponentEnabled, value);
            }
        }

        public bool IsComponentVisible {
            get {
                return this.Component?.IsVisible == true;
            }

            set {
                this.UpdateComponentProperty(nameof(this.Component.IsVisible), this.IsComponentVisible, value);
            }
        }

        private static async void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ComponentControl control) {
                if (e.OldValue is BaseComponent oldComponent) {
                    oldComponent.PropertyChanged -= control.Wrapper_PropertyChanged;
                }

                if (e.NewValue is BaseComponent component) {
                    var task = control.PopulateEditors();
                    await control._busyService.PerformTask(task, false);

                    control._drawOrderEditor.Visibility = Visibility.Collapsed;
                    control._updateableGrid.Visibility = Visibility.Collapsed;
                    control._enableableGrid.Visibility = Visibility.Collapsed;
                    control._baseComponentSeparator.Visibility = Visibility.Collapsed;
                    var showSeparator = false;

                    if (component is IDrawableComponent) {
                        control._drawOrderEditor.Visibility = Visibility.Visible;
                        showSeparator = true;
                    }

                    if (component is IUpdateableComponent) {
                        control._updateableGrid.Visibility = Visibility.Visible;
                        showSeparator = true;
                    }
                    else {
                        control._enableableGrid.Visibility = Visibility.Visible;
                        showSeparator = true;
                    }

                    if (showSeparator && control.Editors.Any()) {
                        control._baseComponentSeparator.Visibility = Visibility.Visible;
                    }

                    control.RaisePropertiesChanged();
                    component.PropertyChanged += control.Wrapper_PropertyChanged;
                }
                else {
                    control.Editors.Clear();
                }
            }
        }

        private async Task PopulateEditors() {
            var editors = await this._valueEditorService.CreateEditors(this.Component, typeof(BaseComponent), typeof(BaseComponent));
            var count = editors.Count;
            for (var i = 0; i < count; i++) {
                if (editors.ElementAtOrDefault(i) is ISeparatedValueEditor currentSeparated) {
                    var previousEditor = editors.ElementAtOrDefault(i);
                    if (previousEditor == null || previousEditor is ISeparatedValueEditor) {
                        currentSeparated.ShowTopSeparator = false;
                    }

                    var nextEditor = editors.ElementAtOrDefault(i + 1);
                    if (nextEditor == null) {
                        currentSeparated.ShowBottomSeparator = false;
                    }
                }
            }

            this.Editors.Reset(editors);
        }

        private void RaisePropertiesChanged() {
            this.RaisePropertyChanged(nameof(this.IsComponentEnabled));
            this.RaisePropertyChanged(nameof(this.IsComponentVisible));
            this.RaisePropertyChanged(nameof(this.ComponentDrawOrder));
            this.RaisePropertyChanged(nameof(this.ComponentUpdateOrder));
            this.RaisePropertyChanged(nameof(this.ComponentName));
            this.RaisePropertyChanged(nameof(this.ComponentScale));
            this.RaisePropertyChanged(nameof(this.ComponentPosition));
            this.RaisePropertyChanged(nameof(this.ComponentTypeName));
            this.RaisePropertyChanged(nameof(this.ComponentLayer));
        }

        private void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateComponentProperty(string propertyPath, object originalValue, object newValue, [CallerMemberName] string localPropertyName = "") {
            var undoCommand = new UndoCommand(
                () => {
                    this.Component.SetProperty(propertyPath, newValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                },
                () => {
                    this.Component.SetProperty(propertyPath, originalValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                });

            this._undoService.Do(undoCommand);
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (sender is BaseComponent) {
                if (e.PropertyName == nameof(BaseComponent.LocalPosition)) {
                    this.RaisePropertyChanged(nameof(this.ComponentPosition));
                }
                else if (e.PropertyName == nameof(BaseComponent.LocalScale)) {
                    this.RaisePropertyChanged(nameof(this.ComponentScale));
                }
                else if (e.PropertyName == nameof(BaseComponent.DrawOrder)) {
                    this.RaisePropertyChanged(nameof(this.ComponentDrawOrder));
                }
                else if (e.PropertyName == nameof(BaseComponent.Layers)) {
                    this.RaisePropertyChanged(nameof(this.ComponentLayer));
                }
                else if (e.PropertyName == nameof(BaseComponent.Name)) {
                    this.RaisePropertyChanged(nameof(this.ComponentName));
                }
                else if (e.PropertyName == nameof(BaseComponent.UpdateOrder)) {
                    this.RaisePropertyChanged(nameof(this.ComponentUpdateOrder));
                }
            }
        }
    }
}