using Otus.Teaching.Concurrency.Import.Core.Loaders;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Otus.Teaching.Concurrency.Import.Loader.Loaders
{
    public class MyThreadPoolDataLoader
        : IDataLoader
    {
        private ICustomerRepository _customerRepository;
        private int _countThread;
        private int _loadedDataCount;
        private int _countTry;
        private static int _numerOfThreadsNotYetCompleted;
        public MyThreadPoolDataLoader(ICustomerRepository customerRepository, int countThread, int countTry)
        {
            _customerRepository = customerRepository;
            _countThread = countThread;
            _countTry = countTry;
            _numerOfThreadsNotYetCompleted = _countThread;
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
            MyDataLoad[] totalThread = new MyDataLoad[countThread];

            AutoResetEvent doneEvents = new AutoResetEvent(false);


            for (var i = 0; i < countThread; i++)
            {
                var startIndex = (int)i * (lenIntArray / countThread);
                var endIndex = ((int)i + 1) * lenIntArray / countThread;
                MyDataLoad f = new MyDataLoad(customers,startIndex,endIndex, _countTry, doneEvents/*autoResetEvent/*doneEvents[i]*/, _customerRepository);
                totalThread[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }
            doneEvents.WaitOne();
            
            for (int i = 0; i < countThread; i++)
            {
                _loadedDataCount += totalThread[i].LoadedDataCount;
            }

            Console.WriteLine("Loaded data...");

            return _loadedDataCount;
        }

        public class MyDataLoad
        {
            private ICustomerRepository _customerRepository;
            private List<Customer> _customers;
            private int _startIndex;
            private int _endIndex;
            private int _countTry;
            private AutoResetEvent _doneEvent;
            public int LoadedDataCount { get; private set; }
            public MyDataLoad(List<Customer> customers, int startIndex, int endIndex, int countTry, AutoResetEvent doneEvent, ICustomerRepository customerRepository)
            {
                _customers = customers;
                _startIndex = startIndex;
                _endIndex = endIndex;
                _countTry = countTry;
                _doneEvent = doneEvent;
                _customerRepository = customerRepository;
            }
            public void ThreadPoolCallback(Object threadContext)
            {
                int threadIndex = (int)threadContext;
                //Console.WriteLine("thread {0} started...", threadIndex);
                LoadedDataCount = LoadData(_startIndex, _endIndex, threadIndex);
                //Console.WriteLine("thread {0} result calculated...", threadIndex);
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                    _doneEvent.Set();
            }
            public int LoadData(int startIndex, int endIndex, int threadIndex)
            {
                int currentCountTry = 1;
                int j = startIndex;
                while (j < endIndex)
                {
                    try
                    {
                        _customerRepository.AddCustomer(_customers[j], threadIndex);
                        j++;
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
                return endIndex - startIndex;
            }

        }
    }
}
