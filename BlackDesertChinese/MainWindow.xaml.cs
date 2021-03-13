using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlackDesertChinese
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        string enPath = @".\BlackDesertChinese\en\languagedata_en.loc";
        string zhPath = @".\BlackDesertChinese\zh\languagedata_en.loc";
        string fontPath = @".\BlackDesertChinese\font\pearl.ttf";

        string enAimPath = @".\ads\languagedata_en.loc";
        string fontAimPath = @".\prestringtable\font\";

        public MainWindow()
        {
            InitializeComponent();
            IsInGamePath();
            TxtNote.Text = @"
使用本程序启动黑色沙漠steam美服可以免去手动替换汉化文件的麻烦。
程序会自动修复用中文语言文件后文字显示空白的问题。

重要：
请先开好游戏加速器和steam后再运行本程序。
本程序务必放到黑色沙漠游戏根目录运行。


文件夹说明:
BlackDesertChinese\en     文件夹里面放最新的英文语言文件，游戏更新了,请点击更新英文文件按钮(游戏更新完点一次就可以)。

BlackDesertChinese\zh     文件夹里面放最新的中文语言文件，有新版汉化自己替换新的进去。

BlackDesertChinese\font   文件夹里面放支持中文的字体文件，一般不用管。


dn.blackdesert.com.tw/UploadData/ads/languagedata_tw.loc（台服中文语言文件,下载后文件名要重命名为 languagedata_en.loc）

qq群：529696365。群主会定时更新汉化文件，然后把 languagedata_en.loc 复制到 BlackDesertChinese\zh 目录即可。

https://github.com/nnn149/BlackDesertChinese/releases  (项目地址)
";


        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = false;
            BtnStart.Content = "正在启动游戏...";

            SetFront();
            ResetLanguagedata();
            Process.Start("steam://rungameid/582660");

            Task.Factory.StartNew(() =>
            {
                bool isRun = false;
                for (int i = 0; i < 60; i++)
                {
                    log("检测是否启动成功...第" + (i + 1) + "次");
                    Thread.Sleep(1000);
                    Process[] processList = Process.GetProcessesByName("BlackDesertPatcher32.pae");
                    if (processList.Length > 0)
                    {
                        log("启动成功");
                        isRun = true;
                        break;
                    }

                }
                if (isRun == false)
                {
                    log("启动失败");
                    return;
                }
                log("检测是否开始游戏");
                while (true)
                {
                    log(".", false);
                    Thread.Sleep(300);
                    Process[] processList = Process.GetProcessesByName("BlackDesertPatcher32.pae");
                    if (processList.Length < 1)
                    {
                        AutoUpdateEnLanguagedata();
                        SetLanguagedata();
                        break;
                    }
                }
                log("游戏运行成功");
                Thread.Sleep(3000);
                Environment.Exit(0);

            });

        }

        private void SetFront()
        {
            if (!File.Exists(fontAimPath + "pearl.ttf"))
            {
                log("字体文件不存在，复制字体文件");
                if (!Directory.Exists(fontAimPath))
                {
                    Directory.CreateDirectory(fontAimPath);
                }
                File.Copy(fontPath, fontAimPath + "pearl.ttf", true);
            }

            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Black Desert\GameOption.txt";
            if (!File.Exists(iniPath))
            {
                MessageBox.Show("未检测到游戏配置文件，若是新安装的游戏，请用steam直接运行游戏一次(要登陆到游戏里面)", "错误");
                Application.Current.Shutdown();
                return;
            }
            string GameOption = File.ReadAllText(iniPath);
            GameOption = Regex.Replace(GameOption, @"UIFontType = (\d{1})", "UIFontType = 0");
            log("更改字体文件");
            File.WriteAllText(iniPath, GameOption);
        }


        private void ResetLanguagedata()
        {
            log("恢复英文文件");
            File.Copy(enPath, enAimPath, true);

        }
        private void SetLanguagedata()
        {
            log("设置中文文件");
            File.Copy(zhPath, enAimPath, true);
        }



        private void log(string str, bool LF = true)
        {
            this.TxtNote.Dispatcher.Invoke(() =>
            {
                if (LF)
                {
                    TxtNote.Text = TxtNote.Text + "\n" + str;
                }
                else
                {
                    TxtNote.Text = TxtNote.Text + str;
                }
                TxtNote.ScrollToEnd();
            });
        }
        private void IsInGamePath()
        {
            if (!File.Exists(@".\BlackDesertLauncher.exe"))
            {
                MessageBox.Show("请将本程序 (BlackDesertChinese.exe) 和 BlackDesertChinese 文件夹 复制到游戏根目录后运行", "提示");
                Application.Current.Shutdown();
                return;
            }

            if (!Directory.Exists("./BlackDesertChinese"))
            {
                MessageBox.Show("请将 BlackDesertChinese 文件夹 复制到游戏根目录后运行", "提示");
                Application.Current.Shutdown();
                return;
            }

            if (!File.Exists(zhPath))
            {
                MessageBox.Show("中文语言文件不存在", "提示");
                Application.Current.Shutdown();
                return;
            }
            if (!File.Exists(enPath))
            {
                MessageBox.Show("英文语言文件不存在", "提示");
                Application.Current.Shutdown();
                return;
            }
            if (!File.Exists(fontPath))
            {
                MessageBox.Show("字体文件不存在", "提示");
                Application.Current.Shutdown();
                return;
            }
        }

        private void BtnUpdateLanguagedata_Click(object sender, RoutedEventArgs e)
        {
            log("更新英文文件");
            File.Copy(enAimPath, enPath, true);
            MessageBox.Show("更新成功");
        }
        private void AutoUpdateEnLanguagedata()
        {
            if (!IsSameFile(enPath, enAimPath))
            {
                log("发现新版英文语言文件，已自动更新");
                File.Copy(enAimPath, enPath, true);
            }

        }

        private bool IsSameFile(string path1, string path2)
        {
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(path1, FileMode.Open), file2 = new FileStream(path2, FileMode.Open))
                {
                    return (BitConverter.ToString(hash.ComputeHash(file1)) == BitConverter.ToString(hash.ComputeHash(file2)));
                }
            }
        }
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/nnn149/BlackDesertChinese/releases");
        }

        private void BtnTW_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"下载后文件名要重命名为 languagedata_en.loc，然后把 languagedata_en.loc 复制到 BlackDesertChinese\zh 目录即可");
            Process.Start("http://dn.blackdesert.com.tw/UploadData/ads/languagedata_tw.loc");
            Process.Start("explorer.exe", @".\BlackDesertChinese\zh\");
        }
    }
}
