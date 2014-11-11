using System;
using System.Collections.Generic;
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
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using ArchiveExtractor.Model;
using ArchiveExtractor.Tools;
using SharpCompress;

namespace ArchiveExtractor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool booleanAmoi = true;

        public MainWindow()
        {
            InitializeComponent();
            
            if (Folder.GetInstance() != null)
                //Folder.GetInstance().Path = "D:\\TV Shows\\Doctor Who";
            this.folderRibbonTextBox.Text = Folder.GetInstance().Path;
            this.folderRibbonTextBox.UpdateLayout();

            this.fillLanguageRibbonCBBox();
            this.fillTagsRibbonCBBox();
            this.fillTheList(Folder.GetInstance().Path);
        }

        private void fillTheList(string path)
        {
            File item;
            List<File> items = new List<File>();
            string[] videoExtensions = { ".avi", ".mkv", ".mp4" };
            string[] files = this.getFilesWithFilter(videoExtensions, path);
            foreach (string file in files)
            {
                item = new File() { Selected = false, Name = file.ToString(), Size = 10.0, HasArchive = false, HasRelease = false };
                items.Add(item);
                //Format.formatFilename(item, )
                item.checkYearInSeriesName();
            }

            this.dataGrid.ItemsSource = items;
            this.dataGrid.UpdateLayout();
        }

        private void checkArchives_Click(object sender, RoutedEventArgs e)
        {
            if (this.LanguageRibbonGallery.SelectedItem == null || this.TagsRibbonGallery.SelectedItem == null)
            //if (this.LanguageRibbonGallery.SelectedItem == null)
            {
                if (this.LanguageRibbonGallery.SelectedItem == null && this.TagsRibbonGallery.SelectedItem != null)
                    System.Windows.MessageBox.Show("Il faut selectionner une langue.");
                else if (this.TagsRibbonGallery.SelectedItem == null && this.LanguageRibbonGallery.SelectedItem != null)
                    System.Windows.MessageBox.Show("Il faut selectionner une version tag ou notag.");
                else
                    System.Windows.MessageBox.Show("Il faut selectionner une langue et une version tag ou notag.");
            }
            else
            {
                string[] archiveExtensions = { ".rar", ".zip", };
                string[] archiveFiles = this.getFilesWithFilter(archiveExtensions, Folder.GetInstance().Path);
                if (archiveFiles.Length > 0)
                {
                    
                    foreach (string archive in archiveFiles)
                    {
                        if (archiveFiles.Length > 1)
                        {
                            string archiveFileName = archive.Remove(archive.IndexOf(Folder.GetInstance().Path), Folder.GetInstance().Path.Length + 1);
                            this.checkArchiveFileName(archiveFileName);
                            //System.Windows.MessageBox.Show("Archive : " + archiveFileName);
                        }

                        using (Stream stream = System.IO.File.OpenRead(@archive))
                        {
                            var reader = SharpCompress.Reader.ReaderFactory.Open(stream);
                            
                            int i = 0;
                            string displayArchive = "";
                            while (reader.MoveToNextEntry())
                            {
                                if (formatAndCheckArchivedFiles(reader.Entry.FilePath))
                                displayArchive += i.ToString() + ": " + reader.Entry.FilePath + "\n";
                                i++;
                            }
                        }
                    }

                }
            }

        }

        private void checkArchiveFileName(string archiveFileName) {
            string serie = "", episode = "", season = "", serieAndEp = "";
            string regexArchiveFilename = "\\d{1,}x\\d{2}";
            string[] substrings;
            Match match;

            // Get Serie name and Season + Episode in two separate strings
            Regex regex = new Regex(regexArchiveFilename, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            substrings = regex.Split(archiveFileName);
            //System.Windows.MessageBox.Show(substrings[0]);
            if (substrings.Length > 0)
            {
                serie = substrings[0];

                serieAndEp = substrings[1].Substring(1);

                regex = new Regex("[0-9]{1,}(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                match = regex.Match(serieAndEp);
                if (match.Success)
                {
                    // Get Season number
                    regex = new Regex("(?i)e[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    regex = new Regex("(?i)s", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(substrings[0]);
                    season = substrings[1];

                    // Get Episode number in two digits (in string)
                    regex = new Regex("(?i)s[0-9]{2}(?i)e", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    episode = substrings[1];
                }
            }
            //System.Windows.MessageBox.Show(serie + episode + season);
        }

        private bool formatAndCheckArchivedFiles(string filePath)
        {
            bool found = false;
            string serie="", episode="", season="", quality="", release="", serieAndEp = "";
            string[] substrings;
            Regex regex;
            Match match;

            // Get Serie name and Season + Episode in two separate strings
            regex = new Regex("(\\.[0-9]{3,})", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            substrings = regex.Split(filePath);
            if (substrings.Length > 0)
            {
                serie = substrings[0];
                serieAndEp = substrings[1].Substring(1);
                season = serieAndEp.Remove(1);
                episode = serieAndEp.Substring(1);
            }
            else
            {
                System.Windows.MessageBox.Show("Pas Ok");
            }

            // Get the quality of the show
            regex = new Regex("[0-9]{3,}p", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            match = regex.Match(filePath);
            if (match.Success)
            {
                substrings = regex.Split(filePath);
                
                quality = filePath.Replace(substrings[0], String.Empty);
                quality = quality.Replace(substrings[1], String.Empty);

            }
            else
            {
                quality = "";
            }

            //Get the release of the file
            if (quality != "")
                release = substrings[1].Substring(1);
            else
            {
                if (filePath.IndexOf("hdtv") == -1)
                    release = filePath.Replace(serie + "." + serieAndEp + ".", String.Empty);
                else
                    release = filePath.Replace(serie + "." + serieAndEp + ".hdtv.", String.Empty);
            }
            if (release.IndexOf('.') != -1)
                release = release.Remove(release.IndexOf('.'));
            else
                release = "";
            //System.Windows.MessageBox.Show(release);

            

            if (booleanAmoi)
            {
                // Check if there is corresponding language and tag/notag version with the choice made in the application
                if (filePath.Contains(this.LanguageRibbonGallery.SelectedItem.ToString()))
                {
                    //System.Windows.MessageBox.Show("Il y a le langage.");
                }
                if (filePath.Contains(this.TagsRibbonGallery.SelectedItem.ToString()))
                {
                    //System.Windows.MessageBox.Show("Il y a le tag.");
                }
                //System.Windows.MessageBox.Show(this.tagSelectorCBBox.SelectedItem.ToString());
                booleanAmoi = false;
            }

            foreach (File item in dataGrid.Items)
            {
                if (serie.Equals(item.Serie) && Convert.ToInt32(season).Equals(Convert.ToInt32(item.Season)) && episode.Equals(item.Episode))
                {
                    item.HasArchive = true;
                }

                if (serie.Equals(item.Serie) && Convert.ToInt32(season).Equals(Convert.ToInt32(item.Season)) && episode.Equals(item.Episode) && quality.Equals(item.Quality) && release.Equals(item.Release))
                {
                    item.HasRelease = true;
                    //item.FilenameArchive = filePath;
                    found = true;
                }
            }
            dataGrid.Items.Refresh();

            return found;
        }

        private void folderPath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                System.Windows.Controls.TextBox txtBox = new System.Windows.Controls.TextBox();
                if (sender.GetType().Equals(txtBox.GetType()))
                {
                    txtBox = (System.Windows.Controls.TextBox)sender;
                    DirectoryInfo dirEntered = new DirectoryInfo(txtBox.Text);
                    if (dirEntered.Exists)
                    {
                        Folder.GetInstance().Path = txtBox.Text;
                        this.fillTheList(Folder.GetInstance().Path);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(txtBox.Text+ " n'est pas un dossier existant. Merci de choisir un dossie existant.");
                    }
                }
            }
        }

        private string[] getFilesWithFilter(string[] filter, string path)
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => filter.Contains(System.IO.Path.GetExtension(f).ToLower())).ToArray();

            return files;
        }

        private void fillLanguageRibbonCBBox()
        {
            string fr = "VF", en = "EN";
            this.LanguageRibbonGalleryCategory.ItemsSource = new List<string>() { fr, en };
            this.LanguageRibbonGallery.SelectedItem = fr;
        }

        private void fillTagsRibbonCBBox()
        {
            string tag = "Tag", notag = "NoTag";
            this.TagsRibbonGalleryCategory.ItemsSource = new List<string>() { tag, notag };
            this.TagsRibbonGallery.SelectedItem = notag;
        }

        private void extractArchives_Click(object sender, RoutedEventArgs e)
        {
            foreach (File item in this.dataGrid.Items)
            {
                if (item.Selected)
                {
                    string[] archiveExtensions = { ".rar", ".zip", };
                    string[] archiveFiles = this.getFilesWithFilter(archiveExtensions, Folder.GetInstance().Path);
                    if (archiveFiles.Length > 0)
                    {
                        foreach (string archive in archiveFiles)
                        {
                            //System.Windows.MessageBox.Show(archive);
                            // Checks the good extension file
                            if (archive.EndsWith(".zip"))
                            {
                                // Open the .zip file
                                using (ZipArchive zipArchive = ZipFile.OpenRead(archive))
                                {
                                    // Browse the files in the archive
                                    foreach(ZipArchiveEntry entry in zipArchive.Entries)
                                    {
                                        if (entry.FullName.Equals(item.FilenameArchive))
                                        {
                                            if (entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                                            {
                                                //System.Windows.MessageBox.Show(Folder.GetInstance().Path + "\\" + item.Name.Remove(item.Name.IndexOf(item.Extension)) + ".srt");
                                                entry.ExtractToFile(Folder.GetInstance().Path + "\\" + item.Name.Remove(item.Name.IndexOf(item.Extension)) + ".srt", true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void folderRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath.Length > 0)
            {
                DirectoryInfo dir = new DirectoryInfo(fbd.SelectedPath);
                Folder.GetInstance().Path = fbd.SelectedPath;
                this.folderRibbonTextBox.Text = fbd.SelectedPath;
                this.folderRibbonTextBox.UpdateLayout();
                this.fillTheList(Folder.GetInstance().Path);

            }
        }

        private void folderRibbonTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                System.Windows.Controls.Ribbon.RibbonTextBox txtBox = new System.Windows.Controls.Ribbon.RibbonTextBox();
                if (sender.GetType().Equals(txtBox.GetType()))
                {
                    txtBox = (System.Windows.Controls.Ribbon.RibbonTextBox)sender;
                    DirectoryInfo dirEntered = new DirectoryInfo(txtBox.Text);
                    if (dirEntered.Exists)
                    {
                        Folder.GetInstance().Path = txtBox.Text;
                        this.fillTheList(Folder.GetInstance().Path);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(txtBox.Text + " n'est pas un dossier existant. Merci de choisir un dossie existant.");
                    }
                }
            }
        }

        private void selectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (File item in this.dataGrid.Items)
            {
                item.Selected = true;
            }
            this.dataGrid.Items.Refresh();
        }

        private void deselectAll_Click(object sender, RoutedEventArgs e)
        {

            foreach (File item in this.dataGrid.Items)
            {
                item.Selected = false;
            }
            this.dataGrid.Items.Refresh();
        }

        private void selectArchives_Click(object sender, RoutedEventArgs e)
        {

            foreach (File item in this.dataGrid.Items)
            {
                if (item.HasRelease)
                    item.Selected = true;
            }
            this.dataGrid.Items.Refresh();
        }
    }
}
