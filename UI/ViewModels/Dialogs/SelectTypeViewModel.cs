namespace Macabre2D.UI.ViewModels.Dialogs {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Models.Validation;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class SelectTypeViewModel : OKCancelDialogViewModel {
        private readonly IAssemblyService _assemblyService;
        private readonly Type _baseType;
        private string _name;
        private Type _selectedType;

        public SelectTypeViewModel(IAssemblyService assemblyService, Type baseType) {
            this._assemblyService = assemblyService;
            this._baseType = baseType;
            this.LoadedCommand = new RelayCommand(async () => await this.OnLoaded());
        }

        public RelayCommand LoadedCommand { get; }

        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

        [RequiredValidation]
        public Type SelectedType {
            get {
                return this._selectedType;
            }

            set {
                var originalType = this._selectedType;
                if (this.Set(ref this._selectedType, value) && (string.IsNullOrEmpty(this.Name) || originalType?.Name == this.Name)) {
                    this.Name = this.SelectedType.Name;
                }
            }
        }

        public ObservableCollection<Type> Types { get; } = new ObservableCollection<Type>();

        private async Task OnLoaded() {
            if (this._baseType != null) {
                var types = await this._assemblyService.LoadTypes(this._baseType);
                foreach (var type in types.OrderBy(x => x.FullName)) {
                    this.Types.Add(type);
                }
            }
        }
    }
}