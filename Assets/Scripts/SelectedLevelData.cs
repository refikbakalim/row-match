using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SelectedLevelData", menuName = "ScriptableObjects/SelectedLevelData")]
public class SelectedLevelData : ScriptableObject
{
   	public int level_number;
	public int grid_width;
	public int grid_height;
	public int move_count;
	public string grid;
	public int highscore;
}