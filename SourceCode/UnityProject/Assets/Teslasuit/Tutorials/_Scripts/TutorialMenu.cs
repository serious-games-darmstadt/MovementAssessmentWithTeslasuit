using System;

using UnityEngine;

namespace TeslasuitAPI.Tutorials
{
    public class TutorialMenu : MonoBehaviour
    {
        public TutorialMenuItem item;
        public Transform Holder;
        TutorialList list;
        public event Action<int> ElementSelected;

        TutorialMenuItem currentItem;

        public void SetList(TutorialList list)
        {
            this.list = list;
            Init();
        }

        void Init()
        {
            for (int i = 0; i < list.elements.Length; i++)
            {
                var newItem = Instantiate(item, Holder);
                var index = i;
                newItem.Clicked += () =>
                {
                    ElementSelected?.Invoke(index);
                    if(currentItem != null)
                    {
                        currentItem.SetSelected(false);
                    }
                    currentItem = newItem;
                    currentItem.SetSelected(true);
                };

                newItem.SetTitle(list.elements[i].title);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}