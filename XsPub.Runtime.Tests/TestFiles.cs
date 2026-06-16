using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using XsPub.Library.IO;

namespace XsPub.Runtime.Tests
{
    class TestFiles
    {
        private static readonly DirectoryInfo _inputDirectory = new DirectoryInfo("Input");
        private static readonly DirectoryInfo _defaultExpectedDirectory = new DirectoryInfo("Expected");
        private static readonly DirectoryInfo _defaultOutputDirectory = new DirectoryInfo("Output");

        public static TestFiles GetWsdlFiles(string testName = null)
        {
            return new TestFiles
                       {
                           Input = _inputDirectory.GetFiles("*.wsdl"),
                           OutputDirectory = testName == null ? _defaultOutputDirectory : new DirectoryInfo(_defaultOutputDirectory.FullName + testName),
                           ExpectedDirectory = testName == null ? _defaultExpectedDirectory : new DirectoryInfo(_defaultExpectedDirectory.FullName + testName)
                       };
        }

        public static TestFiles GetXsdFiles(string testName = null)
        {
            return new TestFiles
            {
                Input = _inputDirectory.GetFiles("*.xsd"),
                OutputDirectory = testName == null ? _defaultOutputDirectory : new DirectoryInfo(_defaultOutputDirectory.FullName + testName),
                ExpectedDirectory = testName == null ? _defaultExpectedDirectory : new DirectoryInfo(_defaultExpectedDirectory.FullName + testName)
            };
        }

        public static TestFiles GetXsdAndWsdlFiles(string testName = null)
        {
            return new TestFiles
            {
                Input = _inputDirectory.GetFiles("*.xsd").Concat(_inputDirectory.GetFiles("*.wsdl")),
                OutputDirectory = testName == null ? _defaultOutputDirectory : new DirectoryInfo(_defaultOutputDirectory.FullName + testName),
                ExpectedDirectory = testName == null ? _defaultExpectedDirectory : new DirectoryInfo(_defaultExpectedDirectory.FullName + testName)
            };
        }

        public IEnumerable<FileInfo> Input { get; private set; }
        public DirectoryInfo OutputDirectory { get; private set; }
        public DirectoryInfo ExpectedDirectory { get; private set; }
        
        public bool IgnoreUnexpected { get; private set; }

        public void CompareOutput()
        {
            //foreach (var fileInfo in Input)
            //{
            //    var outputFile = OutputDirectory.GetFile(fileInfo.Name);
            //    Assert.IsTrue(outputFile.Exists);
            //}
            foreach (var expected in ExpectedDirectory.GetFiles())
            {
                var actual = OutputDirectory.GetFile(expected.Name);
                Assert.AreEqual(ReadAllExpectedText(expected), File.ReadAllText(actual.FullName), "Actual and Expected {0} do not match", actual.Name);
            }
        }

        private string ReadAllExpectedText(FileInfo expected)
        {
            var data = File.ReadAllText(expected.FullName);
            // XmlWriter always creates end tags with a space before the />.
            data = data.Replace("\"/>", "\" />");
            // XmlWriter never writes whitespace after an attribute which is wrapped (on a new line).
            data = Regex.Replace(data, "(?<=<[^>]*)[ ]*\r\n(?=[^<]*>)", "\r\n");
            // .Net always writes the encoding name in lower case.
            data = data.Replace("\"UTF-8\"", "\"utf-8\"");
            return data;
        }
    }
}