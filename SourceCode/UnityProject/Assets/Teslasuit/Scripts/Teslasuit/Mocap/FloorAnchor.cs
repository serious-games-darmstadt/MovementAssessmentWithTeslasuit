using UnityEngine;

namespace TeslasuitAPI
{
    public class FloorAnchor : MonoBehaviour
    {

        public Transform rightFoot;
        public Transform leftFoot;

        public Transform floor;


        private float floorOffsetLeft;
        private float floorOffsetRight;

        float left;
        float right;

        // Use this for initialization 
        void Awake()
        {
            floorOffsetRight = GetHeightOffset(rightFoot, floor);
            floorOffsetLeft = GetHeightOffset(leftFoot, floor);
        }

        // Update is called once per frame 
        void Update()
        {
            left = GetHeightOffset(leftFoot, floor) - floorOffsetLeft;
            right = GetHeightOffset(rightFoot, floor) - floorOffsetRight;

            float offset = left < right ? left : right;

            Vector3 offs = new Vector3(0f, -offset, 0f);

            transform.position += offs;
        }

        private float GetHeightOffset(Transform a, Transform f)
        {
            return (a.transform.position - f.transform.position).y;
        }
    } 
}