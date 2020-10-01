using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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


namespace Batch_rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<File> files = new BindingList<File>();
        BindingList<File> folders = new BindingList<File>();
        List<StringOperation> _prototypes = new List<StringOperation>();
        BindingList<StringOperation> _actions = new BindingList<StringOperation>();
        string selectedFolderPath;
        string selectedFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Nạp CSDL để biết những khả năng đổi tên mình có thể
            var prototype1 = new ReplaceOperation()
            {
                Args = new ReplaceArgs()
                {
                    From = "From",
                    To = "To"
                }
            };

            var prototype2 = new NewCaseOperation()
            {
                Args = new NewCaseArgs()
                {
                    Type = "Capitalize"
                }
            };

            var prototype3 = new FullNameNormalizeOperation(){};

            var prototype4 = new UniqueNameOperation() { };

            var prototype5 = new MoveOperation() {
                Args = new MoveArgs()
                {
                    Position = "After"
                }
            };
            _prototypes.Add(prototype1);
            _prototypes.Add(prototype2);
            _prototypes.Add(prototype3);
            _prototypes.Add(prototype4);
            _prototypes.Add(prototype5);
            prototypesListView.ItemsSource = _prototypes;

            operationsListBox.ItemsSource = _actions;
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            GetItemsInFolder("file");
        }
        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            GetItemsInFolder("folder");
        }
        private void GetItemsInFolder(string type)
        {
            string SelectedFolderPath = "";
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                //SelectedFolderPath = Directory.Exists(dialog.FileName) ? dialog.FileName : System.IO.Path.GetDirectoryName(dialog.FileName);
                SelectedFolderPath = dialog.FileName;
                if(type == "file")
                {
                    selectedFilePath = SelectedFolderPath;
                    GetAllFilesInFolder(SelectedFolderPath);
                }
                else
                {
                    selectedFolderPath = SelectedFolderPath;
                    GetAllFoldersInFolder(SelectedFolderPath);
                }
            }
        }
        private void GetAllFilesInFolder(string path)
        {
  
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            FileInfo[] info = dirInfo.GetFiles("*.*");
            if (info.Length == 0) return;

            files = new BindingList<File>();
            foreach (FileInfo f in info)
            {
                files.Add(new File() { Name = f.Name, Path = path });
            }
            listFiles.ItemsSource = files;
     
        }

        private void GetAllFoldersInFolder(string path)
        {

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            DirectoryInfo[] info = dirInfo.GetDirectories();
            if (info.Length == 0) return;

            folders = new BindingList<File>();
            foreach (DirectoryInfo f in info)
            {
                folders.Add(new File() { Name = f.Name, Path = path });
            }
            listFolders.ItemsSource = folders;

        }
        
        private void excuteRename(string oldFile, string newFile, bool isFileMode)
        {
            if (isFileMode)
                System.IO.File.Move(oldFile, newFile);
            else
            {
                if (oldFile.ToLower() == newFile.ToLower())
                {
                    // uppercase or lowercase => cannot use move to change file name /new => /New => error!
                    // => dirty way to solve it lol!!!
                    string randomDir = newFile + Guid.NewGuid().ToString();
                    Directory.Move(oldFile, randomDir);
                    Directory.Move(randomDir, newFile);
                }
                else
                {
                    Directory.Move(oldFile, newFile);
                }
            }
        }

        private void renameFile(BindingList<File> items, bool isFileMode)
        {
            for (int i = 0; i < items.Count; i++)
            {
                String oldFile = items[i].Path + '/' + items[i].Name;
                String newFile = items[i].Path + '/' + items[i].NewName;
                if(oldFile != newFile)
                {
                    excuteRename(oldFile, newFile, isFileMode);
                }
                
            }
        }

        private void renameFileWithSkip(BindingList<File> items, bool isFileMode)
        {
            for (int i = 0; i < items.Count; i++)
            {
                bool check = true;
                for(int j = 0; j < items.Count; j++)
                {
                    if(i != j && items[i].FileName == items[j].FileName)
                    {
                        check = false;
                        break;
                    }
                }
                if (check)
                {
                    String oldFile = items[i].Path + '/' + items[i].Name;
                    String newFile = items[i].Path + '/' + items[i].NewName;

                    if (oldFile != newFile)
                    {
                        excuteRename(oldFile, newFile, isFileMode);
                    }
                } 
            }
        }

        private bool checkValidFiles(BindingList<File> files)
        {
            int length = files.Count;
            bool check = true;
            for (int i = 0; i < length - 1; i++)
            {
                for(int j = i + 1; j < length; j++)
                {
                    Debug.WriteLine(files[i].FileName + "-" + files[j].FileName);
                    if (files[i].FileName == files[j].FileName) {
                        check = false;
                        files[i].Error = "Duplicate";
                        files[j].Error = "Duplicate";
                        Debug.Write("Duplicate");
                    }
                }
            }
            return check;
        }

        private void AddSuffixes(BindingList<File> files)
        {
            int length = files.Count;
            for(int i = 0; i < length - 1; i++)
            {
                int count = 1;
                string origin = files[i].NewName;
                string originFileName = files[i].FileName;
                for (int j = 0; j < length; j++)
                {
                    if(files[j].FileName == originFileName && i != j)
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
                        if (count == 1) files[i].NewName = fileName + count + fileType;
                        count++;
                        files[j].NewName = fileName + count + fileType;
                    }
                }
            }
            listFiles.ItemsSource = files;
        }


        private void GetNewList(bool isFileMode)
        {
            if (isFileMode)
            {
                if (!string.IsNullOrEmpty(selectedFilePath)) GetAllFilesInFolder(selectedFilePath);
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedFolderPath)) GetAllFoldersInFolder(selectedFolderPath);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(_actions.Count == 0)
            {
                MessageBox.Show("Please add methods to continute!", "Warning");
                return;
            };
            BindingList<File> items = new BindingList<File>();
            bool isFileMode = tabControl.SelectedIndex == 0;
            if (isFileMode) items = files;
                else items = folders;
            
            if (items.Count == 0) {
                MessageBox.Show("Please add files to continute!", "Warning");
                return;
            };
            UpdateNewFileNameUI();
            if (checkValidFiles(items))
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Click Preview to check the result. Are you sure to continute?", "Start Batch Name", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Debug.Write("Start Batch name");
                    renameFile(items, isFileMode);
                    //get new list
                    GetNewList(isFileMode);

                }
            }
            else
            {
                Debug.Write("Error");
                var screen = new NamesakeDialog();
                if (screen.ShowDialog().Value == true)
                {
                    Debug.Write("return1",screen.ReturnData);
                    switch (screen.ReturnData)
                    {
                        case "suffixes":
                            AddSuffixes(items);
                            renameFile(items, isFileMode);
                            GetNewList(isFileMode);
                            break;
                        case "skip":
                            renameFileWithSkip(items, isFileMode);
                            GetNewList(isFileMode);
                            break;
                        default:
                            break;
                    }
                }
            }
            
        }



        private void BtnCloseModal_Click(object sender, RoutedEventArgs e)
        {
            modalContainer.Visibility = Visibility.Hidden;
        }

        private void AddMethodButton_Click(object sender, RoutedEventArgs e)
        {
            modalContainer.Visibility = Visibility.Visible;
        }

        private void BtnAddMethod_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            index = prototypesListView.SelectedIndex;
            Trace.WriteLine(index);
            var action = _prototypes[index];
            _actions.Add(action.Clone());

            modalContainer.Visibility = Visibility.Hidden;
        }

        private void typeNewCaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = operationsListBox.SelectedItem as NewCaseOperation;       
            item?.TriggerUpdateDescription();           
            
        }

        private void TypesPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = operationsListBox.SelectedItem as MoveOperation;
            item?.TriggerUpdateDescription();
        }

        private void UpdateNewFileNameUI()
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    UpdateNewName(files);                    
                    checkValidFiles(files);
                    listFiles.ItemsSource = files;
                    break;
                case 1:
                    UpdateNewName(folders);                    
                    checkValidFiles(folders);
                    listFolders.ItemsSource = folders;
                    break;
            }
        }

        private void UpdateNewName(BindingList<File> list)
        {
            foreach(File file in list)
            {
                string tmp = file.Name;
                foreach(StringOperation action in _actions)
                {
                    if(action.isActive)  tmp = action.Operate(tmp);
                }
                file.NewName = tmp;
            }
            Debug.Write("debug");
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateNewFileNameUI();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_actions.Count == 0)
            {
                MessageBox.Show("Please add methods!", "Warning");
                return;
            };
            var screen = new PresetExportDialog(_actions);
            if (screen.ShowDialog().Value == true)
            {
                Debug.Write("export close");
                if(screen.ReturnData == "success")
                {
                    MessageBox.Show("Save success!", "Success");
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedFilePath = "";
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                //SelectedFolderPath = Directory.Exists(dialog.FileName) ? dialog.FileName : System.IO.Path.GetDirectoryName(dialog.FileName);
                SelectedFilePath = dialog.FileName;
                if (!string.IsNullOrEmpty(SelectedFilePath))
                {
                    BindingList<StringOperation> actions = new BindingList<StringOperation>();
                    
                    using (StreamReader reader = new StreamReader(SelectedFilePath))
                    {
                        try
                        {
                            string currentLine;
                            while ((currentLine = reader.ReadLine()) != null)
                            {
                                string[] tokens = currentLine.Split(new[] { "---" }, StringSplitOptions.None);
                                Debug.Write(tokens[0]);
                                switch (tokens[0])
                                {                             
                                    case "ReplaceOperation":
                                        actions.Add(new ReplaceOperation()
                                        {
                                            Args = new ReplaceArgs()
                                            {
                                                From = tokens[1],
                                                To = tokens[2]
                                            }
                                        });
                                    break;
                                    case "NewCaseOperation":
                                        actions.Add(new NewCaseOperation()
                                        {
                                            Args = new NewCaseArgs()
                                            {
                                                Type = tokens[1]
                                            }
                                        });
                                        break;
                                    case "FullNameNormalizeOperation":
                                        actions.Add(new FullNameNormalizeOperation() { });
                                        break;
                                    case "UniqueNameOperation":
                                        actions.Add(new UniqueNameOperation() { });
                                        break;
                                    case "MoveOperation":
                                        actions.Add(new MoveOperation()
                                        {
                                            Args = new MoveArgs()
                                            {
                                                Position = tokens[1]
                                            }
                                        });
                                        break;
                                    default:
                                        break;
                                }
                            }
                            presetImportAction(actions);
                        }
                        catch (DivideByZeroException err)
                        {
                            MessageBox.Show(err.Message, "Error");
                        }
                        
                    }
                }
            }
        }

        private void presetImportAction(BindingList<StringOperation> actions)
        {
            if (actions.Count == 0)
            {
                MessageBox.Show("Empty methods!", "Warning");
                return;
            };
            var screen = new PresetImportDialog(actions);
            if (screen.ShowDialog().Value == true)
            {
                Debug.Write("export close");
                if (screen.ReturnData == "success")
                {
                    _actions = screen.ReturnActions;
                    operationsListBox.ItemsSource = _actions;                    
                }
            }
        }

        private void DeleteActionBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Method", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Debug.Write("Delete");
                int selectedIndex = operationsListBox.SelectedIndex;
                if(selectedIndex >= 0)  _actions.RemoveAt(selectedIndex);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedIndex == -1)
                foreach (object item in e.RemovedItems)
                {
                    listBox.SelectedItem = item;
                    break;
                }
                    
        }
        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = operationsListBox.SelectedIndex;
            if(selectedIndex > 0)
            {
                StringOperation tmp = _actions[selectedIndex];
                _actions[selectedIndex] = _actions[selectedIndex - 1];
                _actions[selectedIndex - 1] = tmp;
                operationsListBox.Items.Refresh();
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = operationsListBox.SelectedIndex;
            if (selectedIndex < _actions.Count - 1)
            {
                StringOperation tmp = _actions[selectedIndex];
                _actions[selectedIndex] = _actions[selectedIndex + 1];
                _actions[selectedIndex + 1] = tmp;
                operationsListBox.Items.Refresh();
            }
        }

        private void MoveToTopButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = operationsListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                StringOperation tmp = _actions[selectedIndex];
                _actions[selectedIndex] = _actions[0];
                _actions[0] = tmp;
                operationsListBox.Items.Refresh();
            }
        }

        private void MoveToBottomButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = operationsListBox.SelectedIndex;
            if (selectedIndex < _actions.Count - 1)
            {
                StringOperation tmp = _actions[selectedIndex];
                _actions[selectedIndex] = _actions[_actions.Count - 1];
                _actions[_actions.Count - 1] = tmp;
                operationsListBox.Items.Refresh();
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            _actions = new BindingList<StringOperation>();
            operationsListBox.ItemsSource = _actions;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            files = new BindingList<File>();
            folders = new BindingList<File>();
            _actions = new BindingList<StringOperation>();
            operationsListBox.ItemsSource = _actions;
            listFiles.ItemsSource = files;
            listFolders.ItemsSource = folders;
        }
    }
}

