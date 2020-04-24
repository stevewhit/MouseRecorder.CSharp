using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface IPlaybackConfiguration
    {
        /// <summary>
        /// The recordings that are contained in this configuration.
        /// </summary>
        IEnumerable<IRecording> Recordings { get; set; }
    }

    public class PlaybackConfiguration
    {
        /// <summary>
        /// The recordings that are contained in this configuration.
        /// </summary>
        public IEnumerable<IRecording> Recordings { get; set; }
    }
}
