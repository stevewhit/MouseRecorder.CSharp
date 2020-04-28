using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Framework.Generic.Tests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MouseRecorder.CSharp.Business.ExportObjects;
using MouseRecorder.CSharp.Business.Files;
using MouseRecorder.CSharp.DataModel.Actions;
using MouseRecorder.CSharp.DataModel.Configuration;
using MouseRecorder.CSharp.DataModel.Test.Builders;
using Newtonsoft.Json;

namespace MouseRecorder.CSharp.Business.Test.Files
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RecordingFileTest
    {
        private const string _fakeFilePath = @"FakePath:\FakeDirectory\JsonEntityFileTests.txt";
        private MockFileSystem _mockFileSystem;
        private RecordingFile<UnloadedRecording, SerializedRecording> _file;

        [TestInitialize]
        public void Initialize()
        {
            _mockFileSystem = new MockFileSystem();
            _file = new RecordingFile<UnloadedRecording, SerializedRecording>(_fakeFilePath, _mockFileSystem.Object);
        }

        #region Testing RecordingFile(string filePath)...

        [TestMethod]
        public void RecordingFileString_WithFilePath_IsNotNull()
        {
            // Act
            _file = new RecordingFile<UnloadedRecording, SerializedRecording>(_fakeFilePath);

            // Assert
            Assert.IsNotNull(_file);
        }

        #endregion
        #region Testing RecordingFile(string filePath, IFileSystem fileSystem)...

        [TestMethod]
        public void RecordingFileStringAndFileSystem_WithValidArgs_IsNotNull()
        {
            // Act
            _file = new RecordingFile<UnloadedRecording, SerializedRecording>(_fakeFilePath, _mockFileSystem.Object);

            // Assert
            Assert.IsNotNull(_file);
        }

        #endregion
        #region Testing WriteEntity(U recording)...

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void WriteEntity_WithNullEntity_ThrowsException()
        {
            // Act
            _file.WriteEntity(null);
        }

        [TestMethod]
        public void WriteEntity_WithValidRecording_StoresEntity()
        {
            // Arrange
            var entityToWrite = FakeRecordings.CreateFakeUnloadedRecording();
            var serializedEntity = new SerializedRecording()
            {
                Date = entityToWrite.Date,
                Zones = entityToWrite.Zones.ToList(),
                KeyboardButtonPresses = entityToWrite.Actions.OfType<RecordedKeyboardButtonPress>().ToList(),
                KeyboardButtonReleases = entityToWrite.Actions.OfType<RecordedKeyboardButtonRelease>().ToList(),
                MouseButtonPresses = entityToWrite.Actions.OfType<RecordedMouseButtonPress>().ToList(),
                MouseButtonReleases = entityToWrite.Actions.OfType<RecordedMouseButtonRelease>().ToList(),
                MouseMoves = entityToWrite.Actions.OfType<RecordedMouseMove>().ToList()
            };

            var entityToWriteJson = JsonConvert.SerializeObject(serializedEntity);

            // Act
            _file.WriteEntity(entityToWrite);

            var storedFileJson = _mockFileSystem.StoredFiles[_fakeFilePath];

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(storedFileJson));
            Assert.IsTrue(entityToWriteJson.Equals(storedFileJson));
        }

        #endregion
        #region Testing ReadEntity()...

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadEntity_WithNonExistingFilePath_ThrowsException()
        {
            // Act
            _file.ReadEntity();
        }

        [TestMethod]
        public void ReadEntity_WithExistingRecording_ReturnsEntity()
        {
            // Arrange
            var entityToWrite = FakeRecordings.CreateFakeUnloadedRecording();
            _file.WriteEntity(entityToWrite);

            var writtenEntityJson = JsonConvert.SerializeObject(entityToWrite);

            // Act
            var readEntity = _file.ReadEntity();
            var readEntityJson = JsonConvert.SerializeObject(readEntity);

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(readEntityJson));
            Assert.IsTrue(writtenEntityJson.Equals(readEntityJson));
        }

        #endregion
    }
}
