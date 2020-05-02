namespace Macabre2D.UI.CommonLibrary.Controls {

    using GalaSoft.MvvmLight.Command;
    using GongSolutions.Wpf.DragDrop;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Models;
    using Macabre2D.UI.CommonLibrary.Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class ComponentHierarchy : UserControl, IDropTarget {

        public static readonly DependencyProperty HighlightedTypeProperty = DependencyProperty.Register(
            nameof(ComponentHierarchy.HighlightedType),
            typeof(Type),
            typeof(ComponentHierarchy),
            new PropertyMetadata(typeof(BaseComponent)));

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(
            nameof(ComponentHierarchy.IsEditable),
            typeof(bool),
            typeof(ComponentHierarchy),
            new PropertyMetadata(true));

        public static readonly DependencyProperty RootProperty = DependencyProperty.Register(
            nameof(Root),
            typeof(Scene),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedComponentProperty = DependencyProperty.Register(
            nameof(SelectedComponent),
            typeof(BaseComponent),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(ComponentHierarchy),
            new PropertyMetadata());

        private readonly RelayCommand _addComponentCommand;

        private readonly RelayCommand _cloneComponentCommand;

        private readonly IComponentService _componentService = ViewContainer.Resolve<IComponentService>();

        private readonly RelayCommand _createPrefabCommand;

        private readonly IDialogService _dialogService = ViewContainer.Resolve<IDialogService>();

        private readonly IMonoGameService _monoGameService = ViewContainer.Resolve<IMonoGameService>();

        private readonly IProjectService _projectService = ViewContainer.Resolve<IProjectService>();

        private readonly RelayCommand _removeComponentCommand;

        private readonly IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public ComponentHierarchy() {
            this._componentService.PropertyChanged += this.ComponentService_SelectionChanged;
            this._addComponentCommand = new RelayCommand(this.AddComponent, () => this.SelectedItem != null);
            this._cloneComponentCommand = new RelayCommand(this.CloneComponent, () => this.SelectedItem is BaseComponent);
            this._createPrefabCommand = new RelayCommand(this.CreatePrefab, () => this.SelectedItem is BaseComponent);
            this._removeComponentCommand = new RelayCommand(this.RemoveComponent, () => this.SelectedItem is BaseComponent);
            this.InitializeComponent();
        }

        public ICommand AddComponentCommand {
            get {
                return this._addComponentCommand;
            }
        }

        public ICommand CloneComponentCommand {
            get {
                return this._cloneComponentCommand;
            }
        }

        public ICommand CreatePrefabCommand {
            get {
                return this._createPrefabCommand;
            }
        }

        public Type HighlightedType {
            get { return (Type)this.GetValue(HighlightedTypeProperty); }
            set { this.SetValue(HighlightedTypeProperty, value); }
        }

        public bool IsEditable {
            get { return (bool)this.GetValue(IsEditableProperty); }
            set { this.SetValue(IsEditableProperty, value); }
        }

        public ICommand RemoveComponentCommand {
            get {
                return this._removeComponentCommand;
            }
        }

        public Scene Root {
            get { return (Scene)this.GetValue(RootProperty); }
            set { this.SetValue(RootProperty, value); }
        }

        public BaseComponent SelectedComponent {
            get { return (BaseComponent)this.GetValue(SelectedComponentProperty); }
            set { this.SetValue(SelectedComponentProperty, value); }
        }

        public object SelectedItem {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        public IEnumerable<Type> ValidTypes {
            get {
                return new[] { typeof(BaseComponent) };
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo) {
            if (dropInfo.Data is BaseComponent &&
                (dropInfo.TargetItem is BaseComponent || dropInfo.TargetItem is Scene)) {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo) {
            if (dropInfo.Data is BaseComponent component) {
                if (dropInfo.TargetItem is BaseComponent targetComponent) {
                    component.Parent = targetComponent;
                }
                else if (dropInfo.TargetItem is Scene) {
                    component.Parent = null;
                }
            }
        }

        private void AddComponent() {
            var result = this._dialogService.ShowSelectTypeAndNameDialog(typeof(BaseComponent), "Select a Component");
            if (result.Type != null) {
                var addedComponent = Activator.CreateInstance(result.Type) as BaseComponent;
                addedComponent.Name = result.Name;
                this.AddComponent(addedComponent, this.SelectedItem);
            }
        }

        private void AddComponent(BaseComponent component, object parent) {
            var originalParent = component.Parent;
            if (parent is BaseComponent parentComponent) {
                var undoCommand = new UndoCommand(
                    () => parentComponent.AddChild(component),
                    () => parentComponent.RemoveChild(component));

                this._undoService.Do(undoCommand);
            }
            else if (parent is Scene scene) {
                var undoCommand = new UndoCommand(
                    () => scene.AddChild(component),
                    () => scene.RemoveChild(component));

                this._undoService.Do(undoCommand);
            }
        }

        private void CloneComponent() {
            if (this.SelectedItem is BaseComponent component) {
                var clone = component.Clone();
                this.AddComponent(clone, component.Parent);
            }
        }

        private void ComponentService_SelectionChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._componentService.SelectedItem)) {
                if (this._componentService.SelectedItem == null) {
                    if (this._treeView.FindTreeViewItem(this.Root) is TreeViewItem rootTreeViewItem) {
                        rootTreeViewItem.IsSelected = true;
                    }
                }
                else if (this._treeView.FindTreeViewItem(this._componentService.SelectedItem) is TreeViewItem treeViewItem) {
                    treeViewItem.IsSelected = true;
                }
            }
        }

        private void CreatePrefab() {
            if (this.SelectedItem is BaseComponent component) {
                var clone = component.Clone();
                clone.Parent = null;
                var asset = new PrefabAsset(clone.Name);
                asset.SavableValue.Component = clone;
                if (this._dialogService.ShowSaveAssetAsDialog(this._projectService.CurrentProject, asset)) {
                    this._undoService.Clear();
                    var parent = component.Parent;
                    parent.RemoveChild(component);

                    var prefabComponent = new PrefabComponent() {
                        Name = $"{clone.Name} (prefab)",
                        Prefrab = asset.SavableValue
                    };

                    parent.AddChild(prefabComponent);
                    this.SelectedItem = prefabComponent;
                }
            }
        }

        private void RemoveComponent() {
            if (this.SelectedItem is BaseComponent component) {
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

        private void TreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (this.IsEditable) {
                var treeViewItem = (e.OriginalSource as DependencyObject)?.FindAncestor<TreeViewItem>();
                if (treeViewItem?.DataContext is BaseComponent component && this._monoGameService.SceneEditor != null) {
                    this._monoGameService.SceneEditor.FocusComponent(component);
                }
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            this.SelectedItem = e.NewValue;
            this.SelectedComponent = e.NewValue as BaseComponent;
            this._addComponentCommand.RaiseCanExecuteChanged();
            this._cloneComponentCommand.RaiseCanExecuteChanged();
            this._createPrefabCommand.RaiseCanExecuteChanged();
            this._removeComponentCommand.RaiseCanExecuteChanged();
        }
    }
}