using System.Collections.Generic;
using System.Windows;
using ThLaunchSite.Plugin;

namespace ThLaunchSite.MARISA
{
    public class MarisaMain : GameScoreFilesPluginBase
    {
        public override string Name => "MARISA for �����ǐ���";

        public override string Version => "0.1.0-beta";

        public override string Developer => "�쉹䝔�/�����ǐ����J����";

        public override string Description => "�����ǐ��������̃��v���C�t�@�C�������Ǘ��c�[���ł��B";

        public override string CommandName => "���v���C�t�@�C���̊Ǘ�(MARISA)";

        public Window? MainWindow { get; set; }

        public override void Main(List<string> availableGamesList, 
            Dictionary<string, string> availableGameScoreFilesDictionary)
        {
            ManageReplayFilesDialog manageReplayFilesDialog = new()
            {
                AvailableGamesList = availableGamesList,
                AvailableGameScoreFilesDictionary = availableGameScoreFilesDictionary
            };

            if (this.MainWindow != null) manageReplayFilesDialog.Owner = this.MainWindow;
            manageReplayFilesDialog.ShowDialog();
        }
    }

}
