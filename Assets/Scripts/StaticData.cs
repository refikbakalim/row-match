using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData 
{
	// Static variables to pass data between scenes
    public static int level_number;
	public static int grid_width;
	public static int grid_height;
	public static int move_count;
	public static string grid;
	public static int highscore;
	public static bool returningFromGame = false;
	public static bool hasNewHighscore = false;
}
