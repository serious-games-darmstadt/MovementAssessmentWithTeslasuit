using System;
using System.Collections.Generic;
using System.Diagnostics;
using TeslasuitAPI;
using TeslasuitAPI.Utils;
using Thesis;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MocapReplay : MonoBehaviour
{
    [SerializeField] private SuitMocapSkeleton suitMocapSkeleton;

    private List<SuitData> replayData;
    private int replayIndex;
    private Boolean doReplay = false;
    public bool DoReplay => doReplay;

    private Boolean replayPaused = true;
    public bool ReplayPaused => replayPaused;
    
    private double nextReplayTime;
    private double firstReplayPointOffset;
    private double repReplayStartTime;

    private int labelStartIndex = -1;
    private ExerciseLabel _label;
    
    private FileManager _fileManager = new FileManager();

    private Stopwatch _stopwatch = new Stopwatch();

    
    public void load(string subjectId, string datasetType)
    {
        replayIndex = 0;
        replayData = _fileManager.load(subjectId, datasetType);
    }

    public void startStopReplay()
    {
        replayIndex = 0;
        doReplay = !doReplay;
        firstReplayPointOffset = replayData[0].timestamp;
        nextReplayTime = replayData[replayIndex + 1].timestamp - firstReplayPointOffset;
        repReplayStartTime = 0;
    }

    public void pauseResumeReplay()
    {
        replayPaused = !replayPaused;
        
        if (replayPaused)
            _stopwatch.Stop();
        else
        {  
            PerformanceAnalyzer.GetInstance().reset();
            _stopwatch.Start();
        }
    }

    public void markLabelStart(ExerciseLabel label)
    {
        _label = label;
        labelStartIndex = replayIndex;
    }

    public void markLabelStop()
    {
        int labelStopIndex = replayIndex;

        for (int i = labelStartIndex; i <= labelStopIndex; i++)
        {
            replayData[i].label = _label;
        }
    }

    public void saveLabels()
    {
        _fileManager.saveToCSV(replayData, "", "");
    }

    public Quat4f GetCurrentReplayRotation(ulong boneIndex)
    {
        int nodeIndex = FindNodeIndex(boneIndex);
        if (nodeIndex == -1) return Quat4f.Identity;
        return replayData[replayIndex].data[nodeIndex].quat9x;
    }

    public SuitData GetCurrentReplayData()
    {
        SuitData data = replayData[replayIndex];

        if (!replayPaused)
        {
            replayIndex = (replayIndex + 1) % replayData.Count;
        }
            
        return data;
    }

    private int FindNodeIndex(ulong boneIndex)
    {
        for (int index = 0; index < this.replayData[replayIndex].data.Length; ++index)
        {
            if ((long) this.replayData[replayIndex].data[index].mocap_bone_index == (long) boneIndex)
                return index;
        }

        return -1;
    }

    public void sliderValueChanged(float value)
    {
        replayIndex = (int) (value * replayData.Count);
    }
}