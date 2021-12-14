using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Thesis;
using UnityEngine;

public class MotionRecorder: MonoBehaviour
{
    private bool _isRecording = false;
    private List<SuitData> _recordedSuitData = new List<SuitData>();
    
    private readonly FileManager _fileManager = new FileManager();

    public AudioClip count2Pause;
    private float[] count2PauseSegments = new[] {2f, 6f, 8f, 12f, 14f, 18, 20, 24f};
    public AudioClip count1Pause;
    private float[] count1PauseSegments = new[] {1f, 5f, 6f, 10f, 11f, 15, 16, 20f};
    public AudioClip countNoPause;
    private float[] countNoPauseSegments = new[] {1f, 5f, 5f, 9f, 9f, 13f, 13f, 17f};
    private AudioSource countSound;
    private float[] currentSegments;
    private int segmentIndex = 0;

    private Queue<string> segmentBuffer = new Queue<string>();
    public string Segment
    {
        get
        {
            if (segmentBuffer.Count > 0)
            {
                return segmentBuffer.Dequeue();
            }

            return "NONE";
        }
    }


    public void Start()
    {
        countSound = GameObject.Find("CountSound").GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (currentSegments != null)
        {
            if (countSound.time > currentSegments[segmentIndex])
            {
                // Debug.Log(countSound.time);
                if (segmentIndex % 2 == 0)
                {
                    segmentBuffer.Enqueue("START");
                }
                else
                {
                    segmentBuffer.Enqueue("END");
                }

                segmentIndex++;
            }

            if (segmentIndex == 7 && countSound.time == 0)
            {
                segmentBuffer.Enqueue("END");
            }

            if (segmentIndex == 8)
            {
                segmentIndex = 0;
                currentSegments = null;
            }
        }
    }

    public void OnMocapUpdate(SuitData suitData)
    {
        if (!_isRecording) return;
        _recordedSuitData.Add(suitData);
    }

    public void StartStopRecording(bool shouldRecord)
    {
        if (shouldRecord)
        {
            _recordedSuitData = new List<SuitData>();
        }
        _isRecording = shouldRecord;
    }

    public void Clear()
    {
        _recordedSuitData = new List<SuitData>();
    }

    public void Save(string subjectID, string datasetType)
    {
        _fileManager.save(_recordedSuitData, subjectID, datasetType);
        _fileManager.saveToCSV(_recordedSuitData, subjectID, datasetType);
    }

    public void PlayCountSound(DatasetType datasetType)
    {
        segmentIndex = 0;

        switch (datasetType)
        {
            case DatasetType.Squat_Slow:
            case DatasetType.Pushup_Slow:
            case DatasetType.Lunge_Slow:
                countSound.clip = count2Pause;
                currentSegments = count2PauseSegments;
                break;
            case DatasetType.Squat_Medium:
            case DatasetType.Pushup_Medium:
            case DatasetType.Lunge_Medium:
                countSound.clip = count1Pause;
                currentSegments = count1PauseSegments;
                break;
            case DatasetType.Squat_Fast:
            case DatasetType.Pushup_Fast:    
            case DatasetType.Lunge_Fast:
                countSound.clip = countNoPause;
                currentSegments = countNoPauseSegments;
                break;
            case DatasetType.Squat_Error1:
            case DatasetType.Squat_Error2:
            case DatasetType.Squat_Error3:
            case DatasetType.Pushup_Error1:
            case DatasetType.Pushup_Error2:
            case DatasetType.Pushup_Error3:
            case DatasetType.Lunge_Error1:
            case DatasetType.Lunge_Error2:
            case DatasetType.Lunge_Error3:
                countSound.clip = count1Pause;
                currentSegments = count1PauseSegments;
                break;
        }
        
        countSound.Play();
    }

    public void stopCount()
    {
        countSound.Stop();
        currentSegments = null;
    }


}
