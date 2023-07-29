using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;


public class LevelCreator : MonoBehaviour
{
	public int level_number;
	public int grid_width;
	public int grid_height;
	public int move_count;
	public string grid;
	public int highscore;
	public GameObject levelPrefab;
	public GameObject scrollContent;

	private GameObject levelObject;
	private float levelPrefabHeight;
	private int levelCount;
	private string levelsPath = "Assets/Resources/Levels";
	

    void Start()
    {	
		// count the json files in levelsPath
		levelCount = Directory.EnumerateFiles(levelsPath, "*.json").Count();

		// set scroll content height to levelCount * levelPrefabHeight
		levelPrefabHeight = levelPrefab.GetComponent<RectTransform>().rect.height;
		scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, levelCount * levelPrefabHeight);
		
		for(int i = 1; i < levelCount + 1 ; i++){

			// Load levels from json files to this object
			var jsonTextFile = Resources.Load<TextAsset>("Levels/RM_A" + i.ToString());
			if(jsonTextFile == null) continue;
			JsonUtility.FromJsonOverwrite(jsonTextFile.text, this);

			// Instantiate level prefab and set it's parent to scroll content
			levelObject = Instantiate(levelPrefab, new Vector3(0, scrollContent.GetComponent<RectTransform>().position.y + (-i * levelPrefabHeight) - 40, 0), Quaternion.identity);
			levelObject.GetComponent<RectTransform>().SetParent(scrollContent.transform, false);

			// Set level prefab's level number and move count text
			levelObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Level {level_number} - {move_count} moves";

			// Set level prefab's highscore text
			levelObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Highest Score: {highscore}";

			// Set level prefab's script data
			levelObject.transform.GetComponent<LevelSelector>().level_number = level_number;
			levelObject.transform.GetComponent<LevelSelector>().grid_width = grid_width;
			levelObject.transform.GetComponent<LevelSelector>().grid_height = grid_height;
			levelObject.transform.GetComponent<LevelSelector>().move_count = move_count;
			levelObject.transform.GetComponent<LevelSelector>().grid = grid;
			levelObject.transform.GetComponent<LevelSelector>().highscore = highscore;
		}
    }
}
