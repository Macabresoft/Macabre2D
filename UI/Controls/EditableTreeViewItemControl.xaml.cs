namespace Macabre2D.UI.Controls {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class EditableTreeViewItemControl : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty AllowUndoProperty = DependencyProperty.Register(
            nameof(AllowUndo),
            typeof(bool),
            typeof(EditableTreeViewItemControl),
            new PropertyMetadata());

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(EditableTreeViewItemControl),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        public static readonly DependencyProperty ValidEditableTypesProperty = DependencyProperty.Register(
            nameof(ValidEditableTypes),
            typeof(IEnumerable<object>),
            typeof(EditableTreeViewItemControl),
            new PropertyMetadata());

        private readonly IUndoService _undoService;
        private bool _isEditing;
        private TreeView _treeView;

        public EditableTreeViewItemControl() {
            this._undoService = ViewContainer.Resolve<IUndoService>();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool AllowUndo {
            get { return (bool)this.GetValue(AllowUndoProperty); }
            set { this.SetValue(AllowUndoProperty, value); }
        }

        public bool IsEditing {
            get {
                return this._isEditing;
            }

            set {
                if (this._isEditing != value) {
                    this._isEditing = value;

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsEditing)));
                }
            }
        }

        public string Text {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        public IEnumerable<object> ValidEditableTypes {
            get { return (IEnumerable<object>)this.GetValue(ValidEditableTypesProperty); }
            set { this.SetValue(ValidEditableTypesProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is EditableTreeViewItemControl control) {
                control._editableTextBox.Text = e.NewValue as string;
            }
        }

        private void CommitNewText(string oldText, string newText) {
            if (this.AllowUndo) {
                var undoCommand = new UndoCommand(() => {
                    this.Text = newText;
                    this._editableTextBox.Text = newText;
                }, () => {
                    this.Text = oldText;
                    this._editableTextBox.Text = oldText;
                });

                this._undoService.Do(undoCommand);
            }
            else {
                this.Text = newText;
            }

            this.IsEditing = false;
        }

        private void EditableTreeViewItemControl_Loaded(object sender, RoutedEventArgs e) {
            this._treeView = this.FindAncestor<TreeView>();
            this._treeView.SelectedItemChanged += this.TreeView_SelectedItemChanged;
        }

        private void EditableTreeViewItemControl_Unloaded(object sender, RoutedEventArgs e) {
            this._treeView.SelectedItemChanged -= this.TreeView_SelectedItemChanged;
            this._treeView = null;
        }

        private void TreeItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (sender is TextBox textBox && textBox.IsVisible) {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void TreeItem_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                this.CommitNewText(this.Text, this._editableTextBox.Text);
            }
            else if (e.Key == Key.Escape) {
                if (sender is TextBox textBox) {
                    textBox.Text = this.Text;
                    this.IsEditing = false;
                }
            }
        }

        private void TreeItem_LostFocus(object sender, RoutedEventArgs e) {
            this.IsEditing = false;
            this._editableTextBox.Text = this.Text;
        }

        private void TreeItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var treeViewItem = (e.OriginalSource as DependencyObject)?.FindAncestor<TreeViewItem>();
            if (treeViewItem != null && treeViewItem.IsSelected && (!this.ValidEditableTypes.Any() || this.ValidEditableTypes.Contains(treeViewItem.DataContext.GetType()))) {
                this._editableTextBox.Text = this.Text;
                this.IsEditing = true;
                e.Handled = true;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            this.CommitNewText(this.Text, this._editableTextBox.Text);
        }
    }
}