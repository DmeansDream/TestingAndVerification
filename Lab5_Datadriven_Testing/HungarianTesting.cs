using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Other;
using ScriptsToTest;
using UnityEngine;

namespace Hungarian
{
    public class HungarianTesting
    {
        private const string PathToCsvFolder = "Assets/Scripts/Game/Other/Data/Hungarian";
        private string tempFilePath;
        
        [SetUp]
        public void Setup()
        {
            tempFilePath = Path.GetTempFileName();
        }
        
        [TearDown]
        public void Teardown()
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }

        [Test]
        public void TestParserForJaggedArrayErrorThrow()
        {
            string badData = 
                ",Guard,Attack,Explore\n" +
                "Steve,3,N/A,8\n" +
                "Jared,3,9,5";
            File.WriteAllText(tempFilePath, badData);
            
            var ex = Assert.Throws<FormatException>(() => CsvParser.ParseCsv(tempFilePath));
            
            Debug.Log("Successfully caught: " + ex.Message);
        }

        [Test]
        public void TestParserForNaNValueErrorThrow()
        {
            string badData = 
                ",Guard,Attack,Explore\n" +
                "Steve,3,4,8\n" +
                "Jared,3,9"; 
            File.WriteAllText(tempFilePath, badData);
            
            var ex = Assert.Throws<InvalidDataException>(() => CsvParser.ParseCsv(tempFilePath));
            
            Debug.Log("Successfully caught: " + ex.Message);
        }

        [Test]
        public void TestParserForMissingValueErrorThrow()
        {
            string badData = 
                ",Guard,Attack,Explore\n" +
                "Steve,3,,8\n" +
                "Jared,3,9,5";
            File.WriteAllText(tempFilePath, badData);
            
            var ex = Assert.Throws<FormatException>(() => CsvParser.ParseCsv(tempFilePath));
            
            Debug.Log("Caught: " + ex.Message);
        }
        
        [Test]
        [TestCaseSource(nameof(DataDrivenCases))]
        public void HungarianTest(string[,] raw, int[,] data)
        {
            var res = HungarianAlgorithm.FindAssignments(data);
            
            StringBuilder workStr = new StringBuilder();

            for (int i = 0; i < res.Length; i++)
            {
                string name = raw[i + 1, 0];
                string task = raw[0, res[i] + 1];
                
                workStr.AppendLine($"{name} ==> {task}");
            }
            
            Debug.Log(workStr.ToString());

        }

        public static IEnumerable<TestCaseData> DataDrivenCases()
        {
            string[] csvFiles = Directory.GetFiles(PathToCsvFolder, "*.csv");

            foreach (string filePath in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                var (rawMatrix, parsedData) = CsvParser.ParseCsv(filePath);

                yield return new TestCaseData(rawMatrix, parsedData).SetName($"DataDrivenTest_{fileName}");
            }
        }
    }
}