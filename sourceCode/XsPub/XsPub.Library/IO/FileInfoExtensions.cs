using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XsPub.Library.IO
{
    public static class FileInfoExtensions
    {
        public static void MoveTo(this FileInfo source, FileInfo destination)
        {
            source.MoveTo(destination.FullName);
        }

        public static void MoveTo(this FileInfo source, DirectoryInfo destination)
        {
            source.MoveTo(destination, source.Name);
        }

        public static void MoveTo(this FileInfo source, DirectoryInfo destination, string fileName)
        {
            if (!destination.Exists)
                destination.Create();

            source.MoveTo(
                Path.Combine(destination.FullName, fileName));
        }

        public static FileInfo CopyTo(this FileInfo source, DirectoryInfo destination)
        {
            return source.CopyTo(destination, source.Name);
        }

        public static FileInfo CopyTo(this FileInfo source, DirectoryInfo destination, bool overwrite)
        {
            return source.CopyTo(destination, source.Name, overwrite);
        }

        public static FileInfo CopyTo(this FileInfo source, DirectoryInfo destination, string fileName)
        {

            return source.CopyTo(destination, fileName, false);
        }

        public static FileInfo CopyTo(this FileInfo source, DirectoryInfo destination, string fileName, bool overwrite)
        {
            if (!destination.Exists)
                destination.Create();

            return source.CopyTo(
                Path.Combine(destination.FullName, fileName), overwrite);
        } 
    }
}