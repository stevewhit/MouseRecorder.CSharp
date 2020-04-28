using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration
{
    public interface IPlaybackConfiguration
    {
        /// <summary>
        /// The file path location of this configuration.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// The recordings that are contained in this configuration.
        /// </summary>
        IEnumerable<IPlaybackRecording> Recordings { get; set; }
    }

    public class PlaybackConfiguration : IPlaybackConfiguration
    {
        /// <summary>
        /// The file path location of this configuration
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The recordings that are contained in this configuration.
        /// </summary>
        public IEnumerable<IPlaybackRecording> Recordings { get; set; }
    }
}
