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


            var bbb = fileSystemProvider.GetFileSystemEntries("C:\\Lot_Of_Files")
                .Select(x => fileSystemProvider.GetFileSystemDescriptor(x))
                .ToArray();
        }
    }
}