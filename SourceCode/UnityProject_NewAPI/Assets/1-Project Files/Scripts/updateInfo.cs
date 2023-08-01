using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class updateInfo : MonoBehaviour
{

    [SerializeField]
    public Text title;


    public void CreateSubject()
    {
        title.enabled = true;
        title.text = "Subject created.";
    }

        public void saveJson()
        {
            title.enabled = true;
            title.text = "JSON saved.";
        }

        public void disableButton()
    {
        title.enabled = false;
    }

    }

