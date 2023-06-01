using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.DataGenerator.Generators;
using Otus.Teaching.Concurrency.Import.DataAccess.Parsers;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.Loader.Loaders;

namespace Otus.Teaching.Concurrency.Import.Loader
{
    class Program
    {
        private static string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers.xml");
        private static int _count = 1000;
        private static int _countThread = 1000;
        private static int _countTry = 3;
        private static Stopwatch sw = new Stopwatch();


        static void Main(string[] args)
        {
            Console.WriteLine($"Main process Id {Process.GetCurrentProcess().Id}...");
            //args = new string[4];
            //args[0] = "D:\\PAVlovAV\\CSharpProjects\\OtusEducationProjects\\Otus.Teaching.Concurrency.Import\\Otus.Teaching.Concurrency.Import.DataGenerator.App\\bin\\Debug\\netcoreapp3.1\\customers.csv";
            //args[1] = "1000000";
            //args[2] = "1000";
            //args[3] = "D:\\PAVlovAV\\CSharpProjects\\OtusEducationProjects\\Otus.Teaching.Concurrency.Import\\Otus.Teaching.Concurrency.Import.DataGenerator.App\\bin\\Debug\\netcoreapp3.1\\Otus.Teaching.Concurrency.Import.DataGenerator.App.exe";
            if (args != null)
            {
                if (args.Length > 0)
                {
                    _dataFilePath = args[0];
                }

                if (args.Length > 1)
                {
                    int.TryParse(args[1],out _count);
                }
                
                if (args.Length > 2)
                {
                    int.TryParse(args[2], out _countThread);
                }

                if (args.Length > 3)
                {
                    GenerateCustomersDataFileUsingProcess(args[3]);
                }
                else
                {
                    GenerateCustomersDataFile();
                }
            }

            sw.Start();
            var parser = new CSVParser();
            var customers = parser.Parse(_dataFilePath);
            sw.Stop();
            Console.WriteLine($" Parse time {sw.ElapsedMilliseconds / 1000.0}");

            sw.Restart();
            var loader = new MyDataLoader(new CustomerRepository(), _countThread, _countTry);

            loader.LoadData(customers);
            sw.Stop();
            Console.WriteLine($"MyDataLoader time {sw.ElapsedMilliseconds / 1000.0}");

            sw.Restart();
            var threadPoolLoader = new MyThreadPoolDataLoader(new CustomerRepository(), _countThread, _countTry);

            threadPoolLoader.LoadData(customers);
            sw.Stop();
            Console.WriteLine($"MyThreadPoolDataLoader time {sw.ElapsedMilliseconds / 1000.0}");
        }

        static void GenerateCustomersDataFile()
        {
            Console.WriteLine($"Loader started with process Id {Process.GetCurrentProcess().Id}...");
            var xmlGenerator = new CSVGenerator(_dataFilePath, _count);
            xmlGenerator.Generate();
        }

        static void GenerateCustomersDataFileUsingProcess(string fileGenerator)
        {
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = fileGenerator;
                myProcess.StartInfo.Arguments = $"{_dataFilePath} {_count}";
                //myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                Console.WriteLine($"Loader started with process Id {myProcess.Id}...");
                myProcess.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}