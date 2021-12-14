using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI.Tutorials
{
    public class MouseInput : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            FollowMouse();
        }

        void FollowMouse()
        {
            Ray ray;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f, LayerMask.GetMask("FirePlan")))
            {
                transform.position = hit.point;
            }
        }
    }
}