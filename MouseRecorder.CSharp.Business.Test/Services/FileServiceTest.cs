using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Framework.Generic.Tests.Builders;
using Framework.Generic.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MouseRecorder.CSharp.Business.Services;
using MouseRecorder.CSharp.DataModel.Test.Builders;
using Newtonsoft.Json;

namespace MouseRecorder.CSharp.Business.Test.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FileServiceTest
    {
        private MockFileSystem _mockFileSystem;
        private IFileService _service;

        [TestInitialize]
        public void Initialize()
        {
            _mockFileSystem = new MockFileSystem();
            _service = new FileService(_mockFileSystem.Object);
        }

        #region Testing FileService()...

        [TestMethod]
        public void FileService_WithNoParams_IsNotNull()
        {
            // Act
            _service = new FileService();

            // Assert
            Assert.IsNotNull(_service);
        }

        #endregion
        #region Testing FileService(IFileSystem fileSystem)...

        [TestMethod]
        public void FileService_WithValidFileSystem_IsNotNull()
        {
            // Act
            _service = new FileService(_mockFileSystem.Object);

            // Assert
            Assert.IsNotNull(_service);
        }

        #endregion
        #region Testing GetPlaybackConfiguration(string filePath)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPlaybackConfiguration_WithNullFilePath_ThrowsException()
        {
            // Act
            _service.GetPlaybackConfiguration(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPlaybackConfiguration_WithEmptyFilePath_ThrowsException()
        {
            // Act
            _service.GetPlaybackConfiguration(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetPlaybackConfiguration_WithNonExistingFilePath_ThrowsException()
        {
            // Act
            _service.GetPlaybackConfiguration(@"Fakepath:/File/not/found.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetPlaybackConfiguration_WithInvalidFileContents_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeRecording();
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetPlaybackConfiguration(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetPlaybackConfiguration_WithMissingFilePath_ThrowsException()
        {
            // Arrange
            var fakeConfiguration = FakePlaybackConfigurations.CreateFakeSerializedPlaybackConfiguration();
            var validFilePath = fakeConfiguration.FilePath;
            fakeConfiguration.FilePath = null;

            var fakeConfigJson = JsonConvert.SerializeObject(fakeConfiguration);

            _mockFileSystem.StoredFiles.Add(validFilePath, fakeConfigJson);

            // Act
            _service.GetPlaybackConfiguration(validFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetPlaybackConfiguration_WithMissingRecordingsToRunIfFail_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeRecording();
            _service.SaveRecording(fakeRecording);

            var fakeConfiguration = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            fakeConfiguration.Recordings.First().RecordingsToRunIfFail = null;
            var fakeConfigJson = JsonConvert.SerializeObject(fakeConfiguration);

            _mockFileSystem.StoredFiles.Add(fakeConfiguration.FilePath, fakeConfigJson);

            // Act
            _service.GetPlaybackConfiguration(fakeConfiguration.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetPlaybackConfiguration_WithMissingRecordings_ThrowsException()
        {
            // Arrange
            var fakeConfiguration = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            fakeConfiguration.Recordings = null;

            var fakeConfigJson = JsonConvert.SerializeObject(fakeConfiguration);

            _mockFileSystem.StoredFiles.Add(fakeConfiguration.FilePath, fakeConfigJson);

            // Act
            _service.GetPlaybackConfiguration(fakeConfiguration.FilePath);
        }

        [TestMethod]
        public void GetPlaybackConfiguration_WithValidConfig_ReturnsPlaybackConfiguration()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeRecording();
            _service.SaveRecording(fakeRecording);

            var fakeConfiguration = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            _service.SavePlaybackConfiguration(fakeConfiguration);

            // Act
            var returnedConfig = _service.GetPlaybackConfiguration(fakeConfiguration.FilePath);

            // Assert
            Assert.IsNotNull(returnedConfig);
            Assert.IsTrue(returnedConfig.FilePath.Equals(fakeConfiguration.FilePath));
            Assert.IsTrue(returnedConfig.Recordings.Count() == fakeConfiguration.Recordings.Count());
        }

        #endregion
        #region Testing GetRecording(string filePath)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetRecording_WithNullFilePath_ThrowsException()
        {
            // Act
            _service.GetRecording(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetRecording_WithEmptyFilePath_ThrowsException()
        {
            // Act
            _service.GetRecording(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetRecording_WithNonExistingFilePath_ThrowsException()
        {
            // Act
            _service.GetRecording(@"Fakepath:/File/not/found.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithInvalidFileContents_ThrowsException()
        {
            // Arrange
            var fakeConfiguration = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            var fakeConfigurationJson = JsonConvert.SerializeObject(fakeConfiguration);

            _mockFileSystem.StoredFiles.Add(fakeConfiguration.FilePath, fakeConfigurationJson);

            // Act
            _service.GetRecording(fakeConfiguration.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullKeyboardButtonPresses_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.KeyboardButtonPresses = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullKeyboardButtonReleases_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.KeyboardButtonReleases = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullMouseButtonPresses_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.MouseButtonPresses = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullMouseButtonReleases_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.MouseButtonReleases = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullMouseMoves_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.MouseMoves = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullInternalFilePath_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            var filePathBefore = fakeRecording.FilePath;
            fakeRecording.FilePath = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(filePathBefore, fakeRecordingJson);

            // Act
            _service.GetRecording(filePathBefore);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void GetRecording_WithNullZones_ThrowsException()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            fakeRecording.Zones = null;
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            _service.GetRecording(fakeRecording.FilePath);
        }

        [TestMethod]
        public void GetRecording_WithValidRecording_ReturnsRecording()
        {
            // Arrange
            var fakeRecording = FakeRecordings.CreateFakeSerializedRecording();
            var fakeRecordingJson = JsonConvert.SerializeObject(fakeRecording);

            _mockFileSystem.StoredFiles.Add(fakeRecording.FilePath, fakeRecordingJson);

            // Act
            var returnedRecording = _service.GetRecording(fakeRecording.FilePath);

            // Assert
            Assert.IsNotNull(returnedRecording);
            Assert.IsTrue(returnedRecording.FilePath.Equals(fakeRecording.FilePath));
            Assert.IsTrue(returnedRecording.Zones.Count() == fakeRecording.Zones.Count());
        }

        #endregion
        #region Testing SavePlaybackConfiguration(IPlaybackConfiguration playbackConfig)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SavePlaybackConfiguration_WithNullPlaybackConfig_ThrowsException()
        {
            // Act
            _service.SavePlaybackConfiguration(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SavePlaybackConfiguration_WithNullFilePath_ThrowsException()
        {
            // Arrange
            var config = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            config.FilePath = null;

            // Act
            _service.SavePlaybackConfiguration(config);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SavePlaybackConfiguration_WithNullRecordings_ThrowsException()
        {
            // Arrange
            var config = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            config.Recordings = null;

            // Act
            _service.SavePlaybackConfiguration(config);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SavePlaybackConfiguration_WithNullPlaybackRecordingFilePath_ThrowsException()
        {
            // Arrange
            var config = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            config.Recordings.First().FilePath = null;

            // Act
            _service.SavePlaybackConfiguration(config);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SavePlaybackConfiguration_WithNullPlaybackRecordingRecordingsToRunIfFail_ThrowsException()
        {
            // Arrange
            var config = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            config.Recordings.First().RecordingsToRunIfFail = null;

            // Act
            _service.SavePlaybackConfiguration(config);
        }

        [TestMethod]
        public void SavePlaybackConfiguration_WithExistingFile_OverwritesFile()
        {
            // Arrange
            var initialConfig = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();
            initialConfig.Recordings.First().Order = 111;
            _service.SavePlaybackConfiguration(initialConfig);

            var playbackRecording = FakeRecordings.CreateFakeSerializedRecording();
            var playbackRecordingJson = JsonConvert.SerializeObject(playbackRecording);
            _mockFileSystem.StoredFiles.Add(playbackRecording.FilePath, playbackRecordingJson);

            var updatedConfig = _service.GetPlaybackConfiguration(initialConfig.FilePath);
            updatedConfig.Recordings.First().Order = 999;

            // Act
            _service.SavePlaybackConfiguration(updatedConfig);

            // Assert
            Assert.IsTrue(updatedConfig.Recordings.First().Order == 999);
        }

        [TestMethod]
        public void SavePlaybackConfiguration_WithoutExistingFile_SavesFile()
        {
            // Arrange
            var config = FakePlaybackConfigurations.CreateFakePlaybackConfiguration();

            // Act
            _service.SavePlaybackConfiguration(config);

            var fileData = _mockFileSystem.StoredFiles[config.FilePath];

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(fileData));
        }

        #endregion
        #region Testing SaveRecording(IRecording recording)...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveRecording_WithNullRecording_ThrowsException()
        {
            // Act
            _service.SaveRecording(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SaveRecording_WithNullActions_ThrowsException()
        {
            // Arrange 
            var recording = FakeRecordings.CreateFakeRecording();
            recording.Actions = null;

            // Act
            _service.SaveRecording(recording);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SaveRecording_WithNullFilePath_ThrowsException()
        {
            // Arrange 
            var recording = FakeRecordings.CreateFakeRecording();
            recording.FilePath = null;

            // Act
            _service.SaveRecording(recording);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void SaveRecording_WithNullZones_ThrowsException()
        {
            // Arrange 
            var recording = FakeRecordings.CreateFakeRecording();
            recording.Zones = null;

            // Act
            _service.SaveRecording(recording);
        }

        [TestMethod]
        public void SaveRecording_WithExistingFile_OverwritesFile()
        {
            // Arrange
            var initialRecording = FakeRecordings.CreateFakeSerializedRecording();
            initialRecording.Date = SystemTime.Now().AddDays(-1);
            var initialRecordingJson = JsonConvert.SerializeObject(initialRecording);

            _mockFileSystem.StoredFiles.Add(initialRecording.FilePath, initialRecordingJson);

            var updatedRecording = _service.GetRecording(initialRecording.FilePath);
            updatedRecording.Date = new DateTime(111555666);

            // Act
            _service.SaveRecording(updatedRecording);

            // Assert
            Assert.IsTrue(updatedRecording.Date.Ticks == 111555666);
        }

        [TestMethod]
        public void SaveRecording_WithoutExistingFile_SavesFile()
        {
            // Arrange
            var recording = FakeRecordings.CreateFakeRecording();

            // Act
            _service.SaveRecording(recording);

            var fileData = _mockFileSystem.StoredFiles[recording.FilePath];

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(fileData));
        }

        #endregion
    }
}
