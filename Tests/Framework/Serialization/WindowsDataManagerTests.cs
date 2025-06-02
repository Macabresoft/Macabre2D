namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Runtime.Serialization;
using System.Threading;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.Macabre2D.Common;
using NUnit.Framework;

[TestFixture]
public static class WindowsDataManagerTests {
    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_DeleteTest() {
        var saveDataManager = new WindowsDataManager();
        saveDataManager.Save(SaveDataFileName, new TestSaveData());

        var found = saveDataManager.TryLoad(SaveDataFileName, out TestSaveData loadedData);

        using (new AssertionScope()) {
            found.Should().BeTrue();
            loadedData.Should().NotBeNull();
            saveDataManager.Delete(SaveDataFileName);

            found = saveDataManager.TryLoad(SaveDataFileName, out loadedData);
            found.Should().BeFalse();
            loadedData.Should().BeNull();
        }
    }

    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_LoadEmptyData_ThrowsExceptionTest() {
        var saveDataManager = new WindowsDataManager();
        saveDataManager.Delete(SaveDataFileName);

        var found = saveDataManager.TryLoad(SaveDataFileName, out TestSaveData loadedData);

        using (new AssertionScope()) {
            found.Should().BeFalse();
            loadedData.Should().BeNull();
        }
    }

    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_OverwriteSaveTest() {
        var saveDataManager = new WindowsDataManager();

        var saveData1 = new TestSaveData();
        saveDataManager.Save(SaveDataFileName, saveData1);

        saveDataManager.TryLoad(SaveDataFileName, out TestSaveData loadedData1);
        if (loadedData1 != null) {
            using (new AssertionScope()) {
                loadedData1.Id.Should().Be(saveData1.Id);
                loadedData1.RandomNumber.Should().Be(saveData1.RandomNumber);

                Thread.Sleep(100);

                var saveData2 = new TestSaveData();
                saveDataManager.Save(SaveDataFileName, saveData2);

                saveDataManager.TryLoad(SaveDataFileName, out TestSaveData loadedData2);
                loadedData2?.Id.Should().Be(saveData2.Id);
                loadedData2?.RandomNumber.Should().Be(saveData2.RandomNumber);
            }
        }
        else {
            throw new NullReferenceException(nameof(loadedData1));
        }

        saveDataManager.Delete(SaveDataFileName);
    }

    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_SuccessfulSaveAndLoadTest() {
        var saveDataManager = new WindowsDataManager();

        var saveData1 = new TestSaveData();
        saveDataManager.Save(SaveDataFileName, saveData1);

        using (new AssertionScope()) {
            saveDataManager.TryLoad(SaveDataFileName, out TestSaveData loadedData);
            loadedData?.Id.Should().Be(saveData1.Id);
            loadedData?.RandomNumber.Should().Be(saveData1.RandomNumber);

            saveDataManager.TryLoad(SaveDataFileName, out VersionedData versionedLoadedData);
            loadedData?.TypeName.Should().Be(versionedLoadedData?.TypeName);
            typeof(VersionedData).IsAssignableFrom(loadedData?.GetType()).Should().BeTrue();
        }

        saveDataManager.Delete(SaveDataFileName);
    }

    private const string SaveDataFileName = "Test.m2dsave";

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