using System.Collections.Generic;
using System.Windows;
using ThLaunchSite.Plugin;

namespace ThLaunchSite.MARISA
{
    public class MarisaMain : GameScoreFilesPluginBase
    {
        public override string Name => "MARISA for 東方管制塔";

        public override string Version => "0.1.0-beta";

        public override string Developer => "珠音茉白/東方管制塔開発部";

        public override string Description => "東方管制塔向けのリプレイファイル統合管理ツールです。";

        public override string CommandName => "リプレイファイルの管理(MARISA)";

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
