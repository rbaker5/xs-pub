using System.IO;

namespace XsPub.Library.IO;

public static class DirectoryInfoExtensions
{
    public static void Copy(this DirectoryInfo source, string fileName, DirectoryInfo destination)
    {
        File.Copy(
            Path.Join(source.FullName, fileName),
            Path.Join(destination.FullName, fileName));
    }

    public static FileInfo GetFile(this DirectoryInfo directoryInfo, string fileName)
    {
        return new FileInfo(Path.Join(directoryInfo.FullName, fileName));
    }

    public static DirectoryInfo GetDirectory(this DirectoryInfo directoryInfo, string dirName)
    {
        return new DirectoryInfo(Path.Join(directoryInfo.FullName, dirName));
    }

    public static void CopyTo(this DirectoryInfo sourceDir, DirectoryInfo destinationDir)
    {
        if (!destinationDir.Exists)
            destinationDir.Create();

        foreach (var file in sourceDir.GetFiles())
        {
            file.CopyTo(Path.Join(destinationDir.FullName, file.Name));
        }
        foreach (var sourceSubDirectory in sourceDir.GetDirectories())
        {
            var destinationSubDirectory = new DirectoryInfo(Path.Join(destinationDir.FullName, sourceSubDirectory.Name));
            sourceSubDirectory.CopyTo(destinationSubDirectory);
        }
    }
}