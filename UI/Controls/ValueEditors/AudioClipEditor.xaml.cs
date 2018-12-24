namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework.Audio;
    using System.Windows;
    using System.Windows.Input;

    public partial class AudioClipEditor : NamedValueEditor<AudioClip> {

        public static readonly DependencyProperty SelectAudioClipCommandProperty = DependencyProperty.Register(
            nameof(SelectAudioClipCommand),
            typeof(ICommand),
            typeof(AudioClipEditor),
            new PropertyMetadata());

        public AudioClipEditor() {
            this.InitializeComponent();
        }

        public ICommand SelectAudioClipCommand {
            get { return (ICommand)this.GetValue(SelectAudioClipCommandProperty); }
            set { this.SetValue(SelectAudioClipCommandProperty, value); }
        }
    }
}