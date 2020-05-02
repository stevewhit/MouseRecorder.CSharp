using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.DataModel.Configuration.Base
{
    public interface IRecordingBase
    {
        /// <summary>
        /// The date that this recording took place.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// The location that the recording is stored.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// The click-zones of this recording.
        /// </summary>
        IList<IClickZone> Zones { get; set; }
    }

    public abstract class RecordingBase : IRecordingBase
    {
        /// <summary>
        /// The date that this recording took place.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The location that the recording is stored.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The click-zones of this recording.
        /// </summary>
        public IList<IClickZone> Zones { get; set; }

        protected RecordingBase() { }
    }
}
