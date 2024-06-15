namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Unity;

public partial class ResourceStringEditor : ValueEditorControl<string> {
    public static readonly DirectProperty<ResourceStringEditor, IReadOnlyCollection<string>> ResourceNamesProperty =
        AvaloniaProperty.RegisterDirect<ResourceStringEditor, IReadOnlyCollection<string>>(
            nameof(Resources),
            editor => editor.ResourceNames);

    public ResourceStringEditor() : this(null) {
    }

    [InjectionConstructor]
    public ResourceStringEditor(ValueControlDependencies dependencies) : base(dependencies) {
        var resourceNames = new List<string>();
        var resourceSet = Project.Common.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
        if (resourceSet != null) {
            resourceNames.AddRange(from DictionaryEntry resource in resourceSet select resource.Key.ToString());
        }

        resourceNames.Sort();
        this.ResourceNames = resourceNames;
        this.InitializeComponent();
    }

    public IReadOnlyCollection<string> ResourceNames { get; }
}