using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

// Class to instantiate level prefabs on main scene and set their data
// level prefabs are the ones that are shown on the levels popup, not the ones on the game scene

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
	private int initialLevelCount;
	private int levelCount;
	private const int maxLevelCount = 25;
	private string levelsPath = "Assets/Resources/Levels";

	private bool isLevelUnlocked = false;
	public Sprite lockedSprite;

	public static int[] highscores;

	void Start()
	{
		// count the txt files in Resources/Level
		// first 10 offline levels stored in Resources folder
		initialLevelCount = Directory.EnumerateFiles(levelsPath, "RM_*.txt").Count();

		// count the files in persistentPath
		// downloaded levels stored in persistentDataPath
		initialLevelCount += Directory.EnumerateFiles(Application.persistentDataPath, "RM_*").Count();

		// If there are less than 25 levels, download levels from urls
		if (initialLevelCount < maxLevelCount) StartCoroutine(downloadLevels());

		highscores = new int[maxLevelCount];

		readFiles(); // Read all levels from Resources folder and persistentDataPath
	}

	private IEnumerator downloadLevels()
	{

		// level names and urls are stored in Resources/Levels/LevelUrls.txt
		TextAsset txt = Resources.Load<TextAsset>("Levels/LevelUrls");
		List<string> lines = new List<string>(txt.text.Split('\n'));

		foreach (string line in lines)
		{ // each line is a level name and url seperated by comma
			string[] SplitLine = line.Split(',');
			string levelTitle = SplitLine[0];
			string levelUrl = SplitLine[1];

			// if level already exists in persistentDataPath, skip it
			if (File.Exists(Application.persistentDataPath + "/" + levelTitle)) continue;

			// download level from url and save it to persistentDataPath
			using (UnityWebRequest www = UnityWebRequest.Get(levelUrl))
			{
				yield return www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.Log(www.error);
				}
				else
				{
					string savePath = string.Format("{0}/{1}", Application.persistentDataPath, levelTitle);
					System.IO.File.WriteAllText(savePath, www.downloadHandler.text);
				}
			}
		}
	}


	private void readFiles()
	{
		List<string[]> levelLines = new List<string[]>(); // list for storing level data lines

		// Read first 10 offline levels from resources folder
		for (int i = 1; i < 11; i++)
		{
			TextAsset txt = Resources.Load<TextAsset>("Levels/RM_A" + i.ToString());
			string[] lines = txt.text.Split('\n');
			levelLines.Add(lines);

			levelCount++; // count read levels
		}

		// Read downloaded levels from persistentDataPathe
		DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
		// PadLeft is used to sort files by its number as otherwise it sorts like 1, 10, 11, 2 instead of 1, 2, 10, 11
		FileInfo[] files = di.GetFiles("RM*").OrderBy(file => Regex.Replace(file.Name, @"\d+", match => match.Value.PadLeft(4, '0'))).ToArray();

		foreach (FileInfo file in files)
		{
			string[] lines = File.ReadAllLines(file.FullName);
			levelLines.Add(lines);

			levelCount++; // count read levels
		}

		createLevels(levelLines); // send level data lines to create levels
	}

	private void createLevels(List<string[]> levelLines)
	{

		// set scroll content height to (levelCount * levelPrefabHeight + 10) to fit all levels
		// +10 is for spacing between levels, it is set on vertical layout group spacing
		levelPrefabHeight = levelPrefab.GetComponent<RectTransform>().rect.height;
		scrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, levelCount * (levelPrefabHeight + 10));

		for (int j = 0; j < levelLines.Count; j++)
		{
			string[] lines = levelLines[j];

			for (int i = 0; i < lines.Length; i++)
			{
				// split lines into key and value and store them in variables
				string[] SplitLine = lines[i].Split(':');
				string key = SplitLine[0];
				string value = SplitLine[1];
				switch (key)
				{
					case "level_number":
						level_number = int.Parse(value);
						break;
					case "grid_width":
						grid_width = int.Parse(value);
						break;
					case "grid_height":
						grid_height = int.Parse(value);
						break;
					case "move_count":
						move_count = int.Parse(value);
						break;
					case "grid":
						grid = value;
						break;
					default:
						break;
				}
			}

			// Get highscore from playerprefs and store it in highscores array
			highscore = PlayerPrefs.GetInt("Level" + level_number + "Highscore", 0);
			highscores[level_number - 1] = highscore;

			// Check if level is unlocked
			if (highscore > 0) isLevelUnlocked = true; // if a highscore set, it is already unlocked
			else if (level_number == 1) isLevelUnlocked = true; // if it is the first level, it is already unlocked
			else if (highscores[level_number - 2] > 0) isLevelUnlocked = true; // if previous level has highscore, it is unlocked
			else isLevelUnlocked = false;

			// Instantiate level prefab and set it's parent to scroll content
			levelObject = Instantiate(levelPrefab, new Vector3(0, scrollContent.GetComponent<RectTransform>().position.y + (-level_number * levelPrefabHeight), 0), Quaternion.identity);
			levelObject.GetComponent<RectTransform>().SetParent(scrollContent.transform, false);

			// Set level prefab's level number and move count text
			levelObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Level {level_number} - {move_count} moves";

			// Set level prefab's highscore text
			if (highscore > 0)
			{
				levelObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Highest Score: {highscore}";
			}
			else if (!isLevelUnlocked)
			{
				// If level is locked, change button color-text and make play button uninteractable
				levelObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Locked Level";
				var playButton = levelObject.transform.GetChild(0).GetComponent<Button>();
				playButton.interactable = false;
				playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Locked";
				levelObject.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
				levelObject.transform.GetChild(0).GetComponent<Image>().sprite = lockedSprite;
			}
			else
			{
				levelObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "No Score";
			}


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
