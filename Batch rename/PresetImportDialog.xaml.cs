using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PresetImportDialog.xaml
    /// </summary>
    public partial class PresetImportDialog : Window
    {
        public BindingList<StringOperation> _actions;
        public string ReturnData;
        public BindingList<StringOperation> ReturnActions;

        public PresetImportDialog(BindingList<StringOperation> actions)
        {
            InitializeComponent();

            _actions = new BindingList<StringOperation>(actions.Select(action => {
                StringOperation newAction = action.Clone();
                newAction.isActive = true;
                return newAction;
            }).ToList());

            methodsList.ItemsSource = _actions;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnActions = new BindingList<StringOperation>();
            foreach (StringOperation action in _actions)
            {
                if (action.isActive) ReturnActions.Add(action);
            }
            ReturnData = "success";
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            ReturnData = "cancel";
            DialogResult = true;
        }
    }
}
