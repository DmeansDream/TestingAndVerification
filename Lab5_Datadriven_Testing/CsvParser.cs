using System.IO;
using System;
using System.Collections.Generic;
using Codice.Client.Common;
using UnityEngine;

namespace Other
{
    public static class CsvParser
    {
        public static (string[,], int[,]) ParseCsv(string path)
        {
            
            if (!File.Exists(path))
            {
                Debug.Log("No File");
                return (new string[0,0], new int[0,0]);
            }
            
            string content;
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
            {
                content = streamReader.ReadToEnd();
            }
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            int rows =  lines.Length - 1;
            int cols = lines[0].Split(',').Length -1;
            
            string[,] data = new string[rows + 1, cols + 1];
            int[,] result = new int[rows, cols];

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length <= 1) continue;
                
                string[] values =  lines[i].Split(',');
                
                if (values.Length != cols + 1)
                {
                    throw new InvalidDataException($"Structural Error in CSV File: {Path.GetFileName(path)}");
                }
                
                for (int j = 0; j < values.Length; j++)
                {
                    data[i , j ] = values[j];
                    if (i == 0 || j == 0) continue;
                    if (!int.TryParse(values[j], out int parsedCost))
                    {
                        throw new FormatException($"Data Error in CSV File: {Path.GetFileName(path)}");
                    }
                    
                    result[i - 1, j - 1] = parsedCost;
                }
            }
            
            Debug.Log("CSV Data in 2D Array:");
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) 
                {
                    string cellValue = data[i, j] ?? "?";
                    Debug.Log(cellValue.PadRight(12));
                }
            }

            return (data, result);
        }
    }
}