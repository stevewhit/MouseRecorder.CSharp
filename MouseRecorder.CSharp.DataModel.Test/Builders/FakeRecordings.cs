using Framework.Generic.Utility;
using MouseRecorder.CSharp.Business.ExportObjects;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Zone;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Test.Builders
{
    public static class FakeRecordings
    {
        public static Recording CreateFakeRecording()
        {
            var recording = new Recording
            {
                Actions = new List<IRecordedAction>(),
                FilePath = @"FakePath:\FakeDirectory\FakeRecording.txt",
                Date = SystemTime.Now(),
                Zones = new List<ClickZone>
                {
                    new ClickZone() { Shape = new Rectangle(100, 200, 50, 51) }
                }
            };

            recording.Actions.Add(new RecordedMouseButtonPress() { Id = 1, PixelColor = Color.Blue, Button = MouseButtons.Left, Date = SystemTime.Now().AddSeconds(-5) });
            recording.Actions.Add(new RecordedMouseButtonRelease() { Id = 2, PixelColor = Color.Blue, Button = MouseButtons.Left, Date = SystemTime.Now().AddSeconds(-4) });
            recording.Actions.Add(new RecordedKeyboardButtonPress() { Id = 3, Key = Keys.A, Date = SystemTime.Now().AddSeconds(-3) });
            recording.Actions.Add(new RecordedKeyboardButtonRelease() { Id = 4, Key = Keys.A, Date = SystemTime.Now().AddSeconds(-2) });
            recording.Actions.Add(new RecordedMouseMove() { Id = 5, ScreenCoordinate = new Point(100, 125), PixelColor = Color.Brown, Button = MouseButtons.None, Date = SystemTime.Now().AddSeconds(-1) });

            return recording;
        }

        public static LoadedPlaybackRecording CreateFakeLoadedPlaybackRecording()
        {
            var recording = new LoadedPlaybackRecording
            {
                Actions = new List<IPlaybackAction>(),
                FilePath = @"FakePath:\FakeDirectory\FakeRecording.txt",
                Date = SystemTime.Now(),
                Zones = new List<ClickZone>
                {
                    new ClickZone() { Shape = new Rectangle(100, 200, 50, 51) }
                },
                 IgnoreClickZones = false,
                 Order = 1,
                 RecordingsToRunIfFail = new List<IRecording> 
                 { 
                     CreateFakeRecording()
                 },
                 StopIfFail = false,
                 TimesToRepeat = 5, 
                 TimeToRun = new System.TimeSpan(0, 5, 6)
            };

            recording.Actions.Add(new PlaybackMouseButtonPress() { Id = 1, ExpectedPixelColor = Color.Blue, Button = MouseButtons.Left });
            recording.Actions.Add(new PlaybackMouseButtonRelease() { Id = 2, ExpectedPixelColor = Color.Blue, Button = MouseButtons.Left });
            recording.Actions.Add(new PlaybackKeyboardButtonPress() { Id = 3, Key = Keys.A });
            recording.Actions.Add(new PlaybackKeyboardButtonRelease() { Id = 4, Key = Keys.A });
            recording.Actions.Add(new PlaybackMouseMove() { Id = 5, ScreenCoordinate = new Point(100, 125), ExpectedPixelColor = Color.Brown, Button = MouseButtons.None});
            recording.Actions.Add(new PlaybackWait() { Id = 6, Time = new System.TimeSpan(5000)});
            recording.Actions.Add(new PlaybackMouseMove() { Id = 6, ScreenCoordinate = new Point(100, 125), ExpectedPixelColor = Color.Brown, Button = MouseButtons.None });

            return recording;
        }

        public static SerializedRecording CreateFakeSerializedRecording()
        {
            var recording = CreateFakeRecording();

            return new SerializedRecording()
            {
                FilePath = recording.FilePath,
                Date = recording.Date,
                Zones = recording.Zones.ToList(),
                KeyboardButtonPresses = recording.Actions.OfType<RecordedKeyboardButtonPress>().ToList(),
                KeyboardButtonReleases = recording.Actions.OfType<RecordedKeyboardButtonRelease>().ToList(),
                MouseButtonPresses = recording.Actions.OfType<RecordedMouseButtonPress>().ToList(),
                MouseButtonReleases = recording.Actions.OfType<RecordedMouseButtonRelease>().ToList(),
                MouseMoves = recording.Actions.OfType<RecordedMouseMove>().ToList()
            };
        }

        public static SerializedPlaybackRecording CreateFakeSerializedPlaybackRecording()
        {
            var recording = CreateFakeRecording();

            return new SerializedPlaybackRecording()
            {
                FilePath = @"FakePath:\FakeDirectory\FakeRecording.txt", 
                RecordingsToRunIfFail = new List<string>() { @"FakePath:\FakeDirectory\FakeRecording.txt"}
            };
        }
    }
}
