namespace Macabre2D.UI.Library.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Models.FrameworkWrappers;
    using Macabre2D.UI.Library.Services;
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
                var result = false;
                if (this.Module?.Module is IUpdateableModule updateableModule) {
                    result = updateableModule.IsEnabled;
                }

                return result;
            }

            set {
                if (this.Module?.Module is IUpdateableModule updateableModule) {
                    this.UpdateModuleProperty(nameof(IUpdateableModule.IsEnabled), this.IsModuleEnabled, value);
                }
            }
        }

        public bool IsModuleUpdateable {
            get {
                return this.Module?.Module is IUpdateableModule;
            }
        }

        public ModuleWrapper Module {
            get { return (ModuleWrapper)this.GetValue(ModuleProperty); }
            set { this.SetValue(ModuleProperty, value); }
        }

        public string ModuleName {
            get {
                if (this?.Module?.Module != null) {
                    return this.Module.Module.Name;
                }

                return string.Empty;
            }

            set {
                this.UpdateModuleProperty(nameof(this.Module.Module.Name), this.ModuleName, value);
            }
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
                if (this.Module?.Module is IUpdateableModule updateableModule) {
                    return updateableModule.UpdateOrder;
                }

                return 0;
            }

            set {
                if (this.Module?.Module is IUpdateableModule updateableModule) {
                    this.UpdateModuleProperty(nameof(IUpdateableModule.UpdateOrder), this.ModuleUpdateOrder, value);
                }
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
            if (this.IsModuleUpdateable) {
                var editors = await this._valueEditorService.CreateEditors(this.Module.Module, typeof(BaseUpdateableModule), typeof(BaseModule), typeof(BaseUpdateableModule));
                this.Editors.Reset(editors);
            }
            else {
                var editors = await this._valueEditorService.CreateEditors(this.Module.Module, typeof(BaseModule), typeof(BaseModule));
                this.Editors.Reset(editors);
            }
        }

        private void RaisePropertyChanged() {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsModuleEnabled)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleUpdateOrder)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeFullName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsModuleUpdateable)));
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