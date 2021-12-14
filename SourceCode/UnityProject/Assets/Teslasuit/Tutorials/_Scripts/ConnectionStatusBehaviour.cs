using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeslasuitAPI;

namespace TeslasuitAPI.Tutorials
{
    public class ConnectionStatusBehaviour : MonoBehaviour
    {
        public Text text;
        public SuitAPIObject aPIObject;

        // Use this for initialization
        void Start()
        {
            aPIObject.BecameAvailable += delegate { text.text = "Connected"; };
            aPIObject.BecameUnavailable += delegate { text.text = "Not Connected"; };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}