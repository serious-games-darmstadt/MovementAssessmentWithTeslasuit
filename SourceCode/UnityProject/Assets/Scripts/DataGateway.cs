using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace;
using TeslasuitAPI;
using Thesis;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;


public class DataGateway : MonoBehaviour
{
    public MotionCapture motionCapture;
    public MocapReplay mocapReplay;
    
    public SuitAPIObject suitApi;
    private MocapSkeleton _skeleton;

    public MotionRecorder _motionRecorder;
    public Exercise recognizedExercise = Exercise.Pushup;

    private PythonApiClient _pythonApiClient;

    private Stopwatch _stopwatch;

    private AudioSource timeCount;
    private bool audioIsPlaying = false;

    private void Start()
    {
        _motionRecorder = gameObject.GetComponent<MotionRecorder>();
        MocapJoints mocapJoints = MocapJoints.GetInstance();
        _stopwatch = PerformanceAnalyzer.GetInstance()._stopwatch;

        timeCount = GameObject.Find("CountSound").GetComponent<AudioSource>();
        
        _pythonApiClient = new PythonApiClient();

        StartCoroutine(UpdateMocapOptions());
    }
    
    private IEnumerator UpdateMocapOptions()
    {
        yield return new WaitUntil(() => suitApi.Mocap is {isStarted: true});
        
        suitApi.Mocap.Updated += OnMocapUpdate;

        TSMocapOptions thesisOptions = new TSMocapOptions();
        thesisOptions.frequency = TSMocapFrequency.TS_MOCAP_FPS_50;
        thesisOptions.sensors_mask = Config.TsMocapSensorMask();
        
        suitApi.Mocap.UpdateOptions(thesisOptions);
        _skeleton = suitApi.Mocap.Skeleton;
        Debug.Log($"Updated Mocap Options. Sensor mask is {thesisOptions.sensors_mask}");
    }
    
    
    private void OnMocapUpdate()
    {
        int timestamp = (int) _stopwatch.Elapsed.TotalMilliseconds;
        PerformanceAnalyzer.GetInstance().TSOnMocapDataUpdate(timestamp);
        SuitData suitData;
        String segment = _motionRecorder.Segment;

        if (mocapReplay.DoReplay)
        {
            SuitData replayData = mocapReplay.GetCurrentReplayData();

            Vector3[] jointData;
            if (Config.OVERWRITE_JOINT_DATA)
            {
                jointData = motionCapture.JointData.Values.ToArray();
            }
            else
            {
                jointData = replayData.jointData;
            }
            
            suitData = new SuitData(replayData.data, timestamp, jointData, segment);
        }
        else
        {
            TSMocapData[] data = _skeleton.mocapData;
            TSMocapData[] slicedData = new TSMocapData[10];
            Array.Copy(data, slicedData, 10);
            suitData = new SuitData(slicedData, timestamp, motionCapture.JointData.Values.ToArray(), segment);
        }

        _pythonApiClient.pushSuitData(suitData);
        _motionRecorder.OnMocapUpdate(suitData);
        PerformanceAnalyzer.GetInstance().DataPointSend(timestamp);
    }

    private void OnDestroy()
    {
        _pythonApiClient.Stop();
        PerformanceAnalyzer.GetInstance().stop();
    }
}