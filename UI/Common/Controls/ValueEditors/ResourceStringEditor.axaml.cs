namespace Macabre2D.UI.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using ReactiveUI;
using Unity;

public partial class ResourceStringEditor : ValueEditorControl<string> {
    public static readonly DirectProperty<ResourceStringEditor, IReadOnlyCollection<ResourceEntry>> ResourceEntriesProperty =
        AvaloniaProperty.RegisterDirect<ResourceStringEditor, IReadOnlyCollection<ResourceEntry>>(
            nameof(ResourceEntries),
            editor => editor.ResourceEntries);
    
    public static readonly DirectProperty<ResourceStringEditor, IReadOnlyCollection<string>> ResourceNamesProperty =
        AvaloniaProperty.RegisterDirect<ResourceStringEditor, IReadOnlyCollection<string>>(
            nameof(ResourceNames),
            editor => editor.ResourceNames);
    
    public static readonly DirectProperty<ResourceStringEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<ResourceStringEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<ResourceStringEditor, ICommand> SearchCommandProperty =
        AvaloniaProperty.RegisterDirect<ResourceStringEditor, ICommand>(
            nameof(SearchCommand),
            editor => editor.SearchCommand);

    private readonly IUndoService _undoService;
    private readonly ICommonDialogService _dialogService;
    
    public ResourceStringEditor() : this(null, Resolver.Resolve<ICommonDialogService>(), Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ResourceStringEditor(
        ValueControlDependencies dependencies, 
        ICommonDialogService dialogService, 
        IUndoService undoService) : base(dependencies) {
        this._dialogService = dialogService;
        this._undoService = undoService;
        
        var resources = new List<ResourceEntry>();
        var resourceSet = Project.Common.Resources.ResourceManager.GetResourceSet(new CultureInfo(string.Empty), true, true);
        if (resourceSet != null) {
            resources.AddRange(from DictionaryEntry resource in resourceSet select new ResourceEntry(resource.Key.ToString(), resource.Value?.ToString() ?? string.Empty));
        }

        this.ResourceEntries = resources;
        this.ResourceNames = resources.Select(x => x.Key).ToList();
        
        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.WhenAny(x => x.Value, y => !string.IsNullOrEmpty(y.Value)));
        this.SearchCommand = ReactiveCommand.CreateFromTask(this.Select);
        
        this.InitializeComponent();
    }
    
    public ICommand ClearCommand { get; }

    public ICommand SearchCommand { get; }

    public IReadOnlyCollection<ResourceEntry> ResourceEntries { get; }
    
    public IReadOnlyCollection<string> ResourceNames { get; }
    
    private void Clear() {
        var originalValue = this.Value;

        if (!string.IsNullOrEmpty(originalValue)) {
            this._undoService.Do(
                () => this.Value = string.Empty,
                () => this.Value = originalValue);
        }
    }
    
    private async Task Select() {
        var resourceEntry = await this._dialogService.ShowSearchResourceDialog(this.ResourceEntries, this.Title);
        if (resourceEntry != null) {
            var originalResource = this.Value;
            this._undoService.Do(
                () => { this.Value = resourceEntry.Value.Key; },
                () => { this.Value = originalResource; });
        }
    }
}