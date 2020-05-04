using Framework.Generic.IO;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.Business.ExportObjects
{
    public class SerializedRecording : ISerializedJsonObject
    {
        public string FilePath { get; set; }
        public DateTime Date { get; set; }
        public List<ClickZone> Zones { get; set; }
        public List<RecordedStart> RecordingStarts { get; set; }
        public List<RecordedStop> RecordingStops { get; set; }
        public List<RecordedKeyboardButtonPress> KeyboardButtonPresses { get; set; }
        public List<RecordedKeyboardButtonRelease> KeyboardButtonReleases { get; set; }
        public List<RecordedMouseButtonPress> MouseButtonPresses { get; set; }
        public List<RecordedMouseButtonRelease> MouseButtonReleases { get; set; }
        public List<RecordedMouseMove> MouseMoves { get; set; }
    }
}
