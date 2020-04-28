using MouseRecorder.CSharp.Business.ExportObjects;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Configuration.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseRecorder.CSharp.DataModel.Test.Builders
{
    public static class FakePlaybackConfigurations
    {
        public static PlaybackConfiguration CreateFakePlaybackConfiguration()
        {
            return new PlaybackConfiguration()
            {
                FilePath = @"FakePath:\FakeDirectory\FakePlaybackConfig.txt",
                Recordings = new List<IPlaybackRecording>()
                {
                    FakeRecordings.CreateFakeLoadedPlaybackRecording()
                }
            };
        }

        public static SerializedPlaybackConfig CreateFakeSerializedPlaybackConfiguration()
        {
            return new SerializedPlaybackConfig()
            {
                FilePath = @"FakePath:\FakeDirectory\FakeSerializedConfig.txt",
                Recordings = new List<SerializedPlaybackRecording>()
                {
                    FakeRecordings.CreateFakeSerializedPlaybackRecording()
                }
            };
        }
    }
}
