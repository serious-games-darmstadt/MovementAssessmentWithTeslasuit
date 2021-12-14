using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeslasuitAPI.Tutorials
{
    public class ValueIntToText : MonoBehaviour
    {
        public Text text;
        // Use this for initialization
        void Start()
        {

        }

        public void SetValue(float value)
        {
            if (text != null)
                text.text = value.ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}