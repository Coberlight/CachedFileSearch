using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CachedFileSearch
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileCache;
        public MainWindow()
        {
            InitializeComponent();
            TextBlock_MatchesList.Text = "Сквазимабзабза\nБабынбубынбубз\nБубжидубжизус\nБалдымалдыбар\nБарындылдан\nКраблатубажидан\nАркынтырыксус\nМамзиказмамаба\nАроклапежус\nФисташканегречкашрёк\nПирдырдыл\nМамзаказибуба\nПралгабрадафныр\nПакыртылтынлдан\nАкралдыштус\nШвабрадалдан\nДырындилдан\nБуксушашан\nПараншран\nДилдалдан\nБрыгылыгыла\nЧучпекжизуз\nИлкылдылдан";
        }

        private async void Button_MakeCache_Click(object sender, RoutedEventArgs e)
        {
            // Подготовка аргументов для метода, подготовка логгера, создание задачи
            string location = TextBox_DirectoryLocation.Text;
            CachedFileSearchCore.SearchLog logger = SetLogMakeCacheMessage;
            Task<string> task;
            // исключение возникает в другом потоке, поэтому отловить его отсюда обычными средствами не получается
            //try
            //{
                task = new Task<string>(() => CachedFileSearchCore.MakeCache(new string[] { location.Trim('\"') }, 0, logger));
            //}
            //catch (Exception ex) { TextBlock_Info.Text = "Оп ахах неловко вышло\n" + ex.Message; return; }

            // Запуск задачи с измерением времени выполнения
            DateTime dt = DateTime.Now;
            task.Start();
            fileCache = await task;
            TextBlock_Info.Text = "Кэш создан. \nВремя сканирования: " + (DateTime.Now - dt).TotalMilliseconds.ToString() + " мс";

            // Показ результата
            TextBlock_FileSystemList.Text = fileCache;
        }

        private async void Button_SearchGo_Click(object sender, RoutedEventArgs e)
        {
            List<FileSystemInfo> fsis;

            // Подготовка аргументов для метода, подготовка логгера, создание задачи
            string fileCache = this.fileCache;
            string query = TextBox_Query.Text;
            bool enableCase = (bool)CheckBox_EnableCase.IsChecked;
            CachedFileSearchCore.SearchLog logger = SetLogSearchMessage;
            Task<List<FileSystemInfo>> task;
            //try
            //{
                task = new Task<List<FileSystemInfo>>(() => CachedFileSearchCore.FindMatches(query, ref fileCache, enableCase, logger));
            //}
            //catch (Exception ex) { TextBlock_Info.Text = "Оп ахах неловко вышло\n" + ex.Message; return; }

            // Запуск задачи с измерением времени выполнения
            DateTime dt = DateTime.Now;
            task.Start();
            fsis = await task;
            TextBlock_Info.Text = "Поиск завершён.\nВремя поиска: " + (DateTime.Now - dt).TotalMilliseconds.ToString() + " мс\nВсего результатов: " + fsis.Count;

            // Показ результата
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fsis.Count; i++)
            {
                if (fsis[i] is DirectoryInfo)
                {
                    sb.Append("Папка: ");
                }
                else if (fsis[i] is FileInfo)
                {
                    sb.Append("Файл: ");
                }
                else
                {
                    sb.Append("Чё за хрень воще: ");
                }
                sb.Append(fsis[i].FullName).Append('\n');
            }
            TextBlock_MatchesList.Text = sb.ToString();
        }
        private void SetLogMakeCacheMessage(string logMessage)
        {
            this.Dispatcher.Invoke(() => TextBlock_Info.Text = "Обрабатываемая директория:\n" + logMessage);
        }
        private void SetLogSearchMessage(string logMessage)
        {
            this.Dispatcher.Invoke(() => TextBlock_Info.Text = "Обрабатываемая директория:\n" + logMessage);
        }

        private void Button_LoadCache_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            if (fd.FileName != null && fd.FileName != "")
            {
                fileCache = File.ReadAllText(fd.FileName);
                TextBlock_FileSystemList.Text = fileCache;
            }
        }

        private void Button_SaveCache_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.ShowDialog();
            if (fd.FileName != null && fd.FileName != "")
            {
                File.WriteAllText(fd.FileName, fileCache);
            }
        }
    }
}
