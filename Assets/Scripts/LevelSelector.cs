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
	
	public void OpenScene() {
		StaticData.level_number = level_number;
		StaticData.grid_width = grid_width;
		StaticData.grid_height = grid_height;
		StaticData.move_count = move_count;
		StaticData.grid = grid;
		StaticData.highscore = highscore;
		SceneManager.LoadScene("GameScene");
	}
}
