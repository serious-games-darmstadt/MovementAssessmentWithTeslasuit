using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TeslasuitAPI;


namespace TeslasuitAPI.Tutorials
{
    public class HapticSample : MonoBehaviour
    {
        public GraphicRaycaster graphicRaycaster;
        public EventSystem m_EventSystem;
        public HapticMaterialAsset[] materialAssets;

        public Dropdown dropdown;

        float duration;
        float force;
        int currentMaterialIndex = 0;
        PointerEventData m_PointerEventData;

        [HideInInspector]
        private new Camera camera;

        void Start()
        {
            Logger.Enabled = true;
            //Logger.AutoWrite = false;
            //Logger.stacktrace_level = Logger.STACKTRACE_LEVEL.API_ONLY;
            Logger.OnLogMessage += Logger_OnLogMessage;
            Teslasuit.SDKError += (sender, code) =>
            {
                Debug.Log(string.Format("SDK error : {0} sent {1}", sender, code));
            };

            Teslasuit.PluginError += (o, ex) =>
            {
                Debug.Log(string.Format("Plugin error : {0} message: {1}", o.ToString(), ex.Message));
            };

            camera = GetComponent<Camera>();
            dropdown.ClearOptions();
            dropdown.options = CreateOptionListFromMaterials();
        }

        private void Logger_OnLogMessage(string obj)
        {
            Debug.Log("log : " + obj);
        }

        List<Dropdown.OptionData> CreateOptionListFromMaterials()
        {
            return materialAssets.Select(t => new Dropdown.OptionData(t.name)).ToList();
        }

        bool isBlockedByUI()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            graphicRaycaster.Raycast(m_PointerEventData, results);
            if (results.Count != 0)
                return true;
            return false;
        }

        HapticHitInfo GetHapticHitInfo()
        {
            HapticHitInfo hitInfo = new HapticHitInfo();

            hitInfo.material = materialAssets[currentMaterialIndex]; // IHapticMaterial
            hitInfo.hitEvent = HapticHitEvent.HitEnter;               // Type of Hit Event
            hitInfo.duration_ms = (uint)duration;                    // Duration in milliseconds
            hitInfo.impact = force;                                  // Force in range 0.0 to 1.0

            return hitInfo;
        }

        public void SetMaterial(int index)
        {
            if (index < 0 || index > materialAssets.Length - 1)
                return;

            currentMaterialIndex = index;
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        public void SetForce(float force)
        {
            this.force = force;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isBlockedByUI())
                    return;

                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                HapticRaycastHit hit;
                if (HapticHitRaycaster.Raycast(ray, out hit))
                {
                    HapticReceiver receiver = hit.hapticReceiver;
                    var poly = hit.channelPoly;
                    HapticHitInfo hapticHitInfo = GetHapticHitInfo();

                    HapticPolyHit polyHit = new HapticPolyHit(poly, hapticHitInfo);
                    receiver.PolyHit(polyHit);
                }
            }
        }
    }
}