using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TranslationHelper.Model;
using TranslationHelper.View;

namespace TranslationHelper
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string TT;
        Crawl cr;
        private ImageReferenceView IMV = null;
        public MainWindow()
        {
            InitializeComponent();
            cr = new Crawl();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IMV = new ImageReferenceView();
            IMV.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string result;
            switch(module_combo.SelectedIndex)
            {
                case 0:
                    MessageBox.Show("1번 모듈. 현재는 영한사전만 지원됩니다.");
                    result = cr.GetWord(TT);
                    result_block.Text = result;
                    break;
                case 1:
                    //MessageBox.Show("2번 모듈.");
                    cr.GetIdiomReference(TT);
                    Process.Start(@"IdiomReference.txt");
                    break;
                case 2:
                    //MessageBox.Show("3번 모듈.");
                    result = cr.GetTranslatedText(TT, (start_combo.SelectedValue as ComboBoxItem).Content.ToString(), (object_combo.SelectedValue as ComboBoxItem).Content.ToString());
                    result_block.Text = result;
                    break;
                default:
                    MessageBox.Show("선택해주세요.");
                    break;
            }
        }

        private void input_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            TT = (sender as TextBox).Text;
        }
    }
}
