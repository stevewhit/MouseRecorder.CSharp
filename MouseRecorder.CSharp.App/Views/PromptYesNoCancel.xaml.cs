using System.Windows;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for PromptYesNoCancel.xaml
    /// </summary>
    public partial class PromptYesNoCancel : Window
    {
        public enum PromptType
        {
            YesNo,
            YesNoCancel
        }

        public enum DialogAnswer
        {
            Yes,
            No,
            Cancel
        }

        public DialogAnswer SelectedChoice { get; set; }

        public PromptYesNoCancel(PromptType type, string question, string title, Window owner = null)
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

            if (type == PromptType.YesNo)
                BtnCancel.Visibility = Visibility.Collapsed;
        }

        public static DialogAnswer Prompt(PromptType type, string question, string title)
        {
            var prompt = new PromptYesNoCancel(type, question, title);
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
