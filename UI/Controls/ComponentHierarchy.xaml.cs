namespace Macabre2D.UI.Controls {

    using GalaSoft.MvvmLight.Command;
    using GongSolutions.Wpf.DragDrop;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly IDialogService _dialogService;
        private readonly IMonoGameService _monoGameService;
        private readonly RelayCommand _removeComponentCommand;
        private readonly IComponentSelectionService _selectionService;
        private readonly IUndoService _undoService;

        public ComponentHierarchy() {
            this._dialogService = ViewContainer.Resolve<IDialogService>();
            this._monoGameService = ViewContainer.Resolve<IMonoGameService>();
            this._selectionService = ViewContainer.Resolve<IComponentSelectionService>();
            this._undoService = ViewContainer.Resolve<IUndoService>();

            this._selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
            this._addComponentCommand = new RelayCommand(this.AddComponent, () => this.SelectedItem != null);
            this._removeComponentCommand = new RelayCommand(this.RemoveComponent, () => this.SelectedItem != null && this.SelectedItem is ComponentWrapper);
            this.InitializeComponent();
        }

        public ICommand AddComponentCommand {
            get {
                return this._addComponentCommand;
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
            if (dropInfo.Data is DefaultDataWrapper source &&
                dropInfo.TargetItem is IHierarchical<ComponentWrapper, IParent<ComponentWrapper>> target) {
                var components = source.Items.OfType<ComponentWrapper>();

                if (components.Any()) {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else {
                    dropInfo.DropTargetAdorner = null;
                    dropInfo.Effects = DragDropEffects.None;
                }
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo) {
            if (dropInfo.Data is DefaultDataWrapper source && dropInfo.TargetItem is IHierarchical<ComponentWrapper, IParent<ComponentWrapper>> target) {
                var components = source.Items.OfType<ComponentWrapper>();

                foreach (var component in components) {
                    component.Parent = target;
                }
            }
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

        private void SelectionService_SelectionChanged(object sender, ValueChangedEventArgs<ComponentWrapper> e) {
            if (e.NewValue != null && this._treeView.FindTreeViewItem(e.NewValue) is TreeViewItem treeViewItem) {
                treeViewItem.IsSelected = true;
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
            this._removeComponentCommand.RaiseCanExecuteChanged();
        }
    }
}