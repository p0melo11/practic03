using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practic03
{
    public class TextFileStatistics
    {
        private readonly string _path;
        private string[] _words;

        public TextFileStatistics(string path)
        {
            _path = path;
            Load();
        }

        public Dictionary<string, int> CollectStatistic()
        {
            return _words.Aggregate(new Dictionary<string, int>(), (dict, word) =>
            {
                if (dict.ContainsKey(word))
                {
                    dict[word] += 1;

                    return dict;
                }

                dict[word] = 1;

                return dict;
            });
        }

        public void SaveReport(string path, Dictionary<string, int> report)
        {
            File.WriteAllText(path, SerializeReport(report));
        }

        private string SerializeReport(Dictionary<string, int> report)
        {
            var result = "";

            var sortedReport = report.ToList();
            sortedReport.Sort((entry1, entry2) => entry2.Value - entry1.Value);

            foreach (var entry in sortedReport)
            {
                result += $"{entry.Key}: {entry.Value}\n";
            }

            result.Remove(result.Length - 1);

            return result;
        }

        private void Load()
        {
            if (!Directory.Exists(_path))
            {
                return;
            }

            var files = Directory.GetFiles(_path);

            var txtFiles = files.ToList().Where((fileName) => fileName.EndsWith(".txt"));
            var words = new List<string>();

            foreach (var txtFile in txtFiles)
            {
                var content = File.ReadAllText(txtFile);

                words.AddRange(content.Split(' '));
            }

            _words = words.ToArray();
        }
    }
}
