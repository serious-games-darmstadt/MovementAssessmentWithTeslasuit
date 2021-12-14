using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using TeslasuitAPI;
using Debug = UnityEngine.Debug;

public class PerformanceAnalyzer
{
    private static PerformanceAnalyzer INSTANCE;
    public Stopwatch _stopwatch = new Stopwatch();
    
    private int MocapUpdateCount = 0;

    private Dictionary<int, double> dataPointStreamTime = new Dictionary<int, double>();
    private List<double> roundTripTime = new List<double>();
    private List<double> tillSendTime = new List<double>();
    private List<double> tillReceiveTime = new List<double>();
    

    public static PerformanceAnalyzer GetInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new PerformanceAnalyzer();
        }

        return INSTANCE;
    }

    private PerformanceAnalyzer()
    {
        _stopwatch.Start();
    }

    public void reset()
    {
        MocapUpdateCount = 0;
        dataPointStreamTime = new Dictionary<int, double>();
        roundTripTime = new List<double>();
        tillSendTime = new List<double>();
        tillReceiveTime = new List<double>();
    }

    public void TSOnMocapDataUpdate(int dataIndex)
    {
        dataPointStreamTime[dataIndex] = _stopwatch.Elapsed.TotalMilliseconds;
        MocapUpdateCount++;
    }

    public void ErrorReceived(int dataIndex)
    {
        if (!dataPointStreamTime.ContainsKey(dataIndex)) return;
        double time = _stopwatch.Elapsed.TotalMilliseconds - dataPointStreamTime[dataIndex];
        // Debug.Log($"{dataIndex} {dataPointStreamTime[dataIndex]}  {_stopwatch.Elapsed.TotalMilliseconds}");
        roundTripTime.Add(time);
    }

    public void DataPointSend(int dataIndex)
    {
        if (!dataPointStreamTime.ContainsKey(dataIndex)) return;
        tillSendTime.Add(_stopwatch.Elapsed.TotalMilliseconds - dataPointStreamTime[dataIndex]);
    }

    public void DataPointReceived(int dataIndex)
    {
        if (!dataPointStreamTime.ContainsKey(dataIndex)) return;
        tillReceiveTime.Add(_stopwatch.Elapsed.TotalMilliseconds - dataPointStreamTime[dataIndex]);
    }

    public void stop()
    {
        _stopwatch.Stop();
        Debug.Log($"Streamed {MocapUpdateCount/_stopwatch.Elapsed.Seconds} updates per second.");

        if (roundTripTime.Count != 0)
        {
            Debug.Log($"Roundtrip Time: {roundTripTime.Average()} ms");
        }
        Debug.Log($"Till Send Time: {tillSendTime.Average()} ms");
        Debug.Log($"Till Receive Time: {tillReceiveTime.Average()} ms");
    }
}