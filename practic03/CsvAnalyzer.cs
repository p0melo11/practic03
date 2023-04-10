using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practic03
{
    public class CsvAnalyzer
    {
        private string _path;
        private string _outPath = "/home/sawinjer/results";
        public List<CsvFileRecord> Records = new List<CsvFileRecord>();
        public CsvAnalyzer(string path)
        {
            _path = path;
            Load();
        }

        public Dictionary<string, double> AggregateByDate()
        {
            return Records.Aggregate(new Dictionary<string, double>(), (dict, record) =>
            {
                if (!dict.ContainsKey(record.Date))
                {
                    dict[record.Date] = record.Money;

                    return dict;
                }

                dict[record.Date] += record.Money;

                return dict;
            });
        }

        public void Save()
        {
            if (!Directory.Exists(_outPath))
            {
                Directory.CreateDirectory(_outPath);
            }

            var fileIndex = 1;
            var bunchSize = 10;
            var bunch = new List<CsvFileRecord>();

            foreach (var record in Records)
            {
                bunch.Add(record);

                if (bunch.Count != bunchSize)
                {
                    continue;
                }

                File.WriteAllText(Path.Combine(_outPath, $"data_{fileIndex}.csv"), CsvFileRecord.Serialize(bunch));

                bunch.Clear();
                fileIndex += 1;
            }

            if (bunch.Count != 0)
            {
                File.WriteAllText(Path.Combine(_outPath, $"data_{fileIndex}.csv"), CsvFileRecord.Serialize(bunch));
            }
        }

        private void Load()
        {
            using TextFieldParser textFieldParser = new TextFieldParser(_path);
            textFieldParser.TextFieldType = FieldType.Delimited;
            textFieldParser.SetDelimiters(",");

            var isHeaders = true;
            Records.Clear();

            while (!textFieldParser.EndOfData)
            {
                var columns = textFieldParser.ReadFields();

                if (columns == null || columns.Length < 2)
                {
                    Console.WriteLine("row had been skipped");
                    continue;
                }

                if (isHeaders)
                {
                    isHeaders = false;
                    continue;
                }

                try
                {
                    var record = new CsvFileRecord();

                    record.Date = columns[0];
                    record.Money = Double.Parse(columns[1].Replace(',', '.'));

                    Records.Add(record);
                }
                catch
                {
                    Console.WriteLine("row had been skipped");
                }
            }
        }
    }
}
