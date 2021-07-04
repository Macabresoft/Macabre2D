namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls {
    using System.IO;
    using System.Threading.Tasks;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Interactivity;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.UI.Common.Utilities;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Helpers;

    public class EditableTreeViewItem : UserControl {
        public static readonly StyledProperty<bool> AllowUndoProperty =
            AvaloniaProperty.Register<EditableTreeViewItem, bool>(nameof(AllowUndo));

        public static readonly StyledProperty<bool> IsEditableProperty =
            AvaloniaProperty.Register<EditableTreeViewItem, bool>(nameof(IsEditable), true);

        public static readonly DirectProperty<EditableTreeViewItem, bool> IsEditingProperty =
            AvaloniaProperty.RegisterDirect<EditableTreeViewItem, bool>(
                nameof(IsEditing),
                editor => editor.IsEditing,
                (editor, value) => editor.IsEditing = value);

        public static readonly StyledProperty<bool> IsFileNameProperty =
            AvaloniaProperty.Register<EditableTreeViewItem, bool>(nameof(IsFileName));

        public static readonly StyledProperty<UndoScope> UndoScopeProperty =
            AvaloniaProperty.Register<EditableTreeViewItem, UndoScope>(nameof(UndoScopeProperty));

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<EditableTreeViewItem, string>(nameof(Text), string.Empty, notifying: OnTextChanging);


        private readonly IDialogService _dialogService = Resolver.Resolve<IDialogService>();
        private readonly IProjectService _projectService = Resolver.Resolve<IProjectService>();

        private readonly ISceneService _sceneService = Resolver.Resolve<ISceneService>();
        private readonly IUndoService _undoService = Resolver.Resolve<IUndoService>();
        private bool _isEditing;

        public EditableTreeViewItem() {
            this.InitializeComponent();
        }

        public bool AllowUndo {
            get => this.GetValue(AllowUndoProperty);
            set => this.SetValue(AllowUndoProperty, value);
        }

        public bool IsEditable {
            get => this.GetValue(IsEditableProperty);
            set => this.SetValue(IsEditableProperty, value);
        }

        public bool IsEditing {
            get => this._isEditing;
            set => this.SetAndRaise(IsEditingProperty, ref this._isEditing, value);
        }

        public bool IsFileName {
            get => this.GetValue(IsFileNameProperty);
            set => this.SetValue(IsFileNameProperty, value);
        }

        public string Text {
            get => this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public UndoScope UndoScope {
            get => this.GetValue(UndoScopeProperty);
            set => this.SetValue(UndoScopeProperty, value);
        }


        private static bool CanEditTreeViewItem(TreeViewItem treeViewItem) {
            return treeViewItem?.DataContext != null && treeViewItem.IsSelected;
        }

        private async Task CommitNewText(string oldText, string newText) {
            if (this.IsFileName && !FileHelper.IsValidFileName(newText)) {
                await this._dialogService.ShowWarningDialog("Invalid File Name", $"'{newText}' contains invalid characters.");
                return;
            }

            if (this.AllowUndo) {
                var originalHasChanges = this.GetOriginalHasChanges();
                this._undoService.Do(
                    () => {
                        this.SetText(newText);
                        this.SetHasChanges(true);
                    }, () => {
                        this.SetText(oldText);
                        this.SetHasChanges(originalHasChanges);
                    }, this.UndoScope);
            }
            else {
                this.SetText(newText);
                this.SetHasChanges(true);
            }

            this.IsEditing = false;
        }

        private void EditableTextBox_OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
            if (e.Property.Name == nameof(this.IsVisible)) {
                if (sender is TextBox { IsVisible: true } textBox) {
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private string GetEditableText(string originalText) {
            var result = originalText;

            if (this.IsFileName) {
                result = Path.GetFileNameWithoutExtension(originalText);
            }

            return result;
        }

        private bool GetOriginalHasChanges() {
            var result = this.UndoScope switch {
                UndoScope.Scene => this._sceneService.HasChanges,
                UndoScope.Project => this._projectService.HasChanges,
                _ => false
            };

            return result;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private static void OnTextChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is EditableTreeViewItem treeViewItem && treeViewItem.TryGetEditableTextBox(out var textBox)) {
                textBox.Text = treeViewItem.GetEditableText(treeViewItem.Text);
            }
        }

        private void SetHasChanges(bool value) {
            if (this.UndoScope == UndoScope.Scene) {
                Dispatcher.UIThread.Post(() => { this._sceneService.HasChanges = value; });
            }
            else if (this.UndoScope == UndoScope.Project) {
                Dispatcher.UIThread.Post(() => { this._projectService.HasChanges = value; });
            }
        }

        private void SetText(string text) {
            Dispatcher.UIThread.Post(() => { this.Text = text; });
        }

        private async void TextBox_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (this.TryGetEditableTextBox(out var textBox)) {
                    var newText = this.IsFileName ? $"{textBox.Text}{Path.GetExtension(this.Text)}" : textBox.Text;
                    await this.CommitNewText(this.Text, newText);
                }
            }
            else if (e.Key == Key.Escape) {
                if (sender is TextBox textBox) {
                    textBox.Text = this.GetEditableText(this.Text);
                    this.IsEditing = false;
                }
            }
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e) {
            this.IsEditing = false;
            if (this.TryGetEditableTextBox(out var textBox)) {
                textBox.Text = this.GetEditableText(this.Text);
            }
        }

        private void TreeViewItem_OnDoubleTapped(object sender, RoutedEventArgs e) {
            var treeViewItem = this.FindAncestor<TreeViewItem>();
            if (this.IsEditable && CanEditTreeViewItem(treeViewItem) && this.TryGetEditableTextBox(out var textBox)) {
                textBox.Text = this.GetEditableText(this.Text);
                this.IsEditing = true;
                e.Handled = true;
            }
        }

        private bool TryGetEditableTextBox(out TextBox textBox) {
            textBox = this.FindControl<TextBox>("_editableTextBox");
            return textBox != null;
        }
    }
}