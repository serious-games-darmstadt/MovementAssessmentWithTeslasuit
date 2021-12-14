using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeslasuitAPI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Thesis
{
    public class FileManager
    {
        private SurrogateSelector _surrogateSelector;
        private string seperator = ";";

        public FileManager()
        {
            _surrogateSelector = new SurrogateSelector();
            _surrogateSelector.AddSurrogate(typeof(TSMocapData), new StreamingContext(StreamingContextStates.All),
                new TSMocapDataSurrogate());
            _surrogateSelector.AddSurrogate(typeof(Quat4f), new StreamingContext(StreamingContextStates.All),
                new Quat4fSurrogate());
            _surrogateSelector.AddSurrogate(typeof(Vector3s), new StreamingContext(StreamingContextStates.All),
                new Vector3sSurrogate());
            _surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All),
                new Vector3Surrogate());
        }
        
        
        public void save(List<SuitData> data, string subjectID, string datasetType)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            string path = $"{Application.dataPath}/data/{subjectID}/{subjectID}_{datasetType}.tsdat";
            FileStream file;

            if(File.Exists(path)) file = File.OpenWrite(path);
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                file = File.Create(path);
            }
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.SurrogateSelector = _surrogateSelector;
            bf.Serialize(file, data);
            file.Close();
            
            stopwatch.Stop();
        }

        public void saveToCSV(List<SuitData> data, string subjectID, string datasetType)
        {
            StringBuilder sb = new StringBuilder();
            
            // To let Excel know
            sb.Append("SEP=").Append(seperator).Append("\n");
            sb.Append(data[0].GetCsvHeader(seperator));
            foreach (var suitData in data)
            {
                sb.Append(suitData.ToCSV(seperator)).Append("\n");
            }
            
            string path = $"{Application.dataPath}/data/{subjectID}/{subjectID}_{datasetType}.csv";

            using(var writer = new StreamWriter(path, false))
            {
                writer.Write(sb.ToString());
            }

            Debug.Log($"Saved {subjectID}_{datasetType}.csv");
        }

        public List<SuitData> load(string subjectID, string datasetType)
        {
            string path = $"{Application.dataPath}/data/{subjectID}/{subjectID}_{datasetType}.tsdat";
            FileStream file;
            
            if(File.Exists(path)) file = File.OpenRead(path);
            else
            {
                Debug.LogError("Suit data file not found");
                return null;
            }
 
            BinaryFormatter bf = new BinaryFormatter();
            bf.SurrogateSelector = _surrogateSelector;
            
            List<SuitData> data = (List<SuitData>) bf.Deserialize(file);
            file.Close();

            return data;
        }
    }
}