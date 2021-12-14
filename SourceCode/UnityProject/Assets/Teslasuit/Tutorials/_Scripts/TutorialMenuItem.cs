using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TeslasuitAPI.Tutorials
{
    public class TutorialMenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Text title;
        public Image bg;


        public event Action Clicked;

        bool isMouseOver = false;


        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("clicked");
            if (Clicked != null)
                Clicked();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("on enter");
            bg.color = bg.color * 0.9f;
            isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("on exit");
            bg.color = bg.color / 0.9f;
            isMouseOver = false;
        }

        public void SetTitle(string titleString)
        {
            title.text = titleString;
        }

        public void SetSelected(bool selected)
        {
            //Debug.Log("SELECTED");
            if (selected)
            {
                bg.color = (isMouseOver)? Color.gray * 0.9f : Color.gray;
                
            }else { 
                bg.color = (isMouseOver) ? Color.white * 0.9f : Color.white;
            }
        }
        
    }
}