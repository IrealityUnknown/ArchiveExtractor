using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiveExtractor.Model;
using ArchiveExtractor.Tools;

namespace ArchiveExtractor
{
    public class File : IEditableObject
    {
        #region -- Attributes -- 

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
        //private bool test = true;
        private bool _isSrtFile;

        #endregion

        #region -- Properties --

        /// <summary>
        /// Gets or sets the serie.
        /// </summary>
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

        //public bool Test
        //{
        //    get { return test; }
        //    set { test = value; }
        //}

        public bool IsSrtFile
        {
            get { return _isSrtFile; }
            set { _isSrtFile = value; }
        }

        #endregion

        #region -- Methods --

        #region -- From Interface To Edit Element in the list --
        
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

        #endregion

        public void FormatFile()
        {
            Format.FormatAll(this);
        }

        public int checkYearInSeriesName()
        {
            int result = 0;
            string regexYearInSeriesName = "\\d{4}.(?i)s\\d{2}(?i)e\\d{2}|\\d{4}.\\d{1,}(?i)x\\d{2}|\\d{4}.\\d{3,}[^p]";
            string regexNoYearInSeriesName = "(?i)s\\d{2}(?i)e\\d{2}|\\d{1,}(?i)x\\d{2}|\\d{3,}[^p]";
            Regex regexWithYear, regexWithoutYear;
            regexWithYear = new Regex(regexYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            regexWithoutYear = new Regex(regexNoYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Match match = regexWithYear.Match(this.Name), match2 = regexWithoutYear.Match(this.Name);
            if (match.Success)
            {
                //this.formatFilename(regexNoYearInSeriesName, 1);
                Format.formatFilename(this, regexNoYearInSeriesName, 1);
                result = 1;

            }
            else if (match2.Success)
            {
                //this.formatFilename(regexNoYearInSeriesName, 2);
                Format.formatFilename(this, regexNoYearInSeriesName, 2);
                result = 2;
            }
            else
            {
                System.Windows.MessageBox.Show("Problème !");
                result = 0;
            }

            return result;
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

        #endregion
    }
}
