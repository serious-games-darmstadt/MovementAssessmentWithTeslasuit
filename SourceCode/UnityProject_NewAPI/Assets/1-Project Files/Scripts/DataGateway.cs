using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGateway : MonoBehaviour
{
    private PythonClient _pythonClient;
    public PythonClient PythonClient { get => _pythonClient; }
    // Start is called before the first frame update
    void Start()
    {
        _pythonClient = new PythonClient();
    }

    public void OnMocapUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        _pythonClient.Stop();
    }
}
