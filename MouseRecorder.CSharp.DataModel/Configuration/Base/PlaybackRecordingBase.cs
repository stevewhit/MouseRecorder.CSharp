using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration.Base
{
    public interface IPlaybackRecording : IRecordingBase
    {
        /// <summary>
        /// The order in which this playback recording is run.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// The number of times to repeat the playback of this recording.
        /// </summary>
        int TimesToRepeat { get; set; }

        /// <summary>
        /// The length of time to repeat the playback of this recording.
        /// </summary>
        TimeSpan TimeToRun { get; set; }

        /// <summary>
        /// Configuration for if the playback should ignore the click-zones of this recording.
        /// </summary>
        bool IgnoreClickZones { get; set; }

        /// <summary>
        /// Configuration for if the playback should stop running other playback recordings if this one fails.
        /// </summary>
        bool StopIfFail { get; set; }

        /// <summary>
        /// The recordings that should be run if playback of this recording fails.
        /// </summary>
        List<IRecording> RecordingsToRunIfFail { get; set; }
    }

    public abstract class PlaybackRecordingBase : RecordingBase, IPlaybackRecording
    {
        /// <summary>
        /// The order in which this playback recording is run.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The number of times to repeat the playback of this recording.
        /// </summary>
        public int TimesToRepeat { get; set; }

        /// <summary>
        /// The length of time to repeat the playback of this recording.
        /// </summary>
        public TimeSpan TimeToRun { get; set; }

        /// <summary>
        /// Configuration for if the playback should ignore the click-zones of this recording.
        /// </summary>
        public bool IgnoreClickZones { get; set; }

        /// <summary>
        /// Configuration for if the playback should stop running other playback recordings if this one fails.
        /// </summary>
        public bool StopIfFail { get; set; }

        /// <summary>
        /// The recordings that should be run if playback of this recording fails.
        /// </summary>
        public List<IRecording> RecordingsToRunIfFail{ get; set; }
    }
}
