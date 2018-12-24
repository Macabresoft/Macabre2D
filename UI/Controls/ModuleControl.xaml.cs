namespace Macabre2D.UI.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;

    public partial class ModuleControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty ModuleProperty = DependencyProperty.Register(
            nameof(Module),
            typeof(ModuleWrapper),
            typeof(ModuleControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnComponentChanged)));

        private readonly IUndoService _undoService;
        private readonly IValueEditorService _valueEditorService;

        public ModuleControl() {
            this._undoService = ViewContainer.Instance.Resolve<IUndoService>();
            this._valueEditorService = ViewContainer.Instance.Resolve<IValueEditorService>();
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableRangeCollection<DependencyObject> Editors { get; } = new ObservableRangeCollection<DependencyObject>();

        public bool IsModuleEnabled {
            get {
                if (this?.Module?.Module != null) {
                    var result = this.Module.Module.GetProperty("_isEnabled");
                    if (result is bool isEnabled) {
                        return isEnabled;
                    }
                }

                return false;
            }

            set {
                this.UpdateModuleProperty(nameof(this.Module.Module.IsEnabled), this.IsModuleEnabled, value);
            }
        }

        public ModuleWrapper Module {
            get { return (ModuleWrapper)GetValue(ModuleProperty); }
            set { SetValue(ModuleProperty, value); }
        }

        public string ModuleTypeFullName {
            get {
                if (this?.Module?.Module != null) {
                    return this.Module.Module.GetType().FullName;
                }

                return typeof(BaseModule).FullName;
            }
        }

        public string ModuleTypeName {
            get {
                if (this?.Module?.Module != null) {
                    return this.Module.Module.GetType().Name;
                }

                return typeof(BaseModule).Name;
            }
        }

        public int ModuleUpdateOrder {
            get {
                if (this?.Module?.Module != null) {
                    return this.Module.Module.UpdateOrder;
                }

                return 0;
            }

            set {
                this.UpdateModuleProperty(nameof(this.Module.Module.UpdateOrder), this.ModuleUpdateOrder, value);
            }
        }

        private static async void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ModuleControl control) {
                if (control != null && e.NewValue != null) {
                    await control.PopulateEditors();
                    control.RaisePropertyChanged();
                }
                else {
                    control.Editors.Clear();
                }
            }
        }

        private async Task PopulateEditors() {
            var editors = await this._valueEditorService.CreateEditors(this.Module.Module, typeof(BaseModule));
            this.Editors.Reset(editors);
        }

        private void RaisePropertyChanged() {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsModuleEnabled)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleUpdateOrder)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeFullName)));
        }

        private void UpdateModuleProperty(string propertyPath, object originalValue, object newValue, [CallerMemberName] string localPropertyName = "") {
            var undoCommand = new UndoCommand(
                () => {
                    this.Module.UpdateProperty(propertyPath, newValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                },
                () => {
                    this.Module.UpdateProperty(propertyPath, originalValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                });

            this._undoService.Do(undoCommand);
        }
    }
}