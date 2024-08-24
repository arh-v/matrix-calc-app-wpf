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

namespace PROECT
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        static Dictionary<string, string> styles = new Dictionary<string, string>()
        {
            { "Светлая", "light"},
            { "Тёмная", "dark"},
            { "Пользовательская", "user"}
        };
        Dictionary<string, string> styled_elements = new Dictionary<string, string>()
        {
            { "Цвет фона 1", "BackgroundColor1"},
            { "Цвет фона 2", "BackgroundColor2"},
            { "Цвет текста", "ForegroundColor"},
            { "Цвет границ", "BorderBrushColor"}
        };
        Dictionary<string, byte> element = new Dictionary<string, byte>()
        {
            { "BackgroundColor1", 0},
            { "BackgroundColor2", 1},
            { "ForegroundColor", 2},
            { "BorderBrushColor", 3}
        };
        Settings settings;
        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            styleBox.ItemsSource = styles.Keys;
            elementsBox.ItemsSource = styled_elements.Keys;
            elementsBox.SelectedItem = "Цвет фона 1";
            styleBox.SelectedItem = settings.Theme;
            SetSliderVisibility();
            Change_slidersValue();
            styleBox.SelectionChanged += StyleBox_SelectionChanged;
        }

        private void StyleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.Theme = styleBox.SelectedItem as string;
            ThemeChange();
            elementsBox.SelectedItem = elementsBox.SelectedItem;
        }
        public static void ThemeChange(Settings settings)
        {
            string style = styles[settings.Theme];
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();

            if (style == "user")
            {
                ResourceDictionary rd = new ResourceDictionary();

                rd.Add("BackgroundColor1", new SolidColorBrush(settings.Slider_settings[0].Color));
                rd.Add("BackgroundColor2", new SolidColorBrush(settings.Slider_settings[1].Color));
                rd.Add("ForegroundColor", new SolidColorBrush(settings.Slider_settings[2].Color));
                rd.Add("BorderBrushColor", new SolidColorBrush(settings.Slider_settings[3].Color));

                Application.Current.Resources.MergedDictionaries.Add(rd);
                Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("style.xaml", UriKind.Relative)) as ResourceDictionary);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(style + ".xaml", UriKind.Relative)) as ResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("style.xaml", UriKind.Relative)) as ResourceDictionary);
            }
        }
        private void ThemeChange()
        {
            ThemeChange(settings);
            SetSliderVisibility();
        }
        private void SetSliderVisibility()
        {
            switch (styleBox.SelectedItem.ToString())
            {
                case "Пользовательская":
                    Border1.Visibility = Visibility.Visible;
                    elementsBox.SelectionChanged += ElementsBox_SelectionChanged;
                    redSlider.ValueChanged += Slider_ValueChanged;
                    greenSlider.ValueChanged += Slider_ValueChanged;
                    blueSlider.ValueChanged += Slider_ValueChanged;
                    break;
                default:
                    Border1.Visibility = Visibility.Collapsed;
                    elementsBox.SelectionChanged -= ElementsBox_SelectionChanged;
                    redSlider.ValueChanged -= Slider_ValueChanged;
                    greenSlider.ValueChanged -= Slider_ValueChanged;
                    blueSlider.ValueChanged -= Slider_ValueChanged;
                    break;
            }
        }

        private void ElementsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Change_slidersValue();
        }

        private void Change_slidersValue()
        {
            string resource = styled_elements[elementsBox.SelectedItem as string];
            redSlider.Value = settings.Slider_settings[element[resource]].Color.R;
            greenSlider.Value = settings.Slider_settings[element[resource]].Color.G;
            blueSlider.Value = settings.Slider_settings[element[resource]].Color.B;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;
            string resource = styled_elements[elementsBox.SelectedItem as string];
            Color clr = ((SolidColorBrush)Application.Current.Resources.MergedDictionaries[0][resource]).Color;
            switch (s.Name)
            {
                case "redSlider":
                    clr.R = (byte)s.Value;
                    settings.Slider_settings[element[resource]].Color = clr;
                    break;
                case "greenSlider":
                    clr.G = (byte)s.Value;
                    settings.Slider_settings[element[resource]].Color = clr;
                    break;
                case "blueSlider":
                    clr.B = (byte)s.Value;
                    settings.Slider_settings[element[resource]].Color = clr;
                    break;
            }
            Application.Current.Resources.MergedDictionaries[0][resource] = new SolidColorBrush(clr);
        }

        private void Button_UserDefault_Click(object sender, RoutedEventArgs e)
        {
            settings.Slider_settings[0] = new StyledElement("BackgroundColor1", Colors.White);
            settings.Slider_settings[1] = new StyledElement("BackgroundColor2", Color.FromArgb(100, 229, 229, 229));
            settings.Slider_settings[2] = new StyledElement("ForegroundColor", Colors.Black);
            settings.Slider_settings[3] = new StyledElement("BorderBrushColor", Color.FromArgb(100, 172, 172, 172));

            ThemeChange();
            Change_slidersValue();
        }
    }
}
