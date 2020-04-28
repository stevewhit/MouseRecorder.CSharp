using Framework.Generic.IO;
using System;
using System.Collections.Generic;

namespace MouseRecorder.CSharp.Business.ExportObjects
{
    public class SerializedPlaybackConfig : ISerializedJsonObject
    {
        public string FilePath { get; set; }
        public List<SerializedPlaybackRecording> Recordings { get; set; }
    }
}
