using System.Windows;
using System.Windows.Controls;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for RecordView.xaml
    /// </summary>
    public partial class RecordView : UserControl
    {
        public RecordView()
        {
            InitializeComponent();
        }

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
        }
    }
}
