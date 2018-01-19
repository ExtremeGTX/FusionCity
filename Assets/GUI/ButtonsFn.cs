using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFn : MonoBehaviour {

	bool pause=false;
	// Use this for initialization
	public void ExitGame()
	{
		Application.Quit();
	}

	public void Update()
	{
		if(Input.GetKeyDown("p"))
		{
			pause = !pause;
			if (pause)
			{
				Time.timeScale=0;
			}
			else
			{
				Time.timeScale=1;
			}
		}
	}
}
