using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Class to count score and display it on the screen
public sealed class ScoreCounter : MonoBehaviour
{
	public static ScoreCounter Instance { get; private set; }

	private int score;
	public int Score
	{
		get => score;
		set
		{
			if(score == value) return;
			score = value;
			scoreText.SetText(score.ToString());
		}
	}

	[SerializeField] private TMPro.TextMeshProUGUI scoreText;

	private void Awake() => Instance = this;
}
