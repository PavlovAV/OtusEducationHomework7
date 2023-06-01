using Otus.Teaching.Concurrency.Import.Handler.Data;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.DataGenerator.Generators
{
    public class CSVGenerator : IDataGenerator
    {
        private readonly string _fileName;
        private readonly int _dataCount;

        private StreamWriter writer { get; set; }
        public CSVGenerator(string fileName, int dataCount)
        {
            _fileName = fileName;
            _dataCount = dataCount;
        }

        public void Generate()
        {
            writer = new StreamWriter(_fileName, false);
            var customers = RandomCustomerGenerator.Generate(_dataCount);
            var type = typeof(Customer);
            string str = string.Empty;
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
            {
                str += $"{field.Name};";
            }
            str = $"{str[..^1]}";

            writer.WriteLine(str);

            foreach (var customer in customers)
            {
                str = string.Empty;
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                {
                    str += $"{field.GetValue(customer)};";
                }
                str = $"{str[..^1]}";
                writer.WriteLine(str);
            }
            writer.Close();
        }

    }
}
