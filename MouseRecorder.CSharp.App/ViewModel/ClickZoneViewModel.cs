using System.Windows;
using Media = System.Windows.Media;
using Drawing = System.Drawing;

namespace MouseRecorder.CSharp.App.ViewModel
{
    public class ClickZoneViewModel : PageViewModelBase
    {
		private Media.Brush _backgroundColor;
		public Media.Brush BackgroundColor
		{
            get => _backgroundColor;
            set => Set(ref _backgroundColor, value);
        }

        private bool _enableChanges;
        public bool EnableChanges
        {
            get => _enableChanges;
            set => Set(ref _enableChanges, value);
        }

        private ResizeMode _resizeMode;
        public ResizeMode ResizeMode
        {
            get => _resizeMode;
            set => Set(ref _resizeMode, value);
        }

        private int _height;
        public int Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        private int _width;
        public int Width
        {
            get => _width;
            set => Set(ref _width, value);
        }

        public Drawing.Point StartingPosition { get; set; }
    }
}
