using Framework.Generic.IO;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.Business.ExportObjects;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace MouseRecorder.CSharp.Business.Files
{
    public interface IRecordingFile<U> where U : UnloadedRecording
    {
        /// <summary>
        /// Writes the recording to the file.
        /// </summary>
        /// <param name="recording">The recording that is written to the file.</param>
        void WriteEntity(U recording);

        /// <summary>
        /// Reads and returns the recording from the file.
        /// </summary>
        /// <returns>Returns the recording from the file.</returns>
        U ReadEntity();
    }

    public class RecordingFile<U, S> : JsonEntityFile<S>, IRecordingFile<U> where U : UnloadedRecording, new() where S : SerializedRecording, new()
    {
        public RecordingFile(string filePath) : base(filePath)
        {

        }

        public RecordingFile(string filePath, IFileSystem fileSystem) : base(filePath, fileSystem)
        {

        }

        #region IRecordingFile<U, S> Methods

        /// <summary>
        /// Writes the recording to the file.
        /// </summary>
        /// <param name="recording">The recording that is written to the file.</param>
        public void WriteEntity(U recording)
        {
            var serializedObj = Serialize(recording);
            this.WriteEntity(serializedObj);
        }

        /// <summary>
        /// Reads and returns the recording from the file.
        /// </summary>
        /// <returns>Returns the recording from the file.</returns>
        public U ReadEntity()
        {
            var serializedObj = this.GetEntity();
            return Deserialize(serializedObj);
        }

        #endregion

        /// <summary>
        /// Converts the unloaded recording to the serialized version so that it can be written as json.
        /// </summary>
        /// <param name="recording">The un-serialized version of the recording.</param>
        /// <returns>Returns the serialized version of the recording.</returns>
        private S Serialize(U recording)
        {
            return new S()
            {
                Date = recording.Date,
                Zones = recording.Zones.ToList(),
                KeyboardButtonPresses = recording.Actions.OfType<RecordedKeyboardButtonPress>().ToList(),
                KeyboardButtonReleases = recording.Actions.OfType<RecordedKeyboardButtonRelease>().ToList(),
                MouseButtonPresses = recording.Actions.OfType<RecordedMouseButtonPress>().ToList(),
                MouseButtonReleases = recording.Actions.OfType<RecordedMouseButtonRelease>().ToList(),
                MouseMoves = recording.Actions.OfType<RecordedMouseMove>().ToList()
            };
        }

        /// <summary>
        /// Converts the serialized recording to an unloaded recording.
        /// </summary>
        /// <param name="recording">The serialized version of the recording.</param>
        /// <returns>Returns the unserialized version of the recording.</returns>
        private U Deserialize(S recording)
        {
            var actions = new List<IRecordedAction>();

            actions.AddRange(recording.KeyboardButtonPresses);
            actions.AddRange(recording.KeyboardButtonReleases);
            actions.AddRange(recording.MouseButtonPresses);
            actions.AddRange(recording.MouseButtonReleases);
            actions.AddRange(recording.MouseMoves);

            return new U()
            {
                Date = recording.Date,
                Zones = recording.Zones,
                Actions = actions.OrderBy(a => a.Date).ToList()
            };
        }
    }
}
