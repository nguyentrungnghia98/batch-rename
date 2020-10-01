using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Batch_rename
{
    /// <summary>
    /// Interaction logic for PresetExportDialog.xaml
    /// </summary>
    public partial class PresetExportDialog : Window
    {
        BindingList<StringOperation> _actions;
        public string ReturnData;

        public PresetExportDialog(BindingList<StringOperation> actions)
        {
            InitializeComponent();

            _actions = new BindingList<StringOperation> (actions.Select(action => {
                StringOperation newAction = action.Clone();
                newAction.isActive = action.isActive;
                return newAction;
            }).ToList());

            methodsList.ItemsSource = _actions;
        }

        private void SelectPathNameButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save an Txt";
            saveFileDialog.FileName = "Save methods.txt";
            saveFileDialog.Filter = "Text File | *.txt";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                pathNameText.Text = saveFileDialog.FileName;

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnData = "cancel";
            DialogResult = true;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pathNameText.Text))
            {
                MessageBox.Show("Please select path to save export file!", "Warning");
                return;
            };
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathNameText.Text))
                {
                    foreach (StringOperation action in _actions)
                    {
                        if (action.isActive) file.WriteLine(action.ExportText());
                    }
                }
                ReturnData = "success";
                DialogResult = true;
            }
            catch (DivideByZeroException err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }
    }
}
