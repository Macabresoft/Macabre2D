namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.Command;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class ComponentHierarchy : UserControl, INotifyPropertyChanged {

        public static readonly DependencyProperty RootProperty = DependencyProperty.Register(
            nameof(Root),
            typeof(IParent<ComponentWrapper>),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedComponentProperty = DependencyProperty.Register(
            nameof(SelectedComponent),
            typeof(ComponentWrapper),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(IParent<ComponentWrapper>),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        private readonly RelayCommand _addComponentCommand;
        private readonly IBusyService _busyService;
        private readonly IDialogService _dialogService;
        private readonly IMonoGameService _monoGameService;
        private readonly RelayCommand _removeComponentCommand;
        private readonly IUndoService _undoService;
        private bool _isEditing;
        private string _oldText;
        private Point _startPoint = new Point(0d, 0d);

        public ComponentHierarchy() {
            this._busyService = ViewContainer.Resolve<IBusyService>();
            this._dialogService = ViewContainer.Resolve<IDialogService>();
            this._monoGameService = ViewContainer.Resolve<IMonoGameService>();
            this._undoService = ViewContainer.Resolve<IUndoService>();
            this._addComponentCommand = new RelayCommand(this.AddComponent, () => this.SelectedItem != null);
            this._removeComponentCommand = new RelayCommand(this.RemoveComponent, () => this.SelectedItem != null && this.SelectedItem is ComponentWrapper);
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddComponentCommand {
            get {
                return this._addComponentCommand;
            }
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

        public ICommand RemoveComponentCommand {
            get {
                return this._removeComponentCommand;
            }
        }

        public IParent<ComponentWrapper> Root {
            get { return (IParent<ComponentWrapper>)this.GetValue(RootProperty); }
            set { this.SetValue(RootProperty, value); }
        }

        public ComponentWrapper SelectedComponent {
            get { return (ComponentWrapper)this.GetValue(SelectedComponentProperty); }
            set { this.SetValue(SelectedComponentProperty, value); }
        }

        public IParent<ComponentWrapper> SelectedItem {
            get { return (IParent<ComponentWrapper>)this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        private static TreeViewItem FindTreeItem(DependencyObject source) {
            while (source != null && !(source is TreeViewItem)) {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        private void AddComponent() {
            var type = this._dialogService.ShowSelectTypeDialog(typeof(BaseComponent));
            if (type != null) {
                var baseComponent = Activator.CreateInstance(type) as BaseComponent;
                baseComponent.Name = type.Name;
                var componentWrapper = new ComponentWrapper(baseComponent);

                var undoCommand = new UndoCommand(
                    () => this.SelectedItem.AddChild(componentWrapper),
                    () => this.SelectedItem.RemoveChild(componentWrapper));

                this._undoService.Do(undoCommand);
            }
        }

        private void RemoveComponent() {
            if (this.SelectedItem is ComponentWrapper component) {
                UndoCommand undoCommand;

                if (component.Parent != null) {
                    var parent = component.Parent;
                    undoCommand = new UndoCommand(
                        () => parent.RemoveChild(component),
                        () => parent.AddChild(component));
                }
                else {
                    undoCommand = new UndoCommand(
                        () => this.Root.RemoveChild(component),
                        () => this.Root.AddChild(component));
                }

                this._undoService.Do(undoCommand);
            }
        }

        private void TreeItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (sender is TextBox textBox && textBox.IsVisible) {
                textBox.Focus();
                textBox.SelectAll();
                this._oldText = textBox.Text;
            }
        }

        private void TreeItem_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                this.IsEditing = false;
            }
            else if (e.Key == Key.Escape) {
                if (sender is TextBox textBox) {
                    textBox.Text = this._oldText;
                    this.IsEditing = false;
                }
            }
        }

        private void TreeItem_LostFocus(object sender, RoutedEventArgs e) {
            this.IsEditing = false;
        }

        private void TreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var treeViewItem = FindTreeItem(e.OriginalSource as DependencyObject);
            if (treeViewItem?.DataContext is ComponentWrapper componentWrapper && this._monoGameService.EditorGame is EditorGame editorGame) {
                editorGame.FocusComponent(componentWrapper.Component);
            }
        }

        private void TreeItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var treeViewItem = FindTreeItem(e.OriginalSource as DependencyObject);
            if (treeViewItem != null && treeViewItem.IsSelected && treeViewItem.DataContext is ComponentWrapper) {
                this.IsEditing = true;
                e.Handled = true;
            }
        }

        private void TreeView_DragEnter(object sender, DragEventArgs e) {
            if (!e.Data.GetDataPresent(typeof(ComponentWrapper))) {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e) {
            this._busyService.PerformAction(() => {
                if (e.Data.GetDataPresent(typeof(ComponentWrapper)) && e.OriginalSource is DependencyObject dependencyObject) {
                    if (e.Data.GetData(typeof(ComponentWrapper)) is ComponentWrapper draggedItem) {
                        if (!(dependencyObject is TreeViewItem treeViewItem)) {
                            treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                            if (treeViewItem == null) {
                                return;
                            }
                        }

                        if (treeViewItem.DataContext is IParent<ComponentWrapper> dropTarget) {
                            draggedItem.Parent = dropTarget;
                        }
                    }
                }
            });
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e) {
            this._startPoint = e.GetPosition(null);
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e) {
            if (!this._busyService.IsBusy) {
                if (e.LeftButton == MouseButtonState.Pressed) {
                    var mousePosition = e.GetPosition(null);
                    var difference = this._startPoint - mousePosition;
                    if (Math.Abs(difference.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(difference.Y) > SystemParameters.MinimumVerticalDragDistance) {
                        if (sender is TreeView treeView && e.OriginalSource is DependencyObject dependencyObject) {
                            if (!(dependencyObject is TreeViewItem treeViewItem)) {
                                treeViewItem = dependencyObject.FindAncestor<TreeViewItem>();

                                if (treeViewItem == null) {
                                    return;
                                }
                            }

                            if (treeView.SelectedItem is ComponentWrapper componentWrapper) {
                                var dragData = new DataObject(componentWrapper);
                                DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);
                            }
                        }
                    }
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            this.SelectedItem = e.NewValue as IParent<ComponentWrapper>;
            this.SelectedComponent = e.NewValue as ComponentWrapper;
            this._addComponentCommand.RaiseCanExecuteChanged();
            this._removeComponentCommand.RaiseCanExecuteChanged();
            this.IsEditing = false;
        }
    }
}