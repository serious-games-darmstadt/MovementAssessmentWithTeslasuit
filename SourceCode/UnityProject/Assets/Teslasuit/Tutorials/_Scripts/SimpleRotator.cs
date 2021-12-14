using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI.Tutorials
{
    public class SimpleRotator : MonoBehaviour
    {
        float saveValue;

        public void Awake()
        {
            saveValue = transform.rotation.eulerAngles.y;
        }
        public void SetYRotation(float y)
        {
            //var temp= transform.rotation;
            var temp = Quaternion.AngleAxis(saveValue + y, Vector3.up);
            transform.rotation = temp;
        }

        public void AddAngle(float y)
        {
            transform.rotation *= Quaternion.Euler(0, y, 0);
        }

    }
}