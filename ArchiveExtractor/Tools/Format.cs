using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArchiveExtractor.Tools
{
    public static class Format
    {
        private static bool test = true;
        private static string filename;
        private static string[] separators = { ".", " ", "-", "_" };
        private static string regexYearInSeriesName = "\\d{4}.(?i)s\\d{2}(?i)e\\d{2}|\\d{4}.\\d{1,}(?i)x\\d{2}|\\d{4}.\\d{3,}[^p]";
        private static string regexNoYearInSeriesName = "(?i)s\\d{2}(?i)e\\d{2}|\\d{1,}(?i)x\\d{2}|\\d{3,}[^p]";

        public static void formatFilename(File file, string regexString, int index)
        {
            string[] regEpAndSeasonSeparated;
            string[] substrings;
            string filename = file.Name;
            string serieAndEp = "";
            Match match, match2;

            if (index == 1)
            {
                Regex regDelYear = new Regex("\\d{4}[^p]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                Match m = regDelYear.Match(file.Name);
                if (m.Success)
                {
                    substrings = regDelYear.Split(file.Name);
                    filename = substrings[0] + substrings[1];
                    //filename = file.Name.Substring(file.Name.IndexOf(substrings[0]), 5) + file.Name.Remove(file.Name.IndexOf(substrings[0]), substrings[0].Length + 4);
                    System.Windows.MessageBox.Show(substrings[0] + " - " + substrings[1]);
                }
            }
            // Get the extension of the file
            file.Extension = filename.Substring(filename.Length - 4);

            // Get Serie name and Season + Episode in two separate strings
            //Regex regex = new Regex("(?i)s[0-9]{2}(?i)e[0-9]{2}|[0-9]{1,}(?i)x[0-9]{2}|(?<=20[0-9]{2}).\\d{3,}[^p]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Regex regex = new Regex(regexString, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            substrings = regex.Split(filename);
            if (substrings.Length > 1)
            {
                file.Serie = substrings[0];

                //Get Season and Episode before removing separator in Serie
                serieAndEp = filename.Remove(filename.IndexOf(file.Serie), file.Serie.Length);
                //if (index == 1) System.Windows.MessageBox.Show("Serie & Ep : " + serieAndEp + " - regexString : " + regexString);
                serieAndEp = serieAndEp.Remove(serieAndEp.IndexOf(substrings[1]));


                foreach (string separator in separators)
                {
                    regex = new Regex("[" + separator + "]$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    match = regex.Match(file.Serie);
                    if (match.Success)
                    {
                        file.Serie = file.Serie.Remove(file.Serie.LastIndexOf(separator));
                    }
                }

                regex = new Regex("\\d{4}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                match = regex.Match(file.Serie);
                if (match.Success)
                {
                    file.Serie = file.Serie.Remove(file.Serie.Length - 4);
                }

                if (test)
                {
                    //System.Windows.MessageBox.Show(file.Serie);
                    test = false;
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
                                file.Season = substrings[1];

                                // Get Episode number in two digits (in string)
                                regex = new Regex("(?i)s[0-9]{2}(?i)e", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                file.Episode = substrings[1];

                                //file.Season = serieAndEp.Remove(1,1);
                                break;
                            case "[0-9]{1,}(?i)x[0-9]{2}":
                                // Get Season number
                                regex = new Regex("(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                file.Season = substrings[0];

                                // Get Episode number in two digits (in string)
                                regex = new Regex("[0-9]{1,}(?i)x", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                substrings = regex.Split(serieAndEp);
                                file.Episode = substrings[1];
                                break;
                            case "\\d{3,}[^p]":

                                file.Season = serieAndEp.Remove(1);
                                file.Episode = serieAndEp.Substring(1);
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
                    file.Season = substrings[1];

                    // Get Episode number in two digits (in string)
                    regex = new Regex("(?i)s[0-9]{2}(?i)e", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    file.Episode = substrings[1];
                }
                else if (match2.Success)
                {
                    // Get Season number
                    regex = new Regex("(?i)x[0-9]{2}", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    file.Season = substrings[0];

                    // Get Episode number in two digits (in string)
                    regex = new Regex("[0-9]{1,}(?i)x", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                    substrings = regex.Split(serieAndEp);
                    file.Episode = substrings[1];
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
                file.Quality = filename.Replace(substrings[0], String.Empty);
                file.Quality = file.Quality.Replace(substrings[1], String.Empty);
            }
            else
            {
                file.Quality = "";
            }

            //Get the release of the file
            if (filename.LastIndexOf("- ") == -1 && filename.LastIndexOf('-') != -1)
            {
                file.Release = filename.Replace(filename.Remove(filename.LastIndexOf('-')), String.Empty).Substring(1);
                file.Release = file.Release.Remove(file.Release.LastIndexOf('.'));
            }
            else
            {
                file.Release = "";
            }

            if (file.Episode == "02")
            {
                //System.Windows.MessageBox.Show(filename.LastIndexOf('-').ToString() + " - " + filename.LastIndexOf("- ").ToString());
            }
        }

        public static void FormatAll(File file)
        {
            filename = file.Name;

            GetFileName(file);
            GetSeriesName(file);
            GetSeason(file);
            GetEpisode(file);
            GetQuality(file);
            GetReleaseName(file);
        }

        public static void GetFileName(File file)
        {
            int hasYearInSeriesName = 0;
            string[] substrings;
            Regex regexWithYear, regexWithoutYear;
            regexWithYear = new Regex(regexYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            regexWithoutYear = new Regex(regexNoYearInSeriesName, RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Match match = regexWithYear.Match(file.Name), match2 = regexWithoutYear.Match(file.Name);
            
            // Checks if the Serie is a "reboot", therefore contains the year of the reboot
            if (match.Success)          { hasYearInSeriesName = 1; }
            else if (match2.Success)    { hasYearInSeriesName = 2; }
            else                        { System.Windows.MessageBox.Show("Problème !");
                                          hasYearInSeriesName = 0; 
                                        }

            if (hasYearInSeriesName == 1)
            {
                Regex regDelYear = new Regex("\\d{4}[^p]", RegexOptions.CultureInvariant | RegexOptions.Compiled);
                Match m = regDelYear.Match(file.Name);
                if (m.Success)
                {
                    substrings = regDelYear.Split(file.Name);
                    filename = substrings[0] + substrings[1];
                    //filename = file.Name.Substring(file.Name.IndexOf(substrings[0]), 5) + file.Name.Remove(file.Name.IndexOf(substrings[0]), substrings[0].Length + 4);
                    System.Windows.MessageBox.Show(substrings[0] + " - " + substrings[1]);
                }

            }

        }

        public static void GetSeriesName(File file)
        {

        }

        public static void GetSeason(File file) { }

        public static void GetEpisode(File file) { }

        public static void GetQuality(File file) { }

        public static void GetReleaseName(File file) { }


    }
}
