using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using TeslasuitAPI;
using UnityEngine;

namespace DefaultNamespace
{
    public class MotionFeedback : MonoBehaviour
    {
        private MocapJoints _mocapJoints;

        public GameObject HitMarkerRedPrefab;
        public GameObject HitMarkerBluePrefab;

        public HapticMaterialAsset HapticMaterial;

        private Transform _jointPositionReferenceFrame;

        private Dictionary<String, Vector3> motionError = new Dictionary<string, Vector3>();

        public Dictionary<string, Vector3> MotionError
        {
            get => motionError;
            set
            {
                motionError = value;
                _newErrorAvailable = true;
            }
        }

        private bool _newErrorAvailable = false;

        private void Start()
        {
            _mocapJoints = MocapJoints.GetInstance();
            _jointPositionReferenceFrame = GameObject.Find("ReferenceFrame").transform;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!Config.FEEDBACK_ENABLED || !_newErrorAvailable)
                return;

            foreach (var keyValuePair in motionError)
            {
                Transform jointTransform = _mocapJoints.GetJoint(keyValuePair.Key);
                Vector3 errorDirection = keyValuePair.Value;
                Vector3 worldSpaceError = _jointPositionReferenceFrame.TransformDirection(errorDirection);

                Vector3 feedbackOrigin = jointTransform.position + 10 * worldSpaceError;
                Debug.DrawLine(jointTransform.position, feedbackOrigin, _mocapJoints.JointColorMap[keyValuePair.Key], 0.02f);
                float rayLength = Vector3.Distance(feedbackOrigin, jointTransform.position);

                Ray ray = new Ray(feedbackOrigin, -worldSpaceError);
                HapticRaycastHit[] hits = new HapticRaycastHit[5];
                if (HapticHitRaycaster.Raycast(ray, hits, rayLength) > 0)
                {
                    HapticRaycastHit farthestHit = new HapticRaycastHit();
                    float farthestHitDistance = -1;
                    foreach (var hit in hits)
                    {
                        if (hit.channelPoly.count > 0)
                        {
                            Transform hitBone = hit.raycastHit.collider.gameObject.transform.parent;

                            if (hitBone == jointTransform || hitBone == jointTransform.parent)
                            {
                                // GameObject hitmarker = Instantiate(HitMarkerRedPrefab, hit.raycastHit.point, Quaternion.identity,
                                //     MocapJoints.GetInstance().GetJoint(keyValuePair.Key));
                                // Destroy(hitmarker, 0.02f);
                            
                                float distance = Vector3.Distance(jointTransform.position, hit.raycastHit.point);
                                if ( distance > farthestHitDistance)
                                {
                                    farthestHitDistance = distance;
                                    farthestHit = hit;
                                }
                            }
                        }
                    }

                    if (farthestHitDistance > 0)
                    {
                        HapticReceiver receiver = farthestHit.hapticReceiver;
                        var poly = farthestHit.channelPoly;
                        HapticHitInfo hapticHitInfo = GetHapticHitInfo(worldSpaceError);
                        HapticPolyHit polyHit = new HapticPolyHit(poly, hapticHitInfo);
                        // receiver.PolyHit(polyHit);

                        GameObject hitmarker2 = Instantiate(HitMarkerBluePrefab, farthestHit.raycastHit.point, Quaternion.identity,
                            MocapJoints.GetInstance().GetJoint(keyValuePair.Key));
                        Destroy(hitmarker2, 0.02f);
                    }
                }
            }
            _newErrorAvailable = false;
        }

        HapticHitInfo GetHapticHitInfo(Vector3 error)
        {
            HapticHitInfo hitInfo = new HapticHitInfo();

            hitInfo.material = HapticMaterial; // IHapticMaterial
            hitInfo.hitEvent = HapticHitEvent.HitEnter; // Type of Hit Event
            hitInfo.duration_ms = 20; // Duration in milliseconds

            float force = Math.Min(1, error.magnitude / Config.FEEDBACK_THRESHOLD);
            hitInfo.impact = force; // Force in range 0.0 to 1.0

            return hitInfo;
        }
    }
}