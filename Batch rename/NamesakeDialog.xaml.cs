using System;
using System.Collections.Generic;
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
    /// Interaction logic for NamesakeDialog.xaml
    /// </summary>
    
    public partial class NamesakeDialog : Window
    {
        public string ReturnData;

        public NamesakeDialog()
        {
            InitializeComponent();
        }

        private void BtnAddSuffixes_Click(object sender, RoutedEventArgs e)
        {
            ReturnData = "suffixes";
            DialogResult = true;
            
        }

        private void BtnSkipDuplicate_Click(object sender, RoutedEventArgs e)
        {
            ReturnData = "skip";
            DialogResult = true;
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReturnData = "cancel";
            DialogResult = true;
            
        }
    }
}
