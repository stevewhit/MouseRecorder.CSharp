using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.Business.ExportObjects
{
    public class SerializedPlaybackRecording
    {
        public string FilePath { get; set; }
        public int Order { get; set; }
        public int TimesToRepeat { get; set; }
        public TimeSpan TimeToRun { get; set; }
        public bool IgnoreClickZones { get; set; }
        public bool StopIfFail { get; set; }
        public List<string> RecordingsToRunIfFail { get; set; }
    }
}
