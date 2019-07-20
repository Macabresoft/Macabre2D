namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class GenericValueEditor : NamedValueEditor<object> {

        public static readonly DependencyProperty DeclaringTypeProperty = DependencyProperty.Register(
            nameof(DeclaringType),
            typeof(Type),
            typeof(GenericValueEditor),
            new PropertyMetadata(null));

        private readonly IBusyService _busyService;
        private readonly IValueEditorService _valueEditorService;

        public GenericValueEditor() {
            this._busyService = ViewContainer.Resolve<IBusyService>();
            this._valueEditorService = ViewContainer.Resolve<IValueEditorService>();
            this.InitializeComponent();
        }

        public Type DeclaringType {
            get { return (Type)this.GetValue(DeclaringTypeProperty); }
            set { this.SetValue(DeclaringTypeProperty, value); }
        }

        public ObservableRangeCollection<DependencyObject> Editors { get; } = new ObservableRangeCollection<DependencyObject>();

        protected override async Task OnValueChangedAsync(object newValue, object oldValue, DependencyObject d) {
            if (this.Value != null) {
                var task = this.PopulateEditors();
                await this._busyService.PerformTask(task, false);
            }
            else {
                this.Editors.Clear();
            }
        }

        private async Task PopulateEditors() {
            var editors = await this._valueEditorService.CreateEditors(this.Value, this.DeclaringType);
            this.Editors.Reset(editors);
        }
    }
}