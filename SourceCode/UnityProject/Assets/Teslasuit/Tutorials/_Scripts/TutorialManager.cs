using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeslasuitAPI.Tutorials
{
    public class TutorialManager : MonoBehaviour
    {
        public TutorialMenu menu;
        public TutorialList list;
        public Transform PopupHolder;

        PopupMessage currentMessage;

        // Use this for initialization
        void Start()
        {
            menu.SetList(list);
            menu.ElementSelected +=
                (i) =>
                    {
                    //Debug.Log("Element selected : " + i);
                    if (currentMessage != null)
                            currentMessage.Close();
                        Show(i);
                    };

            //Show(0);
        }

        void Show(int i)
        {
            if (i > list.elements.Length - 1 || i < 0)
            {
                throw new System.Exception("Argument exception");
            }

            var popup = Instantiate(list.elements[i].popupPrefab, PopupHolder);
            currentMessage = popup;

            if (i == 0)
            {
                popup.prevButton.interactable = false;
            }
            else
            {
                popup.Prev += () => { popup.Close(); Show(i - 1); };
            }

            if (i == list.elements.Length - 1)
            {
                popup.nextButton.interactable = false;
            }
            else
            {
                popup.Next += () => { popup.Close(); Show(i + 1); };
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}