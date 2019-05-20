namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.Command;
    using GongSolutions.Wpf.DragDrop;
    using Macabre2D.Framework;
    using Macabre2D.Framework.Extensions;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class ComponentHierarchy : UserControl, IDropTarget {

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
        private readonly RelayCommand _cloneComponentCommand;
        private readonly IComponentService _componentService;
        private readonly IDialogService _dialogService;
        private readonly IMonoGameService _monoGameService;
        private readonly RelayCommand _removeComponentCommand;
        private readonly IUndoService _undoService;

        public ComponentHierarchy() {
            this._dialogService = ViewContainer.Resolve<IDialogService>();
            this._monoGameService = ViewContainer.Resolve<IMonoGameService>();
            this._componentService = ViewContainer.Resolve<IComponentService>();
            this._undoService = ViewContainer.Resolve<IUndoService>();

            this._componentService.SelectionChanged += this.ComponentService_SelectionChanged;
            this._addComponentCommand = new RelayCommand(this.AddComponent, () => this.SelectedItem != null);
            this._cloneComponentCommand = new RelayCommand(this.CloneComponent, () => this.SelectedItem is ComponentWrapper);
            this._removeComponentCommand = new RelayCommand(this.RemoveComponent, () => this.SelectedItem is ComponentWrapper);
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

        public IEnumerable<Type> ValidTypes {
            get {
                return new[] { typeof(ComponentWrapper) };
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo) {
            if (dropInfo.Data is ComponentWrapper &&
                dropInfo.TargetItem is IParent<ComponentWrapper>) {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo) {
            if (dropInfo.Data is ComponentWrapper component && dropInfo.TargetItem is IParent<ComponentWrapper> target) {
                component.Parent = target;
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

        private void AddComponent(BaseComponent component, IParent<ComponentWrapper> parent) {
            var componentWrapper = new ComponentWrapper(component);

            var undoCommand = new UndoCommand(
                () => parent.AddChild(componentWrapper),
                () => parent.RemoveChild(componentWrapper));

            this._undoService.Do(undoCommand);
        }

        private void CloneComponent() {
            if (this.SelectedItem is ComponentWrapper componentWrapper) {
                var clone = componentWrapper.Component.Clone();
                this.AddComponent(clone, componentWrapper.Parent);
            }
        }

        private void ComponentService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (e.NewValue != null && this._treeView.FindTreeViewItem(e.NewValue) is TreeViewItem treeViewItem) {
                treeViewItem.IsSelected = true;
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

        private void TreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var treeViewItem = (e.OriginalSource as DependencyObject)?.FindAncestor<TreeViewItem>();
            if (treeViewItem?.DataContext is ComponentWrapper componentWrapper && this._monoGameService.EditorGame is EditorGame editorGame) {
                editorGame.FocusComponent(componentWrapper.Component);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            this.SelectedItem = e.NewValue as IParent<ComponentWrapper>;
            this.SelectedComponent = e.NewValue as ComponentWrapper;
            this._addComponentCommand.RaiseCanExecuteChanged();
            this._cloneComponentCommand.RaiseCanExecuteChanged();
            this._removeComponentCommand.RaiseCanExecuteChanged();
        }
    }
}