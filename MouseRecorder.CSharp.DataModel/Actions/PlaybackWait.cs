using System;

namespace MouseRecorder.CSharp.DataModel.Actions
{
    public interface IPlaybackWait : IAction, IPlaybackAction
    {
        /// <summary>
        /// The amount of time to wait;
        /// </summary>
        TimeSpan Time { get; set; }
    }

    public class PlaybackWait : ActionBase, IPlaybackWait
    {
        /// <summary>
        /// The amount of time to wait;
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}
