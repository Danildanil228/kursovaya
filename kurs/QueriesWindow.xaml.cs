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



namespace CarRentalApp
{
    public partial class QueriesWindow : Window
    {
        public QueriesWindow()
        {
            InitializeComponent();
        }

        private void QueriesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (QueriesListBox.SelectedItem is System.Windows.Controls.ListBoxItem selectedItem)
            {
                string tag = selectedItem.Tag as string;
                Window queryWindow = null;

                switch (tag)
                {
                    case "CarReport":
                        queryWindow = new CarReportWindow();
                        break;
                }

                queryWindow?.Show();
            }
        }
    }
}

