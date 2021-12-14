using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeslasuitAPI;

public class EnableLog : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TeslasuitAPI.Logger.Enabled = true;
        
        TeslasuitAPI.Logger.OnLogMessage += Logger_OnLogMessage;
    }

    private void Logger_OnLogMessage(string obj)
    {
        Debug.Log("log : " + obj);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
