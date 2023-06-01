using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Entities;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class XmlParser
        : IDataParser<List<Customer>>
    {
        public List<Customer> Parse(string dataFileName)
        {
            var customers = new List<Customer>();
            XmlSerializer serializer = new XmlSerializer(typeof(CustomersList));

            using (Stream reader = new FileStream(dataFileName, FileMode.Open))
            {
                customers = ((CustomersList)serializer.Deserialize(reader)).Customers;
            }
            return customers;
        }
    }
}