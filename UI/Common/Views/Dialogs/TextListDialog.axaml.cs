namespace Macabresoft.Macabre2D.UI.Common;

using System.Collections.Generic;
using System.Linq;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class TextListDialog : BaseDialog {
    private readonly ObservableCollectionExtended<LicenseDefinition> _filteredLicenses = new();

    public TextListDialog() : this([]) {
    }

    [InjectionConstructor]
    public TextListDialog(IEnumerable<string> items) : base() {
        this.Items = items.ToList();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<string> Items { get; }
}