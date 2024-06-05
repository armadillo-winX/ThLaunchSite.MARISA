using System.Windows;

namespace ThLaunchSite.MARISA
{
    /// <summary>
    /// RenameDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RenameDialog : Window
    {
        private string? _replayNameWithoutExtension;

        public string? ReplayNameWithoutExtension
        {
            get
            {
                return _replayNameWithoutExtension;
            }

            set
            {
                _replayNameWithoutExtension = value;
                ReplayNameBox.Text = value;
            }
        }

        public RenameDialog()
        {
            InitializeComponent();

            ReplayNameBox.Focus();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            if (ReplayNameBox.Text.Length > 0)
            {
                this.ReplayNameWithoutExtension = ReplayNameBox.Text;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(this, "何か入力してください。", "リプレイファイルのリネーム",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ReplayNameBox.Focus();
            }
        }
    }
}
