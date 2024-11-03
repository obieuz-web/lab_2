using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lab_2
{
    /// <summary>
    /// Logika interakcji dla klasy CreateFileForm.xaml
    /// </summary>
    public partial class CreateFileForm : Window
    {
        public bool closed { get; set; } = false;
        public string path { get; set; }
        public TreeViewItem treeViewItem { get; set; }
        public CreateFileForm()
        {
            InitializeComponent();
        }
        public void CreateNewItem(object sender, RoutedEventArgs e)
        {
            if((bool)_radio_file.IsChecked)
            {
                string basename, extenstion;
                try
                {
                    basename = _textbox_name.Text.Split('.')[0];
                    extenstion = _textbox_name.Text.Split('.')[1];
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Nazwa pliku musi mieć rozszerzenie");
                    return;
                }

                string[] extenstions = { "txt", "php", "html" };

                if (!Regex.IsMatch(basename, "^[a-zA-Z0-9_~\\-]{1,8}$"))
                {
                    MessageBox.Show("Nazwa stanowi od 1 do 8 znaków (litery, cyfry, podkreślenie, tylda, minus)");
                    return;
                }

                bool doesExist = false;
                foreach (var item in extenstions)
                {
                    if (extenstion == item)
                    {
                        doesExist = true;
                        break;
                    }
                }
                if (!doesExist)
                {
                    MessageBox.Show("Dozwolone rozszerzenia to txt, php i html");
                    return;
                }
            }

            this.Close();
        }
        public void Cancel(object sender, RoutedEventArgs e)
        {
            closed = true;
            this.Close();
        }
    }
}
