namespace Macabresoft.Macabre2D.Tests.Framework;

using System;
using System.Runtime.Serialization;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using NUnit.Framework;

[TestFixture]
public static class WindowsDataManagerTests {
    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_DeleteTest() {
        var saveDataManager = new WindowsDataManager();
        saveDataManager.Save(SaveDataFileName, ProjectName, new TestSaveData());

        var found = saveDataManager.TryLoad(SaveDataFileName, ProjectName, out TestSaveData loadedData);

        using (new AssertionScope()) {
            found.Should().BeTrue();
            loadedData.Should().NotBeNull();
            saveDataManager.Delete(SaveDataFileName, ProjectName);

            found = saveDataManager.TryLoad(SaveDataFileName, ProjectName, out loadedData);
            found.Should().BeFalse();
            loadedData.Should().BeNull();
        }
    }

    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_LoadEmptyData_ThrowsExceptionTest() {
        var saveDataManager = new WindowsDataManager();
        saveDataManager.Delete(SaveDataFileName, ProjectName);

        var found = saveDataManager.TryLoad(SaveDataFileName, ProjectName, out TestSaveData loadedData);

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
        saveDataManager.Save(SaveDataFileName, ProjectName, saveData1);

        saveDataManager.TryLoad(SaveDataFileName, ProjectName, out TestSaveData loadedData1);
        if (loadedData1 != null) {
            using (new AssertionScope()) {
                loadedData1.Id.Should().Be(saveData1.Id);
                loadedData1.RandomNumber.Should().Be(saveData1.RandomNumber);

                Thread.Sleep(100);

                var saveData2 = new TestSaveData();
                saveDataManager.Save(SaveDataFileName, ProjectName, saveData2);

                saveDataManager.TryLoad(SaveDataFileName, ProjectName, out TestSaveData loadedData2);
                loadedData2?.Id.Should().Be(saveData2.Id);
                loadedData2?.RandomNumber.Should().Be(saveData2.RandomNumber);
            }
        }
        else {
            throw new NullReferenceException(nameof(loadedData1));
        }

        saveDataManager.Delete(SaveDataFileName, ProjectName);
    }

    [Test]
    [Category("Integration Tests")]
    public static void WindowsSaveDataManager_SuccessfulSaveAndLoadTest() {
        var saveDataManager = new WindowsDataManager();

        var saveData1 = new TestSaveData();
        saveDataManager.Save(SaveDataFileName, ProjectName, saveData1);

        using (new AssertionScope()) {
            saveDataManager.TryLoad(SaveDataFileName, ProjectName, out TestSaveData loadedData);
            loadedData?.Id.Should().Be(saveData1.Id);
            loadedData?.RandomNumber.Should().Be(saveData1.RandomNumber);

            saveDataManager.TryLoad(SaveDataFileName, ProjectName, out VersionedData versionedLoadedData);
            loadedData?.TypeName.Should().Be(versionedLoadedData?.TypeName);
            typeof(VersionedData).IsAssignableFrom(loadedData?.GetType()).Should().BeTrue();
        }

        saveDataManager.Delete(SaveDataFileName, ProjectName);
    }

    private const string ProjectName = "Macabre2DTestProject";
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