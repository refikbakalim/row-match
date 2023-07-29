using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
	public GameObject LevelsPopup;

	public void openLevelsPopup()
	{
		if(!LevelsPopup)
		{
			return;
		}
		LevelsPopup.SetActive(true);
	}

	public void closeLevelsPopup()
	{
		if(!LevelsPopup)
		{
			return;
		}
		LevelsPopup.SetActive(false);
	}
}
