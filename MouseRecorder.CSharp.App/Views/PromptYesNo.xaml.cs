using System.Windows;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for PromptYesNo.xaml
    /// </summary>
    public partial class PromptYesNo : Window
    {
        public enum DialogAnswer
        {
            Yes,
            No,
            Close
        }

        public DialogAnswer SelectedChoice { get; set; }

        public PromptYesNo(string question, string title, Window owner = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                this.Owner = owner;
                this.Topmost = true;
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            txtQuestion.Text = question;
            Title = title;
        }

        public static DialogAnswer Prompt(string question, string title)
        {
            var prompt = new PromptYesNo(question, title);
            prompt.ShowDialog();

            if (prompt.DialogResult == true)
                return prompt.SelectedChoice;

            return DialogAnswer.Close;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            SelectedChoice = DialogAnswer.Yes;
            DialogResult = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            SelectedChoice = DialogAnswer.No;
            DialogResult = true;
            Close();
        }
    }
}
