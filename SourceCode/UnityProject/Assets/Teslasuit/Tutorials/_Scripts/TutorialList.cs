using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TeslasuitAPI.Tutorials
{
    [CreateAssetMenu(fileName = "TutorialList", menuName = "Tutorial/TutorialList")]
    public class TutorialList : ScriptableObject
    {
        public TutorialElement[] elements;
    }
}
