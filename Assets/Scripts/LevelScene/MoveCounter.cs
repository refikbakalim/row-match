using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to count moves and display it on the screen
public class MoveCounter : MonoBehaviour
{
	public static MoveCounter Instance { get; private set; }

	private int move;
	public int Move
	{
		get => move;
		set
		{
			if (move == value) return;
			move = value;
			moveText.SetText(move.ToString());
		}
	}

	[SerializeField] private TMPro.TextMeshProUGUI moveText;

	private void Awake()
	{
		Instance = this;
		Move = StaticData.move_count; // Set move text and value to the value passed from StaticData on awake
	}
}
