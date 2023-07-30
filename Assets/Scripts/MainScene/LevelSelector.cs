using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelSelector : MonoBehaviour
{
	public int level_number;
	public int grid_width;
	public int grid_height;
	public int move_count;
	public string grid;
	public int highscore;

	private const float TweenDuration = 0.1f;

	// set data on StaticData to pass between scenes then change the scene
	public void OpenScene()
	{
		StaticData.level_number = level_number;
		StaticData.grid_width = grid_width;
		StaticData.grid_height = grid_height;
		StaticData.move_count = move_count;
		StaticData.grid = grid;
		StaticData.highscore = highscore;

		SceneManager.LoadScene("LevelScene");
	}

	public async void onPlayButtonClicked()
	{
		// Play the play button animation
		var Sequence = DOTween.Sequence();
		Sequence.Append(this.transform.GetChild(0).gameObject.transform.DOScale(new Vector3(0.95f, 0.95f, 0.95f), TweenDuration));
		Sequence.Append(this.transform.GetChild(0).gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), TweenDuration));
		await Sequence.Play().AsyncWaitForCompletion();

		OpenScene();
	}
}
