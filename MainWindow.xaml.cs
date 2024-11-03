using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;

namespace lab_2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContextMenu directoryMenu { get; set; }
        private ContextMenu fileMenu { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            directoryMenu = CreateDirectoryMenu();
            fileMenu = CreateFileMenu();

            _treeView.SelectedItemChanged += SelectedItemChangedHandler;

            ShowTreeView("C:\\Users\\robiz\\Documents\\aaaatest");
        }
        private void SelectedItemChangedHandler(object sender, EventArgs e)
        {
            TreeViewItem item = (TreeViewItem)_treeView.SelectedItem;
            string path = item.Tag.ToString();

            _attributes.Text = "";

            FileAttributes attributes = File.GetAttributes(path);

            if(attributes.HasFlag(FileAttributes.ReadOnly))
            {
                _attributes.Text += "r";
            }
            if (attributes.HasFlag(FileAttributes.Archive))
            {
                _attributes.Text += "a";
            }
            if (attributes.HasFlag(FileAttributes.System))
            {
                _attributes.Text += "s";
            }
            if (attributes.HasFlag(FileAttributes.Hidden))
            {
                _attributes.Text += "h";
            }
        }
        private void OpenFolderDialog(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.ShowDialog();

            string path = dlg.SelectedPath;

            ShowTreeView(path);

        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
        private void ShowTreeView(string directoryPath)
        {
            TreeViewItem root = CreateTreeViewFromFiles(directoryPath);

            _treeView.Items.Clear();
            _treeView.Items.Add(root);
        }
        private TreeViewItem CreateTreeViewFromFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            var root = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName,
                ContextMenu = directoryMenu
            };

            foreach (var file in directory.GetFileSystemInfos())
            {
                TreeViewItem item;
                if (file is DirectoryInfo)
                {
                    item = CreateTreeViewFromFiles(file.FullName);
                }
                else
                {
                    item = new TreeViewItem
                        {
                            Header = file.Name,
                            Tag = file.FullName,
                            ContextMenu = fileMenu
                        };
                }
                root.Items.Add(item);

            }

            return root;
        }
        private ContextMenu CreateDirectoryMenu()
        {
            ContextMenu directoryMenu = new ContextMenu();

            MenuItem deleteItem = new MenuItem { Header = "Delete" };
            deleteItem.Click += DeleteItem;

            MenuItem createItem = new MenuItem { Header = "Create" };
            createItem.Click += CreateItem;

            directoryMenu.Items.Add(createItem);
            directoryMenu.Items.Add(deleteItem);

            return directoryMenu;
        }

        private ContextMenu CreateFileMenu()
        {
            ContextMenu directoryMenu = new ContextMenu();

            MenuItem deleteItem = new MenuItem { Header = "Delete" };
            deleteItem.Click += DeleteItem;

            MenuItem openFile = new MenuItem { Header = "Open" };
            openFile.Click += OpenFile;

            directoryMenu.Items.Add(openFile);
            directoryMenu.Items.Add(deleteItem);

            return directoryMenu;
        }
        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)item.Parent;
            TreeViewItem treeViewItem = (TreeViewItem)menu.PlacementTarget;
            string path = treeViewItem.Tag.ToString();

            FileAttributes attributes = File.GetAttributes(path);

            if(attributes.HasFlag(FileAttributes.ReadOnly))
            {
                attributes&= ~FileAttributes.ReadOnly;

                File.SetAttributes(path, attributes);
            }

            TreeViewItem parentOfTreeView = (TreeViewItem)treeViewItem.Parent;
            parentOfTreeView.Items.Remove(treeViewItem);

            if(Directory.Exists(path))
            {
                Directory.Delete(path,true);
                return;
            }
            File.Delete(path);
        }
        private void CreateItem(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)item.Parent;
            TreeViewItem treeViewItem = (TreeViewItem)menu.PlacementTarget;
            string path = treeViewItem.Tag.ToString();



            CreateFileForm form = new CreateFileForm();
            form.Show();
            form.path = path;
            form.treeViewItem = (TreeViewItem)treeViewItem;

            form.Closed += CreateItemHandler;

        }
        private void CreateItemHandler(object sender, EventArgs e)
        {
            var form = (CreateFileForm)sender;
            if (form.closed)
            {
                return;
            }

            string path = form.path;

            string name = form._textbox_name.Text;

            string filePath = path + "\\" + name;

            if ((bool)form._radio_file.IsChecked)
            {
                TreeViewItem item = new TreeViewItem { Header = name, Tag = filePath, ContextMenu = fileMenu };
                form.treeViewItem.Items.Add(item);
                File.Create(filePath);
            }
            else
            {
                TreeViewItem item = new TreeViewItem { Header = name, Tag = filePath, ContextMenu = directoryMenu };
                form.treeViewItem.Items.Add(item);
                Directory.CreateDirectory(path+form._textbox_name.Text);
            }
            File.SetAttributes(filePath, GetSelectedAttributes(form));
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            ContextMenu menu = (ContextMenu)item.Parent;
            TreeViewItem treeViewItem = (TreeViewItem)menu.PlacementTarget;
            string path = treeViewItem.Tag.ToString();

            string content = File.ReadAllText(path);

            _textbox_content.Text = content;
        }
        private FileAttributes GetSelectedAttributes(CreateFileForm form)
        {
            FileAttributes attributes = new FileAttributes();

            if ((bool)form._checkbox_readonly.IsChecked)
            {
                attributes |= FileAttributes.ReadOnly;
            }
            if ((bool)form._checkbox_system.IsChecked)
            {
                attributes |= FileAttributes.System;
            }
            if ((bool)form._checkbox_hidden.IsChecked)
            {
                attributes |= FileAttributes.Hidden;
            }
            if ((bool)form._checkbox_archive.IsChecked)
            {
                attributes |= FileAttributes.Archive;
            }

            return attributes;
        }
    }
}
