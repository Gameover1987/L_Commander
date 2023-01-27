using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace L_Commander.Tests
{
    [TestClass]
    public class FileManagerTabViewModelTest : AutoMockerTestsBase<FileManagerTabViewModel>
    {
        [TestMethod]
        public void InitTest()
        {
            var fileSystemProvider = new FileSystemProvider(new IconCache());

            var aaa = fileSystemProvider.GetFileSystemEntries("C:\\Lot_Of_Files").ToArray();


            var directory = new DirectoryInfo("E:\\Download");
            var files = directory.EnumerateFileSystemInfos("*.*", SearchOption.AllDirectories).ToArray();
            var directoris = directory.EnumerateDirectories("*.*", SearchOption.AllDirectories).ToArray();

            //var bbb = fileSystemProvider.GetFileSystemEntries("E:\\Download")
            //    .Select(x => fileSystemProvider.GetFileSystemDescriptor(x))
            //    .ToArray();
        }
    }
}