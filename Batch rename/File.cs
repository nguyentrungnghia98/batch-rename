using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Batch_rename
{
    public class File:  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string error = "";

        public string Name { get; set; }

        public string NewName { get; set; }

        public string FileName
        {
            get
            {
                int index = this.NewName.LastIndexOf('.');
                Debug.Write("index" + index);
                if(index >= 0)
                {
                    //file
                    return this.NewName.Substring(0, index).ToUpper();
                }
                else
                {
                    //folder
                    return this.NewName.ToUpper();
                }
                
            }            
        }

        public string Path { get; set; }

        public string Error { get { return this.error; } set { this.error = value; } }

        public File Clone()
        {
            return new File()
            {
                Name = Name,
                NewName = NewName,
                Path = Path,
                Error = Error
            };
        }
    }
}
