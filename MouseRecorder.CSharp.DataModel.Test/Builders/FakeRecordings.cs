using Framework.Generic.Utility;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Zone;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MouseRecorder.CSharp.DataModel.Test.Builders
{
    public static class FakeRecordings
    {
        public static UnloadedRecording CreateFakeUnloadedRecording()
        {
            var recording = new UnloadedRecording
            {
                Actions = new List<IRecordedAction>()
            };

            recording.Actions.Add(new RecordedMouseButtonPress() { Id = 1, PixelColor = Color.Blue, Button = MouseButtons.Left, Date = SystemTime.Now().AddSeconds(-5) });
            recording.Actions.Add(new RecordedMouseButtonRelease() { Id = 2, PixelColor = Color.Blue, Button = MouseButtons.Left, Date = SystemTime.Now().AddSeconds(-4) });
            recording.Actions.Add(new RecordedKeyboardButtonPress() { Id = 3, Key = Keys.A, Date = SystemTime.Now().AddSeconds(-3) });
            recording.Actions.Add(new RecordedKeyboardButtonRelease() { Id = 4, Key = Keys.A, Date = SystemTime.Now().AddSeconds(-2) });
            recording.Actions.Add(new RecordedMouseMove() { Id = 5, ScreenCoordinate = new Point(100, 125), PixelColor = Color.Brown, Button = MouseButtons.None, Date = SystemTime.Now().AddSeconds(-1) });

            recording.Date = SystemTime.Now();

            recording.Zones = new List<ClickZone>
            {
                new ClickZone() { Shape = new Rectangle(100, 200, 50, 51) }
            };

            return recording;
        }
    }
}
