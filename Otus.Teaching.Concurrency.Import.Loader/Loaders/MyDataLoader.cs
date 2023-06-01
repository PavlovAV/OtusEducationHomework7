using Bogus.Bson;
using Otus.Teaching.Concurrency.Import.DataAccess.Repositories;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Otus.Teaching.Concurrency.Import.Core.Loaders
{
    public class MyDataLoader
        : IDataLoader
    {
        private ICustomerRepository _customerRepository;
        private int _countThread;
        private int _loadedDataCount;
        private int _countTry;
        public MyDataLoader(ICustomerRepository customerRepository, int countThread, int countTry)
        {
            _customerRepository = customerRepository;
            _countThread = countThread;
            _countTry= countTry;
        }
        public int LoadData(List<Customer> customers)
        {
            Console.WriteLine($"Loading data ...");
            var listThreads = new List<Thread>();
            int lenIntArray = customers.Count;
            int countThread = _countThread;
            int countItemInOneThread = lenIntArray / countThread;
            if (countThread * countItemInOneThread < lenIntArray)
                countThread++;
            int[] totalThread = new int[countThread];

            for (var i = 0; i < countThread; i++)
            {
                var startIndex = (int)i * (lenIntArray / countThread);
                var endIndex = ((int)i + 1) * lenIntArray / countThread;
                var thread = new Thread((i) =>
                {
                    int currentCountTry = 1;
                    int j = startIndex;
                    while (j < endIndex)
                    {
                        try
                        {
                            _customerRepository.AddCustomer(customers[j], (int)i);
                            j++;
                            totalThread[(int)i]++;
                            currentCountTry = 1;
                        }
                        catch 
                        {
                            if (currentCountTry < _countTry)
                            {
                                currentCountTry++;
                            }
                            else
                            {
                                j++;
                                currentCountTry = 1;
                            }
                        }
                    }
                });
                listThreads.Add(thread);
                listThreads[i].Start(i);
            }

            for (int i = 0; i < countThread; i++)
            {
                listThreads[i].Join();
            }

            for (int i = 0; i < countThread; i++)
            {
                _loadedDataCount += totalThread[i];
            }

            Console.WriteLine("Loaded data...");

            return _loadedDataCount;
        }
    }
}