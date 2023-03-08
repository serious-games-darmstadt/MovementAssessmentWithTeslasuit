using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

/// <summary>
/// Currently dropped, because the adaptation of old data is somewhat problematic. 
/// </summary>
public class CsvEditor 
{
    const string PATH = @"C:\StudentProjects\Burakhan\Tesla Suit\Assets\OldCSVFiles\";

    private static List<string> _oldCSVData = new List<string>();
    //original comparison of boneIndex
    private static Dictionary<string, TsHumanBoneIndex> _oldNewBonePair = new Dictionary<string, TsHumanBoneIndex>()
    {
        { "1",TsHumanBoneIndex.Hips },
        { "2",TsHumanBoneIndex.LeftUpperLeg },
        { "4",TsHumanBoneIndex.RightUpperLeg },
        { "8",TsHumanBoneIndex.LeftLowerLeg },
        { "16",TsHumanBoneIndex.RightLowerLeg },
        { "32",TsHumanBoneIndex.LeftFoot },
        { "64",TsHumanBoneIndex.RightFoot },
        { "128",TsHumanBoneIndex.Spine },
        { "256",TsHumanBoneIndex.Chest },
        { "512",TsHumanBoneIndex.Neck },
        { "1024",TsHumanBoneIndex.Head },
        { "2048",TsHumanBoneIndex.LeftShoulder },
        { "4096",TsHumanBoneIndex.RightShoulder },
        { "8192",TsHumanBoneIndex.LeftUpperArm },
        { "16384",TsHumanBoneIndex.RightUpperArm },
        { "32768",TsHumanBoneIndex.LeftLowerArm },
        { "65536",TsHumanBoneIndex.RightLowerArm },
        { "131072",TsHumanBoneIndex.LeftHand },
        { "262144",TsHumanBoneIndex.RightHand },
    };
    //For trying out different combinations
    private static Dictionary<string, TsHumanBoneIndex> _oldNewBonePair2 = new Dictionary<string, TsHumanBoneIndex>()
    {
        { "1",TsHumanBoneIndex.Hips },
        { "4",TsHumanBoneIndex.LeftUpperLeg },
        { "2",TsHumanBoneIndex.RightUpperLeg },
        { "16",TsHumanBoneIndex.LeftLowerLeg },
        { "8",TsHumanBoneIndex.RightLowerLeg },
        { "64",TsHumanBoneIndex.LeftFoot },
        { "32",TsHumanBoneIndex.RightFoot },
        { "128",TsHumanBoneIndex.Spine },
        { "256",TsHumanBoneIndex.Chest },
        { "512",TsHumanBoneIndex.Neck },
        { "1024",TsHumanBoneIndex.Head },
        { "4096",TsHumanBoneIndex.LeftShoulder },
        { "2048",TsHumanBoneIndex.RightShoulder },
        { "16384",TsHumanBoneIndex.LeftUpperArm },
        { "8192",TsHumanBoneIndex.RightUpperArm },
        { "65536",TsHumanBoneIndex.LeftLowerArm },
        { "32768",TsHumanBoneIndex.RightLowerArm },
        { "262144",TsHumanBoneIndex.LeftHand },
        { "131072",TsHumanBoneIndex.RightHand },
    };
    public static void DetectCSV()
    {
        DirectoryInfo dir = new System.IO.DirectoryInfo(PATH);
        List<string> filenames = new List<string>();
        foreach (FileInfo f in dir.GetFiles())
        {
            string fileName = f.Name;
            //dont consider meta data they are for git
            if (fileName.Contains("meta")) continue;
            if (fileName.Contains(".csv")) _oldCSVData.Add(f.FullName); 

        }
        ReadCSV();
    }

