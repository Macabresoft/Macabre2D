namespace Macabre2D.UI.CommonLibrary.Controls {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;

    public partial class ModuleControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty ModuleProperty = DependencyProperty.Register(
            nameof(Module),
            typeof(BaseModule),
            typeof(ModuleControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnModuleChanged)));

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
                if (this.Module is IUpdateableModule updateableModule) {
                    result = updateableModule.IsEnabled;
                }

                return result;
            }

            set {
                if (this.Module is IUpdateableModule updateableModule) {
                    this.UpdateModuleProperty(nameof(IUpdateableModule.IsEnabled), this.IsModuleEnabled, value);
                }
            }
        }

        public bool IsModuleUpdateable {
            get {
                return this.Module is IUpdateableModule;
            }
        }

        public BaseModule Module {
            get { return (BaseModule)this.GetValue(ModuleProperty); }
            set { this.SetValue(ModuleProperty, value); }
        }

        public string ModuleName {
            get {
                if (this.Module != null) {
                    return this.Module.Name;
                }

                return string.Empty;
            }

            set {
                this.UpdateModuleProperty(nameof(this.Module.Name), this.ModuleName, value);
            }
        }

        public string ModuleTypeFullName {
            get {
                if (this.Module != null) {
                    return this.Module.GetType().FullName;
                }

                return typeof(BaseModule).FullName;
            }
        }

        public string ModuleTypeName {
            get {
                if (this.Module != null) {
                    return this.Module.GetType().Name;
                }

                return typeof(BaseModule).Name;
            }
        }

        public int ModuleUpdateOrder {
            get {
                if (this.Module is IUpdateableModule updateableModule) {
                    return updateableModule.UpdateOrder;
                }

                return 0;
            }

            set {
                if (this.Module is IUpdateableModule) {
                    this.UpdateModuleProperty(nameof(IUpdateableModule.UpdateOrder), this.ModuleUpdateOrder, value);
                }
            }
        }

        private static async void OnModuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is ModuleControl control) {
                if (e.OldValue is BaseModule oldModule) {
                    oldModule.PropertyChanged -= control.Wrapper_PropertyChanged;
                }

                if (e.NewValue is BaseModule newModule) {
                    await control.PopulateEditors();
                    control.RaisePropertyChanged();
                    newModule.PropertyChanged += control.Wrapper_PropertyChanged;
                }
                else {
                    control.Editors.Clear();
                }
            }
        }

        private async Task PopulateEditors() {
            var editors = this.IsModuleUpdateable ?
                await this._valueEditorService.CreateEditors(this.Module, typeof(BaseUpdateableModule), typeof(BaseModule), typeof(BaseUpdateableModule)) :
                await this._valueEditorService.CreateEditors(this.Module, typeof(BaseModule), typeof(BaseModule));

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

        private void RaisePropertyChanged() {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsModuleEnabled)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleUpdateOrder)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ModuleTypeFullName)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsModuleUpdateable)));
        }

        private void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateModuleProperty(string propertyPath, object originalValue, object newValue, [CallerMemberName] string localPropertyName = "") {
            var undoCommand = new UndoCommand(
                () => {
                    this.Module.SetProperty(propertyPath, newValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                },
                () => {
                    this.Module.SetProperty(propertyPath, originalValue);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(localPropertyName));
                });

            this._undoService.Do(undoCommand);
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (sender is BaseModule) {
                if (e.PropertyName == nameof(BaseModule.Name)) {
                    this.RaisePropertyChanged(nameof(this.ModuleName));
                }
            }
        }
    }
}