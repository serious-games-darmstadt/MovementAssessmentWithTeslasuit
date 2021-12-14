using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHeading : MonoBehaviour {
    public Transform target;
    private Vector3 offset = Vector3.zero;
    // Use this for initialization
    void Start () {
        offset = target.eulerAngles - transform.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateEuler();
    }


    void UpdateEuler()
    {
        Vector3 r = transform.eulerAngles;

        r.y = target.eulerAngles.y - offset.y;
        transform.eulerAngles = r;

    }



}
