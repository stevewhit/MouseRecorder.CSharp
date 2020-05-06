using MouseRecorder.CSharp.App.ViewModel;
using System.Windows;
using System.Windows.Input;
using Media = System.Windows.Media;
using Drawing = System.Drawing;

namespace MouseRecorder.CSharp.App.Views
{
    /// <summary>
    /// Interaction logic for ClickZoneView.xaml
    /// </summary>
    public partial class ClickZoneView : Window
    {
        private readonly ClickZoneViewModel _model;

        private ClickZoneView(ClickZoneViewModel model)
        {
            InitializeComponent();

            this.DataContext = model;
            _model = model;

            // Set starting location.
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = _model.StartingPosition.X;
            Top = _model.StartingPosition.Y;

            // Enable or disable changes to this window.
            if (_model.EnableChanges)
                EnableAndRemoveTransparency();
            else if (!_model.EnableChanges)
                DisableAndMakeTransparent();
        }

        /// <summary>
        /// Constructs and shows a new click-zone view using the supplied parameters.
        /// </summary>
        /// <param name="startingPosition">The coordinates of the top-left corner of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="isEnabled">Indicates if the window is enabled or disabled for changes to sizing and location.</param>
        /// <returns>Returns a newly constructed click-zone view that is now shown.</returns>
        public static ClickZoneView Show(Drawing.Point startingPosition, int width, int height, bool isEnabled)
        {
            var model = new ClickZoneViewModel()
            {
                Width = width,
                Height = height,
                StartingPosition = startingPosition,
                EnableChanges = isEnabled
            };

            // Construct the view, show it, and return it.
            var view = new ClickZoneView(model);
            view.Show();
            return view;
        }

        /// <summary>
        /// Event handler for when the window is clicked down.
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_model.EnableChanges && e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Event handler for when the window is clicked up.
        /// </summary>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Close the window if it is right-clicked.
            if (_model.EnableChanges && e.ChangedButton == MouseButton.Right)
                this.Close();
        }

        /// <summary>
        /// Enable the window to be clicked, moved, and resized.
        /// </summary>
        public void EnableAndRemoveTransparency()
        {
            _model.BackgroundColor = Media.Brushes.LightSalmon;     
            _model.ResizeMode = ResizeMode.CanResizeWithGrip;
            _model.EnableChanges = true;
        }

        /// <summary>
        /// Disable all clicking, moving, and resizing of the window. 
        /// </summary>
        public void DisableAndMakeTransparent()
        {
            _model.BackgroundColor = Media.Brushes.Transparent;
            _model.ResizeMode = ResizeMode.NoResize;
            _model.EnableChanges = false;
        }

        /// <summary>
        /// Returns the window dimensions and location as a rectangle object.
        /// </summary>
        /// <returns>Returns the window dimensions and location as a rectangle object.</returns>
        public Drawing.Rectangle GetWindowRectangle()
        {
            return new Drawing.Rectangle((int)Left, (int)Top, _model.Width, _model.Height);
        }
    }
}
