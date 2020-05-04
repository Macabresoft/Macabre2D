namespace Macabre2D.UI.CommonLibrary.Services {

    using Macabre2D.UI.CommonLibrary.Common.Extensions;
    using Macabre2D.UI.CommonLibrary.Dialogs;
    using Microsoft.Win32;
    using System;
    using System.Windows;
    using Unity;
    using Unity.Injection;
    using Unity.Resolution;

    public interface ICommonDialogService {

        bool ShowFileBrowser(string filter, out string path, string initialDirectory = null);

        bool ShowFolderBrowser(out string path, string initialDirectory = null);

        (Type Type, string Name) ShowSelectTypeAndNameDialog(Type type, string title);

        Type ShowSelectTypeDialog(Type type, string title);

        void ShowWarningMessageBox(string title, string message);

        MessageBoxResult ShowYesNoCancelMessageBox(string title, string message);

        bool ShowYesNoMessageBox(string title, string message);
    }

    public class CommonDialogService : ICommonDialogService {

        public CommonDialogService(IUnityContainer container) {
            this.Container = container;
        }

        protected IUnityContainer Container { get; }

        public bool ShowFileBrowser(string filter, out string path, string initialDirectory = null) {
            var dialog = new OpenFileDialog() {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filter,
                InitialDirectory = initialDirectory
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value) {
                path = dialog.FileName;
                return true;
            }

            path = string.Empty;
            return false;
        }

        public bool ShowFolderBrowser(out string path, string initialDirectory = null) {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() {
                SelectedPath = initialDirectory
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                path = dialog.SelectedPath;
                return true;
            }

            path = string.Empty;
            return false;
        }

        public (Type Type, string Name) ShowSelectTypeAndNameDialog(Type type, string title) {
            var window = this.Container.Resolve<SelectTypeDialog>(new DependencyOverride(typeof(Type), new InjectionParameter(type)));
            window.Title = title;
            window.ShowNameTextBox = true;

            if (window.SimpleShowDialog() && window.ViewModel != null) {
                return (window.ViewModel.SelectedType, window.ViewModel.Name);
            }

            return (null, null);
        }

        public Type ShowSelectTypeDialog(Type type, string title) {
            var window = this.Container.Resolve<SelectTypeDialog>(new DependencyOverride(typeof(Type), new InjectionParameter(type)));
            window.Title = title;
            return window.SimpleShowDialog() ? window.ViewModel?.SelectedType : null;
        }

        public void ShowWarningMessageBox(string title, string message) {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public MessageBoxResult ShowYesNoCancelMessageBox(string title, string message) {
            return MessageBox.Show(message, title, MessageBoxButton.YesNoCancel);
        }

        public bool ShowYesNoMessageBox(string title, string message) {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}