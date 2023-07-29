using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
	public int level_number;
	public int grid_width;
	public int grid_height;
	public int move_count;
	public string grid;
	public int highscore;
	[SerializeField] private SelectedLevelData data;
	
	public void OpenScene() {
		Debug.Log("Opening level " + level_number);
		data.level_number = level_number;
		data.grid_width = grid_width;
		data.grid_height = grid_height;
		data.move_count = move_count;
		data.grid = grid;
		data.highscore = highscore;
		SceneManager.LoadScene("GameScene");
	}
}