    public static void ReadCSV()
    {
        //Currently reading only first data
        string[] csvLines = File.ReadAllLines(_oldCSVData[0]);

        //List<string> lines = new List<string>();

       //// for(int i = 1; i < csvLines.Length; i++)
       // for(int i = 1; i < 2; i++)
       // {
       //     string[] rowData = csvLines[i].Split(';');
       //     lines.Add(rowData[0]);
       //     UnityEngine.Debug.Log(rowData[0]+" "+ rowData[1] + " "+rowData.Length);
       // }

        UnityEngine.Debug.Log("ReadCSVDone!");

        CSVtoReplayObject(csvLines);

    }

    public static void CSVtoReplayObject(string[] lines)
    {
       
        List<ReplayInfo> replayInfos = new List<ReplayInfo>();
        

        // 0 index, 1 label, we begin at 2 
        for (int i = 2; i < lines.Length; i++)
        {
            string[] rowData = lines[i].Split(';');
            
            ReplayInfo replayInfo = new ReplayInfo();
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[2]], CSVtoMyQuaternion(rowData[3], rowData[4], rowData[5], rowData[6]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[13]], CSVtoMyQuaternion(rowData[14], rowData[15], rowData[16], rowData[17]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[24]], CSVtoMyQuaternion(rowData[25], rowData[26], rowData[27], rowData[28]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[35]], CSVtoMyQuaternion(rowData[36], rowData[37], rowData[38], rowData[39]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[46]], CSVtoMyQuaternion(rowData[47], rowData[48], rowData[49], rowData[50]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[57]], CSVtoMyQuaternion(rowData[58], rowData[59], rowData[60], rowData[61]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[68]], CSVtoMyQuaternion(rowData[69], rowData[70], rowData[71], rowData[72]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[79]], CSVtoMyQuaternion(rowData[80], rowData[81], rowData[82], rowData[83]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[90]], CSVtoMyQuaternion(rowData[91], rowData[92], rowData[93], rowData[94]));
            replayInfo.replayRotationQuaternion.Add(_oldNewBonePair[rowData[101]], CSVtoMyQuaternion(rowData[102], rowData[103], rowData[104], rowData[105]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[2]], CSVtoMyQuaternion(rowData[3], rowData[4], rowData[5], rowData[6]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[13]], CSVtoMyQuaternion(rowData[14], rowData[15], rowData[16], rowData[17]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[24]], CSVtoMyQuaternion(rowData[25], rowData[26], rowData[27], rowData[28]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[35]], CSVtoMyQuaternion(rowData[36], rowData[37], rowData[38], rowData[39]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[46]], CSVtoMyQuaternion(rowData[47], rowData[48], rowData[49], rowData[50]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[57]], CSVtoMyQuaternion(rowData[58], rowData[59], rowData[60], rowData[61]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[68]], CSVtoMyQuaternion(rowData[69], rowData[70], rowData[71], rowData[72]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[79]], CSVtoMyQuaternion(rowData[80], rowData[81], rowData[82], rowData[83]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[90]], CSVtoMyQuaternion(rowData[91], rowData[92], rowData[93], rowData[94]));
            //replayInfo.replayRotationQuaternion.Add(_oldNewBonePair2[rowData[101]], CSVtoMyQuaternion(rowData[102], rowData[103], rowData[104], rowData[105]));
            replayInfos.Add(replayInfo);

        }
        ReplayObject replayObject = new ReplayObject("deneme", replayInfos,TrainingTypes.Lunge);
        string json = JsonConvert.SerializeObject(replayObject.replayInfo.ToArray(), Formatting.Indented);

        //write string to file
        System.IO.File.WriteAllText(string.Concat(@"C:\StudentProjects\Burakhan\Tesla Suit\Assets\JsonAttempts\\", $"{replayObject.subjectName}", ".json"), json);
    }

    public static MyQuaternion CSVtoMyQuaternion(string w, string x, string y, string z)
    {

        return new MyQuaternion(float.Parse(x, CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(y, CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(z, CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(w, CultureInfo.InvariantCulture.NumberFormat));
    }
}
