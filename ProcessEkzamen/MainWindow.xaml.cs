using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using Path = System.IO.Path;

namespace ProcessEkzamen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> files = new List<string>();
        List<string> words = new List<string>();
        List<string> filesWithWords = new List<string>();
        
        public MainWindow()
        {
            InitializeComponent();
            //заглушка, додати вибір файлу, або ввід тексту
            words.Add("one");
            words.Add("two");

        }

        private void Open_file_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;


            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                files = Directory.GetFiles(dlg.FileName, "*.txt").ToList();
                tbFolder.Text = dlg.FileName;
                lbTest.DataContext = null;
                lbTest.DataContext = files;
            }
        }

        static object obj = new object();

        //має бути асинк
        private void FindFilesWithWords(object path)
        {
            //lock (obj)
            //{
            string text = File.ReadAllText(path.ToString());
            foreach (var item in words)
            {
                if (text.Contains(item))
                    filesWithWords.Add(path.ToString());
            }

            lbFilesWith.DataContext = null;
            lbFilesWith.DataContext = filesWithWords;
            //}
        }


        //має бути асинк
        private void FileCopy(string source, string pathToCopy)
        {
            File.Copy(source, pathToCopy,true);
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (var item in files)
            {
                FindFilesWithWords(item);
            }
            
            //має бути асинк
            foreach (var oldPath in filesWithWords)
            {

                string new_path = tbFolder_Copy.Text;
                new_path += @"\" + Path.GetFileName(oldPath);
                FileCopy(oldPath, new_path);
                //додати шифрування ***
                ChangeWords(new_path);
            }
        }

        private void ChangeWords(string new_path)
        {
            string text = File.ReadAllText(new_path);
            string result = null;

            foreach (var item in words)
            {
                string pattern = item;
                result = Regex.Replace(text, pattern, "*******");
                text = result;
            }
            string directory = Path.GetDirectoryName(new_path);
            directory += @"\"+Path.GetFileNameWithoutExtension(new_path);
            directory += "_replaced.txt";
            File.WriteAllText(directory, result);
        }

        //choose folder where copy file
        private void FolderCopy_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;


            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                tbFolder_Copy.Text = dlg.FileName;
            }
        }
    }
}
