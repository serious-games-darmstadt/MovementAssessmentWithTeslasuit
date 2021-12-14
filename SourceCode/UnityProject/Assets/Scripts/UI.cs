using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TeslasuitAPI;
using Thesis;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private MotionRecorder _motionRecorder;
    private Text _recorderStatus;
    private Dropdown _performedExerciseDropdown;
    private Dropdown _applicationModeDropdown;
    private Dropdown _datasetTypeDropdown;
    private InputField _subjectIdInput;

    private MocapReplay _mocapReplay;
    private SuitAPIObject suitApi;

    private Dropdown exerciseLabelDropdown;
    private Slider replaySlider;

    // Start is called before the first frame update
    void Start()
    {
        DataGateway dataGateway = GameObject.Find("DataGateway").GetComponent<DataGateway>();
        _motionRecorder = dataGateway.GetComponent<MotionRecorder>();
        _recorderStatus = gameObject.transform.Find("RecordStatus").GetComponent<Text>();

        suitApi = GameObject.Find("Teslasuit_Man").GetComponent<SuitAPIObject>();
        suitApi.BecameAvailable += onSuitAvailable;
        suitApi.BecameUnavailable += onSuitUnavailable;

        _applicationModeDropdown = gameObject.transform.Find("ApplicationMode").GetComponent<Dropdown>();
        _applicationModeDropdown.ClearOptions();
        _applicationModeDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(ApplicationMode))));
        ApplicationModeChanged();

        _datasetTypeDropdown = gameObject.transform.Find("DatasetType").GetComponent<Dropdown>();
        _datasetTypeDropdown.ClearOptions();
        _datasetTypeDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(DatasetType))));
        DatasetTypeSelected();

        _subjectIdInput = gameObject.transform.Find("FieldSubjectID").GetComponent<InputField>();


        _mocapReplay = GameObject.Find("Teslasuit_Man").GetComponent<MocapReplay>();

        exerciseLabelDropdown = gameObject.transform.Find("LabelDropdown").GetComponent<Dropdown>();
        exerciseLabelDropdown.ClearOptions();
        exerciseLabelDropdown.AddOptions(new List<string>(Enum.GetNames(typeof(ExerciseLabel))));
        replaySlider = gameObject.transform.Find("ReplaySlider").GetComponent<Slider>();
    }

    public void onSuitAvailable(SuitHandleObject handleObject)
    {
        Text suitStatus = gameObject.transform.Find("SuitStatus").GetComponent<Text>();
        suitStatus.text = "Teslasuit Status: Connected";
    }

    public void onSuitUnavailable(SuitHandleObject handleObject)
    {
        Text suitStatus = gameObject.transform.Find("SuitStatus").GetComponent<Text>();
        suitStatus.text = "Teslasuit Status: Disconnected";
    }

    public void StartRecording()
    {
        _motionRecorder.StartStopRecording(true);
        _recorderStatus.text = "Recording: Started";
    }

    public void OnStopRecord()
    {
        _motionRecorder.StartStopRecording(false);
        _recorderStatus.text = "Recording: Stopped";
    }

    public void SaveRecordedData()
    {
        string datasetType = _datasetTypeDropdown.options[_datasetTypeDropdown.value].text;
        string subjectID = _subjectIdInput.text;

        _motionRecorder.Save(subjectID, datasetType);
    }

    public void saveLabels()
    {
        _mocapReplay.saveLabels();
    }

    public void onLoadButtonClicked()
    {
        string datasetType = _datasetTypeDropdown.options[_datasetTypeDropdown.value].text;
        string subjectID = _subjectIdInput.text;

        _mocapReplay.load(subjectID, datasetType);
        Text replayStatus = gameObject.transform.Find("ReplayStatus").GetComponent<Text>();
        replayStatus.text = $"Replay: {subjectID}/{datasetType}";
    }

    public void StartStopReplay()
    {
        _mocapReplay.startStopReplay();

        Text pauseButton = gameObject.transform.Find("StartStopReplay").GetComponentInChildren<Text>();
        pauseButton.text = _mocapReplay.DoReplay ? "Stop" : "Start";
    }

    public void OnPauseResumeReplay()
    {
        _mocapReplay.pauseResumeReplay();
        Text pauseButton = gameObject.transform.Find("PauseReplay").GetComponentInChildren<Text>();
        pauseButton.text = _mocapReplay.ReplayPaused ? "Resume" : "Pause";
    }

    public void OnMarkLabelStart()
    {
        string label = exerciseLabelDropdown.options[exerciseLabelDropdown.value].text;
        ExerciseLabel exerciseLabel = (ExerciseLabel) Enum.Parse(typeof(ExerciseLabel), label);
        _mocapReplay.markLabelStart(exerciseLabel);
    }

    public void OnMarkLabelEnd()
    {
        _mocapReplay.markLabelStop();
    }

    public void sliderValueChanged()
    {
        _mocapReplay.sliderValueChanged(replaySlider.value);
    }

    public void ApplicationModeChanged()
    {
        string applicationModeString = _applicationModeDropdown.options[_applicationModeDropdown.value].text;
        ApplicationMode applicationMode = (ApplicationMode) Enum.Parse(typeof(ApplicationMode), applicationModeString);
        Config.APPLICATION_MODE = applicationMode;
    }

    public void ExerciseSelected()
    {
        string exerciseString = _performedExerciseDropdown.options[_performedExerciseDropdown.value].text;
        Exercise exercise = (Exercise) Enum.Parse(typeof(Exercise), exerciseString);
        Config.SELECTED_EXERCISE = exercise;
    }

    public void DatasetTypeSelected()
    {
        string datasetTypeString = _datasetTypeDropdown.options[_datasetTypeDropdown.value].text;
        DatasetType datasetType = (DatasetType) Enum.Parse(typeof(DatasetType), datasetTypeString);

        switch (datasetType)
        {
            case DatasetType.Squat_Fast:
            case DatasetType.Squat_Medium:
            case DatasetType.Squat_Slow:
            case DatasetType.Squat_Error1:
            case DatasetType.Squat_Error2:
            case DatasetType.Squat_Error3:
                Config.SELECTED_EXERCISE = Exercise.Squat;
                break;
            case DatasetType.Pushup_Fast:
            case DatasetType.Pushup_Medium:
            case DatasetType.Pushup_Slow:
            case DatasetType.Pushup_Error1:
            case DatasetType.Pushup_Error2:
            case DatasetType.Pushup_Error3:
                Config.SELECTED_EXERCISE = Exercise.Pushup;
                break;
            case DatasetType.Lunge_Fast:
            case DatasetType.Lunge_Medium:
            case DatasetType.Lunge_Slow:
            case DatasetType.Lunge_Error1:
            case DatasetType.Lunge_Error2:
            case DatasetType.Lunge_Error3:
                Config.SELECTED_EXERCISE = Exercise.Lunge;
                break;
        }
    }

    public void ClearRecording()
    {
        _motionRecorder.Clear();
    }

    public void PlayCount()
    {
        string datasetTypeString = _datasetTypeDropdown.options[_datasetTypeDropdown.value].text;
        DatasetType datasetType = (DatasetType) Enum.Parse(typeof(DatasetType), datasetTypeString);
        _motionRecorder.PlayCountSound(datasetType);
    }

    public void StopCount()
    {
        _motionRecorder.stopCount();
    }
}