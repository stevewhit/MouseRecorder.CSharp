using System.Windows;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for PromptYesNoCancel.xaml
    /// </summary>
    public partial class PromptYesNoCancel : Window
    {
        public enum DialogAnswer
        {
            Yes,
            No,
            Cancel
        }

        public DialogAnswer SelectedChoice { get; set; }

        public PromptYesNoCancel(string question, string title, Window owner = null)
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
            var prompt = new PromptYesNoCancel(question, title);
            prompt.ShowDialog();

            if (prompt.DialogResult == true)
                return prompt.SelectedChoice;

            return DialogAnswer.Cancel;
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedChoice = DialogAnswer.Cancel;
            DialogResult = true;
            Close();
        }
    }
}
