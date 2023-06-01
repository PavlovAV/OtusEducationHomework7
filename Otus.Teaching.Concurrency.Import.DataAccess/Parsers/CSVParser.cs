using Otus.Teaching.Concurrency.Import.Core.Parsers;
using Otus.Teaching.Concurrency.Import.DataGenerator.Dto;
using Otus.Teaching.Concurrency.Import.Handler.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Parsers
{
    public class CSVParser
        : IDataParser<List<Customer>>
    {
        private StreamReader reader { get; set; }
        private Dictionary<int, string> fieldNamesInFile = new Dictionary<int, string>();

        public List<Customer> Parse(string dataFileName)
        {
            var customers = new List<Customer>();

            reader = new StreamReader(dataFileName);
            var strObj = reader.ReadLine();
            string[] strFields = strObj.Split(';');
            int i = 1;
            foreach (string strField in strFields)
            {
                fieldNamesInFile.Add(i++, strField);
            }

            var type = typeof(Customer);
            while (!reader.EndOfStream)
            {
                strObj = reader.ReadLine();
                if (strObj != null && strObj.Length > 1)
                {
                    strFields = strObj.Split(';');
                    Customer myDeserializedObj = null;
                    i = 1;
                    foreach (var strField in strFields)
                    {
                        if (strField != null && strField.Length > 0)
                        {
                            string fieldName = fieldNamesInFile[i++];
                            string fieldValue = strField;
                            if (fieldName.Length > 0 && fieldValue.Length > 0)
                            {
                                if (myDeserializedObj == null)
                                    myDeserializedObj = new Customer();
                                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                                fieldInfo.SetValue(myDeserializedObj, Convert.ChangeType(fieldValue, fieldInfo.FieldType));
                            }
                        }
                    }
                    if (myDeserializedObj != null)
                        customers.Add(myDeserializedObj);
                }
            }

            reader.Close();

            return customers;
        }
    }
}
