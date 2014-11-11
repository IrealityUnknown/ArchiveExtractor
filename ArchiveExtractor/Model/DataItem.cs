using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiveExtractor.Model;

namespace ArchiveExtractor
{
    class DataItem : IEditableObject
    {
        private bool _selected;
        private string _name;
        private double _size;
        private string _extension;
        private bool _hasArchive;
        private bool _hasRelease;
        private string _serie;
        private string _season;
        private string _episode;
        private string _quality;
        private string _release;
        private string _filenameArchive;

        public string Serie
        {
            get { return _serie; }
            set { _serie = value; }
        }

        public string Season
        {
            get { return _season; }
            set { _season = value; }
        }

        public string Episode
        {
            get { return _episode; }
            set { _episode = value; }
        }

        public string Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        public string Release
        {
            get { return _release; }
            set { _release = value; }
        }


        public bool HasRelease
        {
            get { return _hasRelease; }
            set { _hasRelease = value; }
        }

        public bool HasArchive
        {
            get { return _hasArchive; }
            set { _hasArchive = value; }
        }

        public string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value.Replace(Folder.GetInstance().Path+"\\",""); }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public string FilenameArchive
        {
            get { return _filenameArchive; }
            set { _filenameArchive = value; }
        }

        public void BeginEdit()
        {
            //throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            //throw new NotImplementedException();
        }

        public void EndEdit()
        {
            //throw new NotImplementedException();
        }

