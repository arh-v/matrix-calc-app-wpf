using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xml.Serialization;
using MyClass = WpfLibrary1.MyClass;
using Matrix = WpfLibrary1.Matrix;
using System.Windows.Controls.Primitives;

namespace PROECT
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings;
        ListBox history = new ListBox();

        public MainWindowViewModel mainWindowViewModel;
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel(GridMatrA, GridMatrB, GridMatrC);
            for (int i=0; i < 3; i++)
            {
                mainWindowViewModel.MatrA.Add(new MyClass() { MValues = new ObservableCollection<string> { null, null, null } });
                mainWindowViewModel.MatrB.Add(new MyClass() { MValues = new ObservableCollection<string> { null, null, null } });
                mainWindowViewModel.MatrC.Add(new MyClass() { MValues = new ObservableCollection<string> { null, null, null } });
            }
            this.DataContext = mainWindowViewModel;
            Load();
        }
        private void Load()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream("settings.xml", FileMode.OpenOrCreate))
                {
                    settings = xmlSerializer.Deserialize(fs) as Settings;
                }
                this.Width = settings.Width;
                this.Height = settings.Height;
                this.Top = settings.Top;
                this.Left = settings.Left;
            }
            catch
            {
                StyledElement[] slider_settings = new StyledElement[]
                {
                    new StyledElement("BackgroundColor1", Colors.White),
                    new StyledElement("BackgroundColor2", Color.FromArgb(100, 229, 229, 229)),
                    new StyledElement("ForegroundColor", Colors.Black),
                    new StyledElement("BorderBrushColor", Color.FromArgb(100, 172, 172, 172))
                };
                settings = new Settings("Светлая", slider_settings, Width, Height, Top, Left);
            }
            SettingsWindow.ThemeChange(settings);
        }

        private void ButtonSwap_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<MyClass> tmp_Matr = new ObservableCollection<MyClass>(mainWindowViewModel.MatrA);
            mainWindowViewModel.MatrA = new ObservableCollection<MyClass>(mainWindowViewModel.MatrB);
            mainWindowViewModel.MatrB = tmp_Matr;
            GridMatrA.ItemsSource = mainWindowViewModel.MatrA;
            GridMatrB.ItemsSource = mainWindowViewModel.MatrB;
        }

        private void ButtonUpsize_Click(object sender, RoutedEventArgs e)
        {
            foreach (ObservableCollection<MyClass> M in new ObservableCollection<ObservableCollection<MyClass>> { mainWindowViewModel.MatrA, mainWindowViewModel.MatrB, mainWindowViewModel.MatrC })
            {
                ObservableCollection<string> MValues_tmp = new ObservableCollection<string>();

                while (MValues_tmp.Count < M.Count)
                    MValues_tmp.Add(null);
                M.Add(new MyClass() { MValues = MValues_tmp });

                foreach (MyClass m in M)
                    m.MValues.Add(null);
            }
        }

        private void ButtonDownsize_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindowViewModel.MatrA.Count < 3)
                return;
            mainWindowViewModel.MatrA.RemoveAt(mainWindowViewModel.MatrA.Count - 1);
            mainWindowViewModel.MatrB.RemoveAt(mainWindowViewModel.MatrB.Count - 1);
            mainWindowViewModel.MatrC.RemoveAt(mainWindowViewModel.MatrC.Count - 1);

        }

        private void Button_OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow(settings).ShowDialog();
        }

        private void Button_OpenHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow hw = new HistoryWindow(history);
            hw.Owner = this;
            hw.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            settings.Height = Height;
            settings.Width = Width;
            settings.Top = Top;
            settings.Left = Left;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            using (FileStream fs = new FileStream("settings.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(fs, settings);
            }
        }

        private void Button_Sum_Click(object sender, RoutedEventArgs e)
        {
            Matrix matrixA = new Matrix(mainWindowViewModel.MatrA);
            Matrix matrixB = new Matrix(mainWindowViewModel.MatrB);

            mainWindowViewModel.MatrC = matrixA + matrixB;
            GridMatrC.ItemsSource = mainWindowViewModel.MatrC;
            if (mainWindowViewModel.MatrC[0].MValues[0] != null)
                History_Add("+");
        }

        private void Button_Minus_Click(object sender, RoutedEventArgs e)
        {
            Matrix matrixA = new Matrix(mainWindowViewModel.MatrA);
            Matrix matrixB = new Matrix(mainWindowViewModel.MatrB);

            mainWindowViewModel.MatrC = matrixA - matrixB;
            GridMatrC.ItemsSource = mainWindowViewModel.MatrC;
            if (mainWindowViewModel.MatrC[0].MValues[0] != null)
                History_Add("-");
        }

        private void Button_Multiply_Click(object sender, RoutedEventArgs e)
        {
            Matrix matrixA = new Matrix(mainWindowViewModel.MatrA);
            Matrix matrixB = new Matrix(mainWindowViewModel.MatrB);

            mainWindowViewModel.MatrC = matrixA * matrixB;
            GridMatrC.ItemsSource = mainWindowViewModel.MatrC;
            if (mainWindowViewModel.MatrC[0].MValues[0] != null)
                History_Add("x");
        }

        private void History_Add(string op)
        {
            ObservableCollection<MyClass> tmp_a = Matrix.Copy(mainWindowViewModel.MatrA);
            ObservableCollection<MyClass> tmp_b = Matrix.Copy(mainWindowViewModel.MatrB);
            ObservableCollection<MyClass> tmp_c = Matrix.Copy(mainWindowViewModel.MatrC);
            DataGrid dg_a = new DataGrid() { ItemsSource = tmp_a, Style = (Style)Application.Current.Resources.MergedDictionaries[1]["DataGridStyle"], IsReadOnly=true};
            DataGrid dg_b = new DataGrid() { ItemsSource = tmp_b, Style = (Style)Application.Current.Resources.MergedDictionaries[1]["DataGridStyle"], IsReadOnly=true};
            DataGrid dg_c = new DataGrid() { ItemsSource = tmp_c, Style = (Style)Application.Current.Resources.MergedDictionaries[1]["DataGridStyle"], IsReadOnly=true};
            foreach(DataGrid dg in new DataGrid[] { dg_a, dg_b, dg_c })
                for (int i = 0; i < tmp_a.Count; i++)
                    dg.Columns.Add(new DataGridTextColumn() { Binding = new Binding($"MValues[{i}]") });
            UniformGrid ug = new UniformGrid() { Columns = 5 };
            ug.Children.Add(dg_a);
            ug.Children.Add(new Viewbox() { Child = new TextBlock() { Text = op, Style = (Style)Application.Current.Resources.MergedDictionaries[1]["TextBlockStyle"] } });
            ug.Children.Add(dg_b);
            ug.Children.Add(new Viewbox() { Child = new TextBlock() { Text = "=", Style = (Style)Application.Current.Resources.MergedDictionaries[1]["TextBlockStyle"] } });
            ug.Children.Add(dg_c);
            history.Items.Add(ug);
            
        }

        private void Button_Transp_Click(object sender, RoutedEventArgs e)
        {
            Matrix.Transp(((Button)sender).Name == "Transp1" ? mainWindowViewModel.MatrA : mainWindowViewModel.MatrB);
        }
    }

    public class MainWindowViewModel
    {
        ObservableCollection<MyClass> matrA;
        ObservableCollection<MyClass> matrB;
        ObservableCollection<MyClass> matrC;
        public ObservableCollection<MyClass> MatrA { get => matrA; set { matrA = value; MatrA.CollectionChanged += MatrA_CollectionChanged; } }
        public ObservableCollection<MyClass> MatrB { get => matrB; set { matrB = value; MatrB.CollectionChanged += MatrB_CollectionChanged; } }
        public ObservableCollection<MyClass> MatrC { get => matrC; set { matrC = value; MatrC.CollectionChanged += MatrC_CollectionChanged; } }

        private DataGrid MatrA_DG;
        private DataGrid MatrB_DG;
        private DataGrid MatrC_DG;

        public MainWindowViewModel(DataGrid matrA_DG, DataGrid matrB_DG, DataGrid matrC_DG)
        {
            MatrA_DG = matrA_DG;
            MatrB_DG = matrB_DG;
            MatrC_DG = matrC_DG;
            MatrA = new ObservableCollection<MyClass>();
            MatrB = new ObservableCollection<MyClass>();
            MatrC = new ObservableCollection<MyClass>();
        }

        private void MatrA_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Change_Matr(MatrA_DG, sender, e);
        }
        private void MatrB_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Change_Matr(MatrB_DG, sender, e);
        }
        private void MatrC_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Change_Matr(MatrC_DG, sender, e);
        }
        private void Change_Matr(DataGrid Matr, object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action.ToString())
            {
                case "Add":
                    Matr.Columns.Add(new DataGridTextColumn() { Binding = new Binding($"MValues[{e.NewStartingIndex}]") });
                    /*DataTemplate dt = new DataTemplate();
                    FrameworkElementFactory vb = new FrameworkElementFactory(typeof(Viewbox));
                    FrameworkElementFactory tb = new FrameworkElementFactory(typeof(TextBlock));
                    tb.SetBinding(TextBlock.TextProperty, new Binding($"MValues[{e.NewStartingIndex}]"));
                    vb.AppendChild(tb);
                    dt.VisualTree = vb;
                    Matr.Columns.Add(new DataGridTemplateColumn() { CellTemplate = dt});*/

                    break;
                case "Remove":
                    Matr.Columns.RemoveAt(e.OldStartingIndex);
                    foreach (MyClass m in (ObservableCollection<MyClass>)sender)
                        m.MValues.RemoveAt(e.OldStartingIndex);
                    break;
            }
        }
    }

    public class Settings
    {
        public string Theme { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public StyledElement[] Slider_settings { get; set; }
        public Settings() { }
        public Settings(string theme, StyledElement[] slider_settings, double width, double height, double top, double left)
        {
            Theme = theme;
            Slider_settings = slider_settings;
            Width = width;
            Height = height;
            Top = top;
            Left = left;
        }
    }
    public class StyledElement
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public StyledElement() { }
        public StyledElement(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
