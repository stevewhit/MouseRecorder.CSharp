

using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IPlaybackMouseAction : IMouseAction, IPlaybackAction
    {
        /// <summary>
        /// The expected color of the pixel at the time that this action is played back.
        /// </summary>
        Color ExpectedPixelColor { get; set; }
    }

    public abstract class PlaybackMouseActionBase : MouseActionBase, IPlaybackMouseAction
    {
        /// <summary>
        /// The expected color of the pixel at the time that this action is played back.
        /// </summary>
        public Color ExpectedPixelColor { get; set; }
    }
}
