namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models.Validation;
    using Macabre2D.UI.CommonLibrary.Services;
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
            this.FilterFunc = new Func<object, string, bool>(this.CheckValueMatchesFilter);
            this.LoadedCommand = new RelayCommand(async () => await this.OnLoaded());
        }

        public Func<object, string, bool> FilterFunc { get; }

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

        private bool CheckValueMatchesFilter(object value, string filterText) {
            var result = false;
            if (value is Type type) {
                result = type.FullName.Contains(filterText, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

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