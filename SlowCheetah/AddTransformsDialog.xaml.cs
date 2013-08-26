using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.ObjectModel;

using System.ComponentModel;

namespace SlowCheetah.VisualStudio
{
    /// <summary>
    /// Interaction logic for AddTransform.xaml
    /// </summary>
    public partial class AddTransformsDialog : Window
    {
        public ObservableCollection<FileExtension> ExtensionList { get; set; }
        public ObservableCollection<TreeViewModel> TransformsList { get; set; }

        public AddTransformsDialog(List<string> possibleTransformsToCreate)
        {
            InitializeComponent();

            ExtensionList = new ObservableCollection<FileExtension>();
            TransformsList = new ObservableCollection<TreeViewModel>();
            TransformsList.Add(new TreeViewModel("Possible Transforms"));
        }

        public void AddPossibleTransform(string possibleTransform)
        {
            TransformsList.First().Children.Add(new TreeViewModel(possibleTransform));
            treeView1.ItemsSource = TransformsList;
        }

        public void AddPossibleExtension(FileExtension extension)
        {
            ExtensionList.Add(extension);

            listExtensions.DataContext = ExtensionList;
        }

        private void createTransforms_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class FileExtension
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class TreeViewModel : INotifyPropertyChanged
    {
        public TreeViewModel(string name)
        {
            Name = name;
            Children = new List<TreeViewModel>();
        }

        public string Name { get; private set; }
        public List<TreeViewModel> Children { get; private set; }

        bool? _isChecked = false;
        TreeViewModel _parent;

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));
            }

            if (updateParent && _parent != null)
            {
                _parent.VerifyCheckedState();
            }

            NotifyPropertyChanged("IsChecked");
        }

        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        void Initialize()
        {
            foreach (TreeViewModel child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }


        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
