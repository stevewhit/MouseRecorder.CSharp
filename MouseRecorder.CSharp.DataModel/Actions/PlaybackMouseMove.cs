using System.Drawing;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IPlaybackMouseMove : IPlaybackMouseAction
    {
        /// <summary>
        /// The (X,Y) screen coordinate of this mouse move.
        /// </summary>
        Point ScreenCoordinate { get; set; }
    }

    public class PlaybackMouseMove : PlaybackMouseActionBase, IPlaybackMouseMove
    {
        /// <summary>
        /// The (X,Y) screen coordinate of this mouse move.
        /// </summary>
        public Point ScreenCoordinate { get; set; }
    }
}
