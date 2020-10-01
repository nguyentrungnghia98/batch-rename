using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Batch_rename
{
    public class StringHelper
    {
        private static StringHelper instance;
        public static StringHelper Instance
        {
            get
            {
                if (instance == null) instance = new StringHelper();
                return instance;
            }
        }
        private StringHelper() { }

        public string UpperCaseFirstLetter(string origin)
        {
            string result = "";
            for (int i = 0; i < origin.Length; i++)
            {
                if (i == 0 || origin[i - 1] == ' ')
                {
                    result += (origin[i].ToString()).ToUpper();
                }
                else
                {
                    result += (origin[i].ToString()).ToLower();
                }
            }
            return result;
        }
    }


    public class StringArgs
    {

    }

    public class ReplaceArgs: StringArgs, INotifyPropertyChanged
    {
        public string From { get; set; }
        public string To { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class NewCaseArgs: StringArgs, INotifyPropertyChanged
    {
        public string Type { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MoveArgs: StringArgs, INotifyPropertyChanged
    {
        public string Position { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class StringOperation : INotifyPropertyChanged
    {
        public StringOperation()
        {
            isActive = true;
        }

        public bool isActive { get; set; }

        public StringArgs Args { get; set; }

        public abstract string Operate(string origin);

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract StringOperation Clone();

        public abstract void Config();

        public abstract string ExportText();

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ReplaceOperation: StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Name => "Replace";

        public override string Description
        {
            get
            {
                var args = Args as ReplaceArgs;
                return $"Replace from {args.From} to {args.To}";
            }
        }

        public override string ExportText()
        {
            var args = Args as ReplaceArgs;
            return $"ReplaceOperation---{args.From}---{args.To}";
        }


        public override string Operate(string origin)
        {
            var args = Args as ReplaceArgs;
            var from = args.From;
            var to = args.To;

            if (string.IsNullOrEmpty(from)) return origin;

            return origin.Replace(from, to);
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as ReplaceArgs;
            return new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = oldArgs.From,
                    To = oldArgs.To
                }
            };
        }

        public override void Config()
        {
            Console.WriteLine("Config Replace");
        }
    }

    public class NewCaseOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Name => "New Case";

        public List<String> typesNewCase => new List<String> { "Uppercase", "Downcase", "Capitalize" };

        public void TriggerUpdateDescription()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
        }

        public override string ExportText()
        {
            var args = Args as NewCaseArgs;
            return $"NewCaseOperation---{args.Type}";
        }

        public override string Description
        {
            get
            {   
                var args = Args as NewCaseArgs;

                switch (args.Type)
                {
                    case "Uppercase":
                        return "Uppercase all character";
                    case "Downcase":
                        return "Downcase all character";
                    case "Capitalize":
                        return "Uppercase the first letter in each word";
                    default:
                        return "";
                }                
            }
        }

        public override string Operate(string origin)
        {
            var args = Args as NewCaseArgs;
            
            switch (args.Type)
            {
                case "Uppercase":
                    return origin.ToUpper();
                case "Downcase":
                    return origin.ToLower();
                case "Capitalize":                    
                    return StringHelper.Instance.UpperCaseFirstLetter(origin);
                default:
                    return "";
            }
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as NewCaseArgs;
            return new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    Type = oldArgs.Type                  
                }
            };
        }

        public override void Config()
        {
            Console.WriteLine("Config Replace");
        }
    }      

    public class FullNameNormalizeOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Name => "Fullname Normalize";

        public override string Description
        {
            get
            {
                return "No space in the start and the end, just one space between two word. Uppercase the first letter in each word";
            }
        }

        public override string ExportText()
        {
            return "FullNameNormalizeOperation";
        }

        public override string Operate(string origin)
        {
            int index = origin.LastIndexOf('.');
            string fileName, fileType;
            if(index >=0)
            {
                //file
                fileName = origin.Substring(0, index);
                fileType = origin.Substring(index);
            }
            else
            {
                fileName = origin;
                fileType = "";
            }
            string removeSpace = Regex.Replace(fileName, @"\s+", " "); 
            if(removeSpace[0] == ' ') removeSpace = removeSpace.Remove(0, 1);
            if (removeSpace[removeSpace.Length - 1] == ' ') removeSpace = removeSpace.Remove(removeSpace.Length - 1, 1);

            return StringHelper.Instance.UpperCaseFirstLetter(removeSpace) + fileType;
        }

        public override StringOperation Clone()
        {           
            return new FullNameNormalizeOperation(){};
        }

        public override void Config()
        {
            Console.WriteLine("Config Replace");
        }
    }

    public class UniqueNameOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Name => "Unique Name";

        public override string Description
        {
            get
            {
                return "Convert all files to Unique Name.";
            }
        }

        public override string ExportText()
        {
            return "UniqueNameOperation";
        }

        public override string Operate(string origin)
        {
            int index = origin.LastIndexOf('.');
            string fileName, fileType;
            if (index >= 0)
            {
                //file
                fileName = origin.Substring(0, index);
                fileType = origin.Substring(index);
            }
            else
            {
                fileName = origin;
                fileType = "";
            }
                        
            return fileName + '-' + string.Format(@"{0}{1}", Guid.NewGuid(), fileType);
        }

        public override StringOperation Clone()
        {
            return new UniqueNameOperation() { };
        }

        public override void Config()
        {
            Console.WriteLine("Config Replace");
        }
    }

    public class MoveOperation : StringOperation, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string Name => "Move";

        public List<String> typesPosition => new List<String> { "After", "Before" };

        public void TriggerUpdateDescription()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
        }

        public override string ExportText()
        {
            var args = Args as MoveArgs;
            return $"MoveOperation---{args.Position}";
        }

        public override string Description
        {
            get
            {
                var args = Args as MoveArgs;
                if (args.Position == "After")
                    return "Move ISBN number from the start to the end.\nIf filename length is less than 13, It will be ignored.";
                else return "Move ISBN number from the end to the start.\nIf filename length is less than 13, It will be ignored.";
            }
        }

        public override string Operate(string origin)
        {
            var args = Args as MoveArgs;
            int index = origin.LastIndexOf('.');
            string fileName, fileType;
            if (index >= 0)
            {
                //file
                fileName = origin.Substring(0, index);
                fileType = origin.Substring(index);
            }
            else
            {
                fileName = origin;
                fileType = "";
            }

            if (fileName.Length <= 13) return origin;

            if (args.Position == "After")
            {
                string ISBN = fileName.Substring(0, 13), name = fileName.Substring(14);
                return name + ' ' + ISBN + fileType;
            }
            else {
                int indexISBN = fileName.Length - 1 - 13;
                string ISBN = fileName.Substring(indexISBN), name = fileName.Substring(0, indexISBN - 1);
                return ISBN + ' ' + name + fileType;
            }          
        }

        public override StringOperation Clone()
        {
            var oldArgs = Args as MoveArgs;

            return new MoveOperation() {
                Args = new MoveArgs()
                {
                    Position = oldArgs.Position
                }
            };
        }

        public override void Config()
        {
            Console.WriteLine("Config Replace");
        }
    }
}
