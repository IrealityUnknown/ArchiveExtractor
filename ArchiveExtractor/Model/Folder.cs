using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveExtractor.Model
{
    public class Folder
    {
        private static Folder instance = null;
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private Folder()
        {
            this._path = "C:\\";
        }

        public static Folder GetInstance()
        {
            if (instance == null)
            {
                instance = new Folder();
            }
            return instance;
        }

    }
}
