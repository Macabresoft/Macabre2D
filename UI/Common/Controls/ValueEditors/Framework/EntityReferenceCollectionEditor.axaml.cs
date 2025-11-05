namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class EntityReferenceCollectionEditor : ValueEditorControl<IEntityReferenceCollection> {

    public static readonly DirectProperty<EntityReferenceCollectionEditor, ICommand> AddCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceCollectionEditor, ICommand>(
            nameof(AddCommand),
            editor => editor.AddCommand);

    public static readonly DirectProperty<EntityReferenceCollectionEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceCollectionEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<EntityReferenceCollectionEditor, IReadOnlyCollection<IEntity>> EntitiesProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceCollectionEditor, IReadOnlyCollection<IEntity>>(
            nameof(Entities),
            editor => editor.Entities);

    public static readonly DirectProperty<EntityReferenceCollectionEditor, ICommand> RemoveCommandProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceCollectionEditor, ICommand>(
            nameof(RemoveCommand),
            editor => editor.RemoveCommand);

    public static readonly DirectProperty<EntityReferenceCollectionEditor, IEntity> SelectedEntityProperty =
        AvaloniaProperty.RegisterDirect<EntityReferenceCollectionEditor, IEntity>(
            nameof(SelectedEntity),
            editor => editor.SelectedEntity,
            (editor, value) => editor.SelectedEntity = value);

    private readonly ICommonDialogService _dialogService;
    private readonly ObservableCollectionExtended<IEntity> _entities = new();

    private readonly ISceneService _sceneService;
    private readonly IUndoService _undoService;
    private IEntity _selectedEntity;

    public EntityReferenceCollectionEditor() : this(
        null,
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<ISceneService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public EntityReferenceCollectionEditor(
        ValueControlDependencies dependencies,
        ICommonDialogService dialogService,
        ISceneService sceneService,
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
        this._sceneService = sceneService;
        this._undoService = undoService;

        this.ClearCommand = ReactiveCommand.Create(this.Clear);
        this.AddCommand = ReactiveCommand.CreateFromTask(this.Add);
        this.RemoveCommand = ReactiveCommand.Create<IEntity>(this.Remove);

        this.ResetEntities();
        this.InitializeComponent();
    }

    public ICommand AddCommand { get; }

    public ICommand ClearCommand { get; }

    public IReadOnlyCollection<IEntity> Entities => this._entities;

    public ICommand RemoveCommand { get; }

    public IEntity SelectedEntity {
        get => this._selectedEntity;
        set => this.SetAndRaise(SelectedEntityProperty, ref this._selectedEntity, value);
    }


    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<IEntityReferenceCollection> args) {
        base.OnValueChanged(args);
        this.ResetEntities();
    }

    private async Task Add() {
        var entity = await this._dialogService.OpenEntitySelectionDialog(this.Value.Type, this.Title);
        if (entity != null && !this.Value.EntityIds.Contains(entity.Id)) {
            this._undoService.Do(
                () =>
                {
                    this.Value.AddEntity(entity.Id);
                    this._entities.Add(entity);
                },
                () =>
                {
                    this.Value.RemoveEntity(entity.Id);
                    this._entities.Remove(entity);
                });
        }
    }

    private void Clear() {
        var ids = this.Value.EntityIds.ToList();

        if (ids.Any()) {
            this._undoService.Do(
                () =>
                {
                    this.Value.Clear();
                    this._entities.Clear();
                },
                () =>
                {
                    var entities = this.GetEntities(this.Value.EntityIds);
                    this._entities.Reset(entities);
                    foreach (var entityId in this._entities.Select(x => x.Id)) {
                        this.Value.AddEntity(entityId);
                    }
                });
        }
    }

    private IEnumerable<IEntity> GetEntities(IEnumerable<Guid> entityIds) {
        var entities = new List<IEntity>();

        foreach (var entityId in entityIds) {
            if (this._sceneService.CurrentlyEditing.FindChild(entityId) is { } entity) {
                entities.Add(entity);
            }
        }

        return entities;
    }

    private void Remove(IEntity selectedEntity) {
        if (selectedEntity != null && this.Value.EntityIds.Contains(selectedEntity.Id)) {
            this._undoService.Do(
                () =>
                {
                    this.Value.RemoveEntity(selectedEntity.Id);
                    this._entities.Remove(selectedEntity);
                },
                () =>
                {
                    this.Value.AddEntity(selectedEntity.Id);
                    this._entities.Add(selectedEntity);
                });
        }
    }

    private void ResetEntities() {
        if (this.Value != null && this._sceneService != null) {
            if (this.Value.EntityIds.Any()) {
                this._entities.AddRange(this.GetEntities(this.Value.EntityIds));
            }
            else {
                this._entities.Clear();
            }
        }
    }
}