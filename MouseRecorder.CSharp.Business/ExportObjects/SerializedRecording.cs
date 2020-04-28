using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Zone;
using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.Business.ExportObjects
{
    public class SerializedRecording : ISerializedObject
    {
        public DateTime Date { get; set; }
        public List<ClickZone> Zones { get; set; }
        public List<RecordedKeyboardButtonPress> KeyboardButtonPresses { get; set; }
        public List<RecordedKeyboardButtonRelease> KeyboardButtonReleases { get; set; }
        public List<RecordedMouseButtonPress> MouseButtonPresses { get; set; }
        public List<RecordedMouseButtonRelease> MouseButtonReleases { get; set; }
        public List<RecordedMouseMove> MouseMoves { get; set; }
    }
}
