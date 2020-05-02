using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Abstractions;
using Framework.Generic.IO;
using MouseRecorder.CSharp.Business.ExportObjects;
using System;
using MouseRecorder.CSharp.DataModel.Zone;

namespace MouseRecorder.CSharp.Business.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Returns the playback configuration from the designated <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path location of where the playback configuration is stored.</param>
        /// <returns>Returns the playback configuration from the designated <paramref name="filePath"/>.</returns>
        IPlaybackConfiguration GetPlaybackConfiguration(string filePath);

        /// <summary>
        /// Returns the recording from the designated <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path location of where the recording is stored.</param>
        /// <returns>Returns the recording from the designated <paramref name="filePath"/>.</returns>
        IRecording GetRecording(string filePath);

        /// <summary>
        /// Saves the <paramref name="playbackConfig"/> to the file.
        /// </summary>
        /// <param name="playbackConfig">The playback configuration that should be saved.</param>
        void SavePlaybackConfiguration(IPlaybackConfiguration playbackConfig);

        /// <summary>
        /// Saves the <paramref name="recording"/> to the file.
        /// </summary>
        /// <param name="recording">The recording that should be saved.</param>
        void SaveRecording(IRecording recording);
    }

    public class FileService : IFileService
    {
        /// <summary>
        /// Abstracted System.IO.File for better testability.
        /// </summary>
        private IFileSystem _fileSystem;

        public FileService() : this(new FileSystem()) { }
        public FileService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        #region IFileService Methods

        /// <summary>
        /// Returns the playback configuration from the designated <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path location of where the playback configuration is stored.</param>
        /// <returns>Returns the playback configuration from the designated <paramref name="filePath"/>.</returns>
        public IPlaybackConfiguration GetPlaybackConfiguration(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!_fileSystem.File.Exists(filePath))
                throw new FileNotFoundException($"Invalid filePath. Could not find a playback configuration at '{filePath}'.");

            // Read the json from the file and convert it to the serialized version.
            var serializedPlaybackConfig = new JsonEntityFile<SerializedPlaybackConfig>(filePath, _fileSystem).GetEntity();

            return Deserialize(serializedPlaybackConfig);
        }

        /// <summary>
        /// Returns the recording from the designated <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path location of where the recording is stored.</param>
        /// <returns>Returns the recording from the designated <paramref name="filePath"/>.</returns>
        public IRecording GetRecording(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!_fileSystem.File.Exists(filePath))
                throw new FileNotFoundException($"Invalid filePath. Could not find a recording at '{filePath}'.");

            // Read the json from the file and convert it to the serialized version.
            var serializedRecording = new JsonEntityFile<SerializedRecording>(filePath, _fileSystem).GetEntity();

            return Deserialize(serializedRecording);
        }

        /// <summary>
        /// Saves the <paramref name="playbackConfig"/> to the file.
        /// </summary>
        /// <param name="playbackConfig">The playback configuration that should be saved.</param>
        public void SavePlaybackConfiguration(IPlaybackConfiguration playbackConfig)
        {
            if (playbackConfig == null)
                throw new ArgumentNullException(nameof(playbackConfig));

            var serializedPlaybackConfig = Serialize(playbackConfig);

            // Write the serialized playback config to the specified file.
            var file = new JsonEntityFile<SerializedPlaybackConfig>(playbackConfig.FilePath, _fileSystem);
            file.WriteEntity(serializedPlaybackConfig);
        }

        /// <summary>
        /// Saves the <paramref name="recording"/> to the file.
        /// </summary>
        /// <param name="recording">The recording that should be saved.</param>
        public void SaveRecording(IRecording recording)
        {
            if (recording == null)
                throw new ArgumentNullException(nameof(recording));

            var serializedRecording = Serialize(recording);
            
            // Write the serialized recording to the specified file.
            var file = new JsonEntityFile<SerializedRecording>(recording.FilePath, _fileSystem);
            file.WriteEntity(serializedRecording);
        }

        #endregion

        /// <summary>
        /// Converts the <paramref name="recording"/> to the serialized version.
        /// </summary>
        /// <param name="recording">The un-serialized version of the recording.</param>
        /// <returns>Returns the serialized version of the <paramref name="recording"/>.</returns>
        private SerializedRecording Serialize(IRecording recording)
        {
            if (recording.Actions == null)
                throw new InvalidDataException("Unable to serialize the object because it is invalid.");

            return new SerializedRecording()
            {
                FilePath = recording.FilePath ?? throw new InvalidDataException("Unable to serialize the object because it is invalid."),
                Date = recording.Date,
                Zones = recording.Zones.OfType<ClickZone>()?.ToList() ?? throw new InvalidDataException("Unable to serialize the object because it is invalid."),
                KeyboardButtonPresses = recording.Actions.OfType<RecordedKeyboardButtonPress>().ToList(),
                KeyboardButtonReleases = recording.Actions.OfType<RecordedKeyboardButtonRelease>().ToList(),
                MouseButtonPresses = recording.Actions.OfType<RecordedMouseButtonPress>().ToList(),
                MouseButtonReleases = recording.Actions.OfType<RecordedMouseButtonRelease>().ToList(),
                MouseMoves = recording.Actions.OfType<RecordedMouseMove>().ToList() 
            };
        }

        /// <summary>
        /// Converts the <paramref name="playbackConfig"/> to the serialized version.
        /// </summary>
        /// <param name="playbackConfig">The un-serialized version of the playback configuration.</param>
        /// <returns>Returns the serialized version of the <paramref name="playbackConfig"/>.</returns>
        private SerializedPlaybackConfig Serialize(IPlaybackConfiguration playbackConfig)
        {
            return new SerializedPlaybackConfig()
            {
                FilePath = playbackConfig.FilePath ?? throw new InvalidDataException("Unable to serialize the object because it is invalid."),
                Recordings = playbackConfig.Recordings?.Select((r) =>
                {
                    return new SerializedPlaybackRecording()
                    {
                        FilePath = r.FilePath ?? throw new InvalidDataException("Unable to serialize the object because it is invalid."),
                        Order = r.Order,
                        TimesToRepeat = r.TimesToRepeat,
                        TimeToRun = r.TimeToRun,
                        IgnoreClickZones = r.IgnoreClickZones,
                        StopIfFail = r.StopIfFail,
                        RecordingsToRunIfFail = r.RecordingsToRunIfFail?.Select(f => f.FilePath).ToList() ?? throw new InvalidDataException("Unable to serialize the object because it is invalid.")
                    };
                }).ToList() ?? throw new InvalidDataException("Unable to serialize the object because it is invalid.")
            };
        }

        /// <summary>
        /// Converts the serialized <paramref name="recording"/> to the un-serialized version.
        /// </summary>
        /// <param name="recording">The serialized version of the recording.</param>
        /// <returns>Returns the un-serialized version of the <paramref name="recording"/>.</returns>
        private IRecording Deserialize(SerializedRecording recording)
        {
            var actions = new List<IRecordedAction>();
            actions.AddRange(recording.KeyboardButtonPresses ?? throw new FileLoadException("There was an issue deserializing the object."));
            actions.AddRange(recording.KeyboardButtonReleases ?? throw new FileLoadException("There was an issue deserializing the object."));
            actions.AddRange(recording.MouseButtonPresses ?? throw new FileLoadException("There was an issue deserializing the object."));
            actions.AddRange(recording.MouseButtonReleases ?? throw new FileLoadException("There was an issue deserializing the object."));
            actions.AddRange(recording.MouseMoves ?? throw new FileLoadException("There was an issue deserializing the object."));

            var zones = new List<IClickZone>();
            zones.AddRange(recording.Zones ?? throw new FileLoadException("There was an issue deserializing the object."));

            return new Recording()
            {
                FilePath = recording.FilePath ?? throw new FileLoadException("There was an issue deserializing the object."),
                Date = recording.Date,
                Zones = zones,
                Actions = actions.OrderBy(a => a.TimeRecorded).ToList()
            };
        }

        /// <summary>
        /// Converts the serialized <paramref name="playbackConfig"/> to the un-serialized version.
        /// </summary>
        /// <param name="playbackConfig">The serialized version of the playback configuration.</param>
        /// <returns>Returns the un-serialized version of the <paramref name="playbackConfig"/>.</returns>
        private IPlaybackConfiguration Deserialize(SerializedPlaybackConfig playbackConfig)
        {
            return new PlaybackConfiguration()
            {
                FilePath = playbackConfig.FilePath ?? throw new FileLoadException("There was an issue deserializing the object."),
                Recordings = playbackConfig.Recordings?.Select((spr) =>
                {
                    var recording = GetRecording(spr.FilePath);

                    return new UnloadedPlaybackRecording()
                    {
                        // Serialized properties
                        FilePath = recording.FilePath,
                        Order = spr.Order,
                        TimesToRepeat = spr.TimesToRepeat,
                        TimeToRun = spr.TimeToRun,
                        IgnoreClickZones = spr.IgnoreClickZones,
                        StopIfFail = spr.StopIfFail,
                        RecordingsToRunIfFail = spr.RecordingsToRunIfFail?.Select(filePath => GetRecording(filePath)).ToList() ?? throw new FileLoadException("There was an issue deserializing the object."),

                        // Additional properties
                        Date = recording.Date,
                        Actions = recording.Actions,
                        Zones = recording.Zones,
                    };
                }).ToList() ?? throw new FileLoadException("There was an issue deserializing the object.")
            };
        }
    }
}
