using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace BlackDesertChinese
{
    class Program
    {
        static void Main(string[] args)
        {

            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Black Desert\GameOption.txt";
            string gamePath = File.ReadAllText("path.txt");

            if (!File.Exists(gamePath + @"\prestringtable\font\pearl.ttf"))
            {
                Console.WriteLine("字体文件不存在，复制字体文件");
                if (!Directory.Exists(gamePath + @"\prestringtable\font\"))
                {
                    Directory.CreateDirectory(gamePath + @"\prestringtable\font\");
                }
                File.Copy(@".\font\pearl.ttf", gamePath + @"\prestringtable\font\pearl.ttf", true);
            }

            string GameOption = File.ReadAllText(iniPath);
            GameOption = Regex.Replace(GameOption, @"UIFontType = (\d{1})", "UIFontType = 0");
            Console.WriteLine("更改字体");
            File.WriteAllText(iniPath, GameOption);
            Console.WriteLine("恢复英文");
            File.Copy(@".\en\languagedata_en.loc", gamePath + @"\ads\languagedata_en.loc", true);

            Process.Start("steam://rungameid/582660");

            bool isRun = false;
            for (int i = 0; i < 60; i++)
            {
                Console.WriteLine("检测是否启动成功...第" + (i + 1) + "次");
                Thread.Sleep(1000);
                Process[] processList = Process.GetProcessesByName("BlackDesertPatcher32.pae");
                if (processList.Length > 0)
                {
                    Console.WriteLine("启动成功");
                    isRun = true;
                    break;
                }

            }
            if (isRun == false)
            {
                Console.WriteLine("启动失败");
                return;
            }
            Console.WriteLine("检测是否开始游戏");
            while (true)
            {
                Console.Write(".");
                Thread.Sleep(300);
                Process[] processList = Process.GetProcessesByName("BlackDesertPatcher32.pae");
                if (processList.Length < 1)
                {
                    Console.WriteLine();
                    Console.WriteLine("设置中文");
                    File.Copy(@".\zh\languagedata_en.loc", gamePath + @"\ads\languagedata_en.loc", true);
                    break;
                }
            }
            Console.WriteLine("游戏运行成功,按任意键退出。bug联系qq:1041836312");
            Console.ReadKey();
        }
    }
}
