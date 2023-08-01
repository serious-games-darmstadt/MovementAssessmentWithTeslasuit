using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class disable_button : MonoBehaviour
{

		public Button button;
		
		void Start()
		{
			Button btn = button.GetComponent<Button>();
			btn.onClick.AddListener(TaskOnClick);
		

	}

	void TaskOnClick()
		{
			Debug.Log("You have clicked the button!");
			button.interactable = false;
		    StartCoroutine(waiter());
	}
	IEnumerator waiter()
	{

		//Wait for 4 seconds
		yield return new WaitForSecondsRealtime(4);
		button.interactable = true;

	}
}

