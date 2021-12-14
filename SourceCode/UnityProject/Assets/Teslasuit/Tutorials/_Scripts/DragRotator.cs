using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace TeslasuitAPI.Tutorials
{
    [System.Serializable]
    public class RotateEvent : UnityEvent<float> { }

    public class DragRotator : MonoBehaviour, IDragHandler
    {
        public float Sensetivity = 1.0f;
        public RotateEvent onRotate;

        public void OnDrag(PointerEventData eventData)
        {
            onRotate.Invoke(-eventData.delta.x * Sensetivity);
        }
    }
}