        public void checkYearInSeriesName()
        {
            string regexYearInSeriesName = "\\d{4}.(?i)s\\d{2}(?i)e\\d{2}|\\d{4}.\\d{1,}(?i)x\\d{2}|\\d{4}.\\d{3,}[^p]";
            string regexNoYearInSeriesName = "(?i)s\\d{2}(?i)e\\d{2}|\\d{1,}(?i)x\\d{2}|\\d{3,}[^p]";
            Regex regexWithYear, regexWithoutYear;
            regexWithYear = new Regex(regexYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            regexWithoutYear = new Regex(regexNoYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Match match = regexWithYear.Match(this.Name), match2 = regexWithoutYear.Match(this.Name);
            if (match.Success)
            {
                this.formatFilename(regexNoYearInSeriesName, 1);
            }
            else if (match2.Success)
            {
                this.formatFilename(regexNoYearInSeriesName, 2);
            }
            else
            {
                System.Windows.MessageBox.Show("Problème !");
            }
        }

        public void formatFilename(string regexString, int index)
        {
            string regEpsAndSeasons = "(?i)s[0-9]{2}(?i)e[0-9]{2}|[0-9]{1,}(?i)x[0-9]{2}|\\d{3,}[^p]";
            string[] regEpAndSeasonSeparated;
            string[] separators = { ".", " ", "-", "_" };
            string[] substrings;
            string filename = this.Name;
            string serieAndEp = "";
            Match match, match2;

            if (index == 1)
            {
                Regex regDelYear = new Regex("\\d{4}[^p]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                Match m = regDelYear.Match(this.Name);
                if (m.Success)
                {
                    substrings = regDelYear.Split(this.Name);
                    filename = this.Name.Substring(this.Name.IndexOf(substrings[0]), 5) + this.Name.Remove(this.Name.IndexOf(substrings[0]), substrings[0].Length + 4);
                    System.Windows.MessageBox.Show("Filename : " + filename);
                }
            }
            // Get the extension of the file
            this.Extension = filename.Substring(filename.Length - 4);

            // Get Serie name and Season + Episode in two separate strings
            //Regex regex = new Regex("(?i)s[0-9]{2}(?i)e[0-9]{2}|[0-9]{1,}(?i)x[0-9]{2}|(?<=20[0-9]{2}).\\d{3,}[^p]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Regex regex = new Regex(regexString, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            substrings = regex.Split(filename);
            if (substrings.Length > 1)
            {
                this.Serie = substrings[0];

                //Get Season and Episode before removing separator in Serie
                serieAndEp = filename.Remove(filename.IndexOf(this.Serie), this.Serie.Length);
                if (index == 1)
                    System.Windows.MessageBox.Show("Serie & Ep : " + serieAndEp + " - regexString : " + regexString);
                serieAndEp = serieAndEp.Remove(serieAndEp.IndexOf(substrings[1]));
                

                foreach (string separator in separators)
                {
                    regex = new Regex("[" + separator + "]$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    match = regex.Match(this.Serie);
                    if (match.Success)
                    {
                        this.Serie = this.Serie.Remove(this.Serie.IndexOf(separator));
                    }
                }

                regex = new Regex("\\d{4}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                match = regex.Match(this.Serie);
                if (match.Success)
                {
                    this.Serie = this.Serie.Remove(this.Serie.Length - 4);
                }
                
                regEpAndSeasonSeparated = regexString.Split('|');

                foreach (string regEpAndSeason in regEpAndSeasonSeparated)
                {
                    regex = new Regex(regEpAndSeason, RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    match = regex.Match(serieAndEp);
                    if (match.Success)
                    {
                        switch (regEpAndSeason)
                        {
                            case "(?i)s[0-9]{2}(?i)e[0-9]{2}":
                                // Get Season number
                                regex = new Regex("(?i)e[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                regex = new Regex("(?i)s", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(substrings[0]);
                                this.Season = substrings[1];

                                // Get Episode number in two digits (in string)
                                regex = new Regex("(?i)s[0-9]{2}(?i)e", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                this.Episode = substrings[1];

                                //this.Season = serieAndEp.Remove(1,1);
                                break;
                            case "[0-9]{1,}(?i)x[0-9]{2}":
                                // Get Season number
                                regex = new Regex("(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                this.Season = substrings[0];

                                // Get Episode number in two digits (in string)
                                regex = new Regex("[0-9]{1,}(?i)x", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                this.Episode = substrings[1];
                                break;
                            case "\\d{3,}[^p]":
                                
                                this.Season = serieAndEp.Remove(1);
                                this.Episode = serieAndEp.Substring(1);
                                break;
                        }
                    }
                }
                
                
                regex = new Regex("(?i)s[0-9]{2}(?i)e[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                match = regex.Match(serieAndEp);
                // For next pattern of filenames
                regex = new Regex("[0-9]{1,}(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                match2 = regex.Match(serieAndEp);
                if (match.Success)
                {
                    // Get Season number
                    regex = new Regex("(?i)e[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    regex = new Regex("(?i)s", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(substrings[0]);
                    this.Season = substrings[1];
                    
                    // Get Episode number in two digits (in string)
                    regex = new Regex("(?i)s[0-9]{2}(?i)e", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    this.Episode = substrings[1];
                }
                else if (match2.Success) 
                {
                    // Get Season number
                    regex = new Regex("(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    this.Season = substrings[0];

                    // Get Episode number in two digits (in string)
                    regex = new Regex("[0-9]{1,}(?i)x", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    this.Episode = substrings[1];
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Pas Ok");
            }

            // Get the quality of the show
            regex = new Regex("[0-9]{3,}p", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            match = regex.Match(filename);
            if (match.Success)
            {
                substrings = regex.Split(filename);
                this.Quality = filename.Replace(substrings[0], String.Empty);
                this.Quality = this.Quality.Replace(substrings[1], String.Empty);
            }
            else
            {
                this.Quality = "";
            }

            //Get the release of the file
            if (filename.LastIndexOf("- ") == -1 && filename.LastIndexOf('-') != -1)
            {
                this.Release = filename.Replace(filename.Remove(filename.LastIndexOf('-')), String.Empty).Substring(1);
                this.Release = this.Release.Remove(this.Release.LastIndexOf('.'));
            }
            else
            {
                this.Release = "";
            }

            if (this.Episode == "02")
            {
                //System.Windows.MessageBox.Show(filename.LastIndexOf('-').ToString() + " - " + filename.LastIndexOf("- ").ToString());
            }
        }

        public string displayObject()
        {
            string display = Selected.ToString();
            display += "\nName: " + Name;
            display += "\nSize: " + Size.ToString();
            display += "\nArchive: " + HasArchive.ToString();
            display += "\nRelease: " + HasRelease.ToString();
            display += "\nSerie: " + Serie;
            display += "\nSeason: " + Convert.ToInt32(Season).ToString();
            display += "\nEpisode: " + Episode;
            display += "\nQuality: " + Quality;
            display += "\nRelease: " + Release;
            return display;
        }

    }
}
