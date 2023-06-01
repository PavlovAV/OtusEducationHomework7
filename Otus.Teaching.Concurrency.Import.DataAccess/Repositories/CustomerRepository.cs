using System;
using System.Diagnostics;
using System.Threading;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using Otus.Teaching.Concurrency.Import.Handler.Repositories;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class CustomerRepository
        : ICustomerRepository
    {
        public void AddCustomer(Customer customer, int threadNumber)
        {
            //Add customer to data source   
            Thread.Sleep(10);
            //Console.WriteLine($"{customer.Id} thread number {threadNumber} ...Add {customer}");
        }
    }
}