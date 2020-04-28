namespace Macabre2D.UI.Library.Dialogs {

    using Macabre2D.Framework;
    using Macabre2D.UI.Library.Models.Validation;
    using System;
    using System.Collections.Generic;

    public sealed class SelectModuleViewModel : OKCancelDialogViewModel {
        private BaseModule _selectedModule;

        public SelectModuleViewModel(IEnumerable<BaseModule> modules) {
            this.AvailableModules = new List<BaseModule>(modules);
            this.FilterFunc = new Func<object, string, bool>(this.CheckValueMatchesFilter);
        }

        public IReadOnlyCollection<BaseModule> AvailableModules { get; }
        public Func<object, string, bool> FilterFunc { get; }

        [RequiredValidation]
        public BaseModule SelectedModule {
            get {
                return this._selectedModule;
            }

            set {
                if (this.Set(ref this._selectedModule, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        protected override bool CanExecuteOKCommand() {
            return this.SelectedModule != null;
        }

        private bool CheckValueMatchesFilter(object value, string filterText) {
            var result = false;
            if (value is BaseModule module) {
                result = module.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}