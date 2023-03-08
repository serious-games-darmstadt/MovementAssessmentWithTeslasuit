using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayObject
{
    public string subjectName;
    public List<ReplayInfo> replayInfo = new List<ReplayInfo>();
    public TrainingTypes trainingType = TrainingTypes.Lunge;

    public ReplayObject() { }
    public ReplayObject(string name, List<ReplayInfo> info, TrainingTypes training)
    {
        subjectName = name;
        replayInfo = info;
        trainingType = training;
    }
    // public List<float> timeStamp;   
}