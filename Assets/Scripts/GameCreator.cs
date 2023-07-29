using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreator : MonoBehaviour
{
    public int level_number;
	public int grid_width;
	public int grid_height;
	public int move_count;
	public string grid;
	public int highscore;

    void Start()
    {
		level_number = StaticData.level_number;
		grid_width = StaticData.grid_width;
		grid_height = StaticData.grid_height;
		move_count = StaticData.move_count;
		grid = StaticData.grid;
		highscore = StaticData.highscore;
    }
}
