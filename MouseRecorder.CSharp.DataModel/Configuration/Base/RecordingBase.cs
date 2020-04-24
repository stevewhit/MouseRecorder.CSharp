using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Configuration.Base
{
    public interface IRecording
    {
        /// <summary>
        /// The date that this recording took place.
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        /// The click-zones of this recording.
        /// </summary>
        IEnumerable<ClickZone> Zones { get; set; }
    }

    public abstract class RecordingBase : IRecording
    {
        /// <summary>
        /// The date that this recording took place.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The click-zones of this recording.
        /// </summary>
        public IEnumerable<ClickZone> Zones { get; set; }
    }
}
