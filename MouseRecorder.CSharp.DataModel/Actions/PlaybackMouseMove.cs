using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
