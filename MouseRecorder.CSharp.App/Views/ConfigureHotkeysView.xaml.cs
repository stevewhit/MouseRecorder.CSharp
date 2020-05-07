using MouseRecorder.CSharp.App.ViewModel;
using System.Windows;
using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using Framework.Generic.Utility;
using System.Linq;
using System;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for ClickZoneView.xaml
    /// </summary>
    public partial class ConfigureHotkeysView : Window
    {
        private readonly ConfigureHotkeysViewModel _model;
        private IList<Forms.Keys> _pressedKeys;
        private Combination _defaultCombination;

        /// <summary>
        /// Build and displays a window prompt with the supplied parameters. Returns the selected key combination.
        /// </summary>
        /// <param name="promptTitle">The title of the window.</param>
        /// <param name="promptText">The text that is displayed in the prompt.</param>
        /// <param name="defaultCombination">The default key combination</param>
        /// <returns>Returns the selected key combination.</returns>
        public static Combination Prompt(string promptTitle, string promptText, Combination defaultCombination)
        {
            var model = new ConfigureHotkeysViewModel()
            {
                Title = promptTitle,
                PromptText = promptText,
                KeyCombinationString = defaultCombination.ToString()
            };

            var view = new ConfigureHotkeysView(model, defaultCombination);
            view.ShowDialog();

            // Return the selected combination.
            return view.GetSelectedCombination();
        }

        private ConfigureHotkeysView(ConfigureHotkeysViewModel model, Combination defaultCombination)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _defaultCombination = defaultCombination ?? throw new ArgumentNullException(nameof(defaultCombination));

            InitializeComponent();

            this.DataContext = _model;

            ResetToDefaultCombination();
        }

        /// <summary>
        /// Builds the combination using the pressed keys;
        /// </summary>
        /// <returns></returns>
        private Combination GetSelectedCombination()
        {
            // Build and return the combination. 
            // Note: The last key is the "trigger" key, don't include it as a "chord" key.
            var combinationBuilder = Combination.TriggeredBy(_pressedKeys.Last());
            _pressedKeys.Take(_pressedKeys.Count - 1).ForEach(k => combinationBuilder = combinationBuilder.With(k));

            return combinationBuilder;
        }

        private void ResetToDefaultCombination()
        {
            // Update the pressed keys to the default combination.
            _pressedKeys = new List<Forms.Keys>();

            _defaultCombination.Chord.ForEach(k => _pressedKeys.Add(k));
            _pressedKeys.Add(_defaultCombination.TriggerKey);

            // Reset the displayed key combination to the default combination.
            _model.KeyCombinationString = string.Join("+", _pressedKeys.Select(k => k.GetKeyDescription()));
        }

        /// <summary>
        /// When the window captures a key press event, this handler records the key that was pressed
        /// and updates the view model.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Convert key to System.Windows.Forms because that is the type 
            // that the application uses.
            var pressedKey = e.Key == Key.System ? e.SystemKey : e.Key;
            var convertedFormsKey = (Forms.Keys)KeyInterop.VirtualKeyFromKey(pressedKey);
            
            // Consolidate all variations of the same key (LShift == RShift)
            var consolidatedKey = convertedFormsKey.Consolidate();

            // Don't allow duplicate keys to be tracked.
            if (!_pressedKeys.Contains(consolidatedKey))
            {
                _pressedKeys.Add(consolidatedKey);
                _model.KeyCombinationString = string.Join("+", _pressedKeys.Select(k => k.GetKeyDescription()));
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event handler for when the save button is clicked.
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event handler for when the clear button is clicked.
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _pressedKeys = new List<Forms.Keys>();
            _model.KeyCombinationString = string.Empty;
        }

        /// <summary>
        /// Event handler for when the cancel button is clicked.
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetToDefaultCombination();
            Close();
        }
    }
}
