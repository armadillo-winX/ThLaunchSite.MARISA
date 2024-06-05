using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ThLaunchSite.MARISA
{
    /// <summary>
    /// ManageReplayFilesDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ManageReplayFilesDialog : Window
    {
        private List<string>? _availableGamesList;
        private Dictionary<string, string>? _availableGameScoreFilesDictionary;

        public List<string>? AvailableGamesList
        {
            get
            {
                return _availableGamesList;
            }

            set
            {
                _availableGamesList = value;

                if (value != null && value.Count > 0)
                {
                    foreach (string gameId in value)
                    {
                        string gameName = GameIndex.GetGameName(gameId);
                        GamesListBox.Items.Add($"{gameId}: {gameName}");
                    }
                }
            }
        }

        public Dictionary<string, string>? AvailableGameScoreFilesDictionary
        {
            get
            {
                return _availableGameScoreFilesDictionary;
            }

            set
            {
                _availableGameScoreFilesDictionary = value;
            }
        }

        private string? ReplayDirectory { get; set; }

        public ManageReplayFilesDialog()
        {
            InitializeComponent();

            MarisaMain marisaMain = new();

            NameBlock.Text = "Multi Advanced Replay files Import and Sort Add-on for 東方管制塔";
            VersionBlock.Text = $"Version.{marisaMain.Version}";
            DeveloperBlock.Text = marisaMain.Developer;
            DescriptionBox.Text = @"MARISA for 東方管制塔
Multi Advanced Replay files Import and Sort Add-on for 東方管制塔
Copyright © 2024 珠音茉白/東方管制塔開発部

このツールは、東方管制塔 NX 向けのリプレイファイル統合管理プラグインです。
主な機能は以下の通りです。
※本ソフトウェアはベータ版であり、未実装の機能も多く含まれます。
・リプレイフォルダへのリプレイファイルの追加
・リプレイフォルダのリプレイファイルのリネーム・コピー・移動・削除
・複数のリプレイファイルを各作品のリプレイフォルダに一括で仕分け、追加";
        }

        private string? GetReplayDirectory(string? gameId)
        {
            if (!string.IsNullOrEmpty(gameId) && 
                this.AvailableGameScoreFilesDictionary != null &&
                this.AvailableGameScoreFilesDictionary.TryGetValue(gameId, out string? gameScoreFile))
            {
                string replayDirectory = $"{Path.GetDirectoryName(gameScoreFile)}\\replay";
                return replayDirectory;
            }
            else
            {
                return string.Empty;
            }
        }

        private string[]? GetReplayFiles(string? gameId)
        {
            string? replayDirectory = GetReplayDirectory(gameId);
            if (!string.IsNullOrEmpty(replayDirectory) && Directory.Exists(replayDirectory))
            {
                string[] replayFiles = Directory.GetFiles(replayDirectory, "*.rpy", SearchOption.TopDirectoryOnly);
                return replayFiles;
            }
            else
            {
                return null;
            }
        }

        private void AddReplayFile(string? gameId, string replayFile)
        {
            string replayDirectory = GetReplayDirectory(gameId);
            
            if (File.Exists($"{replayDirectory}\\{Path.GetFileName(replayFile)}"))
            {
                MessageBoxResult result =
                    MessageBox.Show(this, 
                    $"追加先に '{Path.GetFileName(replayFile)}' が既に存在します。\n上書きして追加しますか？", "確認",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    File.Delete($"{replayDirectory}\\{Path.GetFileName(replayFile)}");
                    File.Copy(replayFile, $"{replayDirectory}\\{Path.GetFileName(replayFile)}");
                }
                else
                {
                    MessageBox.Show(this, "キャンセルされました。", "リプレイファイルの追加",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                File.Copy(replayFile, $"{replayDirectory}\\{Path.GetFileName(replayFile)}");
            }
        }

        private void DeleteReplayFile(string? gameId, string? replayName)
        {
            string replayDirectory = GetReplayDirectory(gameId);

            MessageBoxResult result = MessageBox.Show(
                this, $"'{replayName}' を削除してもよろしいですか?", "リプレイファイルの削除",
                MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                File.Delete($"{replayDirectory}\\{replayName}");
            }
        }

        private void ShowReplayFiles()
        {
            ReplayFilesListBox.Items.Clear();

            string selectedItem = GamesListBox.SelectedItem as string;
            string gameId = selectedItem.Split(':')[0];
            string[]? replayFiles = GetReplayFiles(gameId);
            this.ReplayDirectory = GetReplayDirectory(gameId);

            if (replayFiles != null && replayFiles.Length > 0)
            {
                foreach (string replayFile in replayFiles)
                {
                    ReplayFilesListBox.Items.Add(Path.GetFileName(replayFile));
                }
            }
        }

        private void GamesListBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (GamesListBox.Items.Count > 0 && GamesListBox.SelectedIndex > -1)
            {
                ShowReplayFiles();
            } 
        }

        private void RenameButtonClick(object sender, RoutedEventArgs e)
        {
            if (GamesListBox.Items.Count > 0 && GamesListBox.SelectedIndex > -1 &&
                ReplayFilesListBox.Items.Count > 0 && ReplayFilesListBox.SelectedIndex > -1)
            {
                string selectedItem = GamesListBox.SelectedItem as string;
                string gameId = selectedItem.Split(':')[0];
                string replayDirectory = GetReplayDirectory(gameId);

                string replayFileName = ReplayFilesListBox.SelectedItem as string;

                string replayFile = $"{replayDirectory}\\{replayFileName}";

                RenameDialog renameDialog = new()
                {
                    Owner = this,
                    ReplayNameWithoutExtension = Path.GetFileNameWithoutExtension(replayFile)
                };

                if (renameDialog.ShowDialog() == true)
                {
                    try
                    {
                        string newReplayFileName = $"{renameDialog.ReplayNameWithoutExtension}.rpy";

                        if (!File.Exists($"{replayDirectory}\\{newReplayFileName}"))
                        {
                            File.Move(replayFile, $"{replayDirectory}\\{newReplayFileName}");

                            ShowReplayFiles();
                        }
                        else
                        {
                            MessageBox.Show(this, $"'{newReplayFileName}' は既に存在するので、リネームできません。", "ファイル名の重複",
                                MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "東方リプレイファイル|*.rpy",
                Title = "追加するリプレイファイルを選択してください"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedItem = GamesListBox.SelectedItem as string;
                string gameId = selectedItem.Split(':')[0];

                string replayFile = openFileDialog.FileName;

                try
                {
                    AddReplayFile(gameId, replayFile);

                    ShowReplayFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (GamesListBox.Items.Count > 0 && GamesListBox.SelectedIndex > -1 &&
                ReplayFilesListBox.Items.Count > 0 && ReplayFilesListBox.SelectedIndex > -1)
            {
                string selectedItem = GamesListBox.SelectedItem as string;
                string gameId = selectedItem.Split(':')[0];

                string replayFileName = ReplayFilesListBox.SelectedItem as string;

                try
                {
                    DeleteReplayFile(gameId, replayFileName);

                    ShowReplayFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
