namespace Macabresoft.MonoGame.Tests.Core {

    using Macabresoft.MonoGame.Core;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;

    [TestFixture]
    public static class WindowsSaveDataManagerTests {
        private const string ProjectName = "Macabre2DTestProject";
        private const string SaveDataFileName = "Test.m2dsave";

        [Test]
        [Category("Integration Test")]
        public static void WindowsSaveDataManager_DeleteTest() {
            var saveDataManager = new WindowsSaveDataManager();
            ((GameSettings)GameSettings.Instance).ProjectName = ProjectName;
            saveDataManager.Save(SaveDataFileName, new TestSaveData());

            Assert.NotNull(saveDataManager.Load<TestSaveData>(SaveDataFileName));
            saveDataManager.Delete(SaveDataFileName);

            var hadException = false;
            try {
                saveDataManager.Load<TestSaveData>(SaveDataFileName);
            }
            catch (FileNotFoundException) {
                hadException = true;
            }

            Assert.True(hadException);
        }

        [Test]
        [Category("Integration Test")]
        public static void WindowsSaveDataManager_LoadEmptyData_ThrowsExceptionTest() {
            var saveDataManager = new WindowsSaveDataManager();
            ((GameSettings)GameSettings.Instance).ProjectName = ProjectName;
            saveDataManager.Delete(SaveDataFileName);

            var hadException = false;
            try {
                saveDataManager.Load<TestSaveData>(SaveDataFileName);
            }
            catch (FileNotFoundException) {
                hadException = true;
            }

            Assert.True(hadException);
        }

        [Test]
        [Category("Integration Test")]
        public static void WindowsSaveDataManager_OverwriteSaveTest() {
            var saveDataManager = new WindowsSaveDataManager();
            ((GameSettings)GameSettings.Instance).ProjectName = ProjectName;

            var saveData1 = new TestSaveData();
            saveDataManager.Save(SaveDataFileName, saveData1);

            var loadedData1 = saveDataManager.Load<TestSaveData>(SaveDataFileName);
            Assert.AreEqual(saveData1.Id, loadedData1.Id);
            Assert.AreEqual(saveData1.RandomNumber, loadedData1.RandomNumber);

            Thread.Sleep(100);

            var saveData2 = new TestSaveData();
            Assert.AreNotEqual(saveData1.Id, saveData2.Id);
            Assert.AreNotEqual(saveData1.RandomNumber, saveData2.RandomNumber);
            saveDataManager.Save(SaveDataFileName, saveData2);

            var loadedData2 = saveDataManager.Load<TestSaveData>(SaveDataFileName);
            Assert.AreNotEqual(loadedData1.Id, loadedData2.Id);
            Assert.AreNotEqual(loadedData1.RandomNumber, loadedData2.RandomNumber);

            saveDataManager.Delete(SaveDataFileName);
        }

        [Test]
        [Category("Integration Test")]
        public static void WindowsSaveDataManager_SuccessfulSaveAndLoadTest() {
            var saveDataManager = new WindowsSaveDataManager();
            ((GameSettings)GameSettings.Instance).ProjectName = ProjectName;

            var saveData1 = new TestSaveData();
            saveDataManager.Save(SaveDataFileName, saveData1);

            var loadedData = saveDataManager.Load<TestSaveData>(SaveDataFileName);
            Assert.AreEqual(saveData1.Id, loadedData.Id);
            Assert.AreEqual(saveData1.RandomNumber, loadedData.RandomNumber);

            var versionedLoadedData = saveDataManager.Load<VersionedData>(SaveDataFileName);
            Assert.AreEqual(loadedData.TypeName, versionedLoadedData.TypeName);
            Assert.AreNotEqual(loadedData.GetType(), versionedLoadedData.GetType());

            saveDataManager.Delete(SaveDataFileName);
        }

        [DataContract]
        private sealed class TestSaveData : VersionedData {

            public TestSaveData() : base() {
                var rand = new Random();
                this.RandomNumber = rand.Next(int.MinValue, int.MaxValue);
            }

            [DataMember]
            public Guid Id { get; private set; } = Guid.NewGuid();

            [DataMember]
            public int RandomNumber { get; private set; }
        }
    }
}