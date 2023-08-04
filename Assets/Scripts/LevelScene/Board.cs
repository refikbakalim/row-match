using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Array = System.Array;
using DG.Tweening;

public sealed class Board : MonoBehaviour
{
	public static Board Instance { get; private set; }

	public int Width => StaticData.grid_width;
	public int Height => StaticData.grid_height;

	private int moveCount = StaticData.move_count;
	private int levelNumber = StaticData.level_number;
	private int highScore = StaticData.highscore;

	public Row[] rows { get; private set; } = new Row[StaticData.grid_height];
	public Tile[,] tiles { get; private set; } = new Tile[StaticData.grid_width, StaticData.grid_height];

	private readonly string[] grid = StaticData.grid.Split(',');

	private readonly List<Tile> selectedTiles = new List<Tile>();

	private const float TweenDuration = 0.2f;

	public GameObject rowPrefab;
	public GameObject tilePrefab;
	public Sprite starImage;

	public GameObject pauseButton;
	public GameObject completePopup;
	[SerializeField] private TMPro.TextMeshProUGUI completeScoreText;

	[SerializeField] private TMPro.TextMeshProUGUI highestScoreText;

	private bool ongoingSwap = false;

	private void Awake() => Instance = this;

	private void Start()
	{
		// Instantiate row prefabs to board, and tile prefabs to rows
		for (int x = 0; x < Height; x++)
		{
			var rowGameObject = Instantiate(rowPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			rowGameObject.GetComponent<RectTransform>().SetParent(gameObject.transform, false);
			Row row = rowGameObject.GetComponent<Row>();
			Tile[] rowTiles = new Tile[Width];
			for (int y = 0; y < Width; y++)
			{
				var tileGameObject = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
				tileGameObject.GetComponent<RectTransform>().SetParent(rowGameObject.transform, false);
				Tile tile = tileGameObject.GetComponent<Tile>();
				rowTiles[y] = tile;
			}
			row.tiles = rowTiles;
			rows[x] = row;
		}

		// Set tile properties, get tile type from grid string
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				var tile = rows[y].tiles[x];
				tile.x = x;
				tile.y = y;

				switch (grid[y * Width + x])
				{
					case "b":
						tile.Item = ItemDatabase.Items[0];
						tile.type = "blue";
						break;
					case "g":
						tile.Item = ItemDatabase.Items[1];
						tile.type = "green";
						break;
					case "r":
						tile.Item = ItemDatabase.Items[2];
						tile.type = "red";
						break;
					case "y":
						tile.Item = ItemDatabase.Items[3];
						tile.type = "yellow";
						break;
					default:
						tile.Item = ItemDatabase.Items[0];
						tile.type = "blue";
						break;
				}
				tiles[x, y] = tile;
			}
		}

		// Get highscore text from StaticData which took it from PlayerPrefs
		highestScoreText.text = highScore.ToString();
	}

	public async Task Swap(Tile tile1, Tile tile2)
	{

		if (ongoingSwap) return; // If there is already a swap happening, don't do anything
		ongoingSwap = true; // Set ongoingSwap to true to prevent other swaps from happening

		// Assign tile properties to temp variables
		var icon1 = tile1.icon;
		var icon2 = tile2.icon;
		var type1 = tile1.type;
		var type2 = tile2.type;
		var icon1Transform = icon1.transform;
		var icon2Transform = icon2.transform;

		// Switch tile icon positions with Dotween
		var sequence = DOTween.Sequence();
		sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration));
		sequence.Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));
		await sequence.Play().AsyncWaitForCompletion();

		// Switch tile properties
		icon1Transform.SetParent(tile2.transform);
		icon2Transform.SetParent(tile1.transform);
		tile1.icon = icon2;
		tile2.icon = icon1;
		var temp = tile1.Item;
		tile1.Item = tile2.Item;
		tile2.Item = temp;
		tile1.type = type2;
		tile2.type = type1;

		// Swap happened, reduce move count on variables and text
		moveCount -= 1;
		MoveCounter.Instance.Move = moveCount;

		CanCompleteRow(tile1, tile2); // Check if swap completed a row

		// If move count is 0 after swap, end game
		if (moveCount == 0)
		{
			EndGame();
			return;
		}

		ongoingSwap = false; // Unlock swap so other swaps can happen
	}

	private void EndGame()
	{
		pauseButton.GetComponent<Button>().interactable = false; // Disable pause button
		if (ScoreCounter.Instance.Score > highScore) // Check if endscore is a new highscore
		{
			// If it is, save it to PlayerPrefs and StaticData
			PlayerPrefs.SetInt("Level" + levelNumber.ToString() + "Highscore", ScoreCounter.Instance.Score);
			PlayerPrefs.Save();
			StaticData.hasNewHighscore = true;
			StaticData.highscore = ScoreCounter.Instance.Score;
		}

		// Set score text on complete popup, set popup's scale to 0 and enable it
		completeScoreText.SetText(ScoreCounter.Instance.Score.ToString());
		completePopup.transform.GetChild(0).GetChild(1).transform.localScale = new Vector3(0f, 0f, 0f);
		completePopup.SetActive(true);

		EndGameAnimation();
	}

	private async void EndGameAnimation()
	{
		// Create animation for complete popup -> Scales it up, waits, scales it down
		var popupSequence = DOTween.Sequence();
		popupSequence.Append(completePopup.transform.GetChild(0).GetChild(1).transform.DOScale(1f, 0.6f));
		popupSequence.AppendInterval(3.5f);
		popupSequence.Append(completePopup.transform.GetChild(0).GetChild(1).transform.DOScale(0f, 0.6f));
		await popupSequence.Play().AsyncWaitForCompletion();

		completePopup.SetActive(false); // After animation is completed, disable popup

		// Create animation for all tiles -> Scales them down, waits then returns to menu
		var tileSequence = DOTween.Sequence();
		float sequenceDuration = 0.3f;
		for (int i = Height - 1; i >= 0; i--)
		{
			for (int j = 0; j < Width; j++)
			{
				tiles[j, i].canMove = false;
				tileSequence.Append(tiles[j, i].transform.DOScale(0f, sequenceDuration));
				if (sequenceDuration > 0.05f)
				{
					sequenceDuration -= 0.01f; // Reduce duration after each tile so it gets faster
				}
			}
		}
		tileSequence.AppendInterval(0.5f);
		tileSequence.OnComplete(returnToMenu);
		await tileSequence.Play().AsyncWaitForCompletion();
	}

	public void returnToMenu()
	{
		StaticData.returningFromGame = true; // Set returningFromGame to true so MainScene knows to load LevelSelect
		SceneManager.LoadScene("MainScene");
	}


	private void CanCompleteRow(Tile tile1, Tile tile2)
	{
		bool isCompleted = false; // Check if swap completed a row

		if (tile1.y == tile2.y) return; //swap happened in the same row, means it can't complete a row

		//Check if first tile can complete a row
		if (tiles[tile1.x, tile1.y].GetRowConnectedTiles().Count == Width) // Check if first tile's row connected tiles count equal to board width
		{
			CompleteRow(tile1); // if it is equal, complete this row
			isCompleted = true;
		}

		//Check if second tile can complete a row
		if (tiles[tile2.x, tile2.y].GetRowConnectedTiles().Count == Width) // Check if second tile's row connected tiles count equal to board width
		{
			CompleteRow(tile2); // if it is equal, complete this row
			isCompleted = true;
		}

		// If a row is completed, there is a chance that the game is deadlocked
		// Check the deadlock if there is one, end game
		if (isCompleted)
		{
			if (isDeadlocked())
			{
				EndGame();
			}
		}
	}

	private async void CompleteRow(Tile tile)
	{
		var connectedTiles = tile.GetRowConnectedTiles(); // Get the tile's whole row
		
		ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count; // Add this row's score to score counter

		// Create animation for row completion
		// First deflate all tiles, then inflate them with star image
		var deflateSequence = DOTween.Sequence();
		var inflateSequence = DOTween.Sequence();
		foreach (var connectedTile in connectedTiles)
		{
			deflateSequence.Join(connectedTile.icon.transform.DOScale(0f, TweenDuration));
			inflateSequence.Join(connectedTile.icon.transform.DOScale(1f, TweenDuration));
			connectedTile.icon.sprite = starImage;
			connectedTile.canMove = false; // Set this so completed tiles can't be swapped
		}
		await deflateSequence.Play().AsyncWaitForCompletion();
		await inflateSequence.Play().AsyncWaitForCompletion();
	}

	private bool isDeadlocked()
	{

		List<List<int>> emptyNeighbours = new List<List<int>>(); // List for keeping neighbour relation of not completed rows
		List<int> emptyRows = new List<int>(); // y coordinates of rows which are not completed yet

		for (int i = 0; i < Height; i++) // Traverse through rows
		{
			if (tiles[0, i].canMove) // If a rows first tile can move, it means it is not completed
			{
				emptyRows.Add(i); // Add not completed rows to emptyRows
			}
			else // If a rows first tile can't move, we found a completed row
			{
				if (emptyRows.Count > 1) emptyNeighbours.Add(emptyRows); // add previous emptyRows to emptyNeighbours
				emptyRows = new List<int>(); // clear list to use it again
			}
		}

		// do it one last time for the last row
		// because if the last row is not completed, it won't be added to emptyRows in upper loop
		if (emptyRows.Count > 1) emptyNeighbours.Add(emptyRows);

		Dictionary<string, int> tileTypeCount = new Dictionary<string, int>(); // Dictionary for keeping tile type counts in neighbour rows

		for (int i = 0; i < emptyNeighbours.Count; i++) // For all neighbour relation
		{
			// Reset data
			tileTypeCount["blue"] = 0;
			tileTypeCount["green"] = 0;
			tileTypeCount["red"] = 0;
			tileTypeCount["yellow"] = 0;

			for (int j = 0; j < emptyNeighbours[i].Count; j++) // Get not completed rows
			{
				for (int k = 0; k < Width; k++) // Traverse their tiles
				{
					Tile tile = tiles[k, emptyNeighbours[i][j]];
					tileTypeCount[tile.type] += 1; // Add tile type to dictionary
				}
			}

			// If there is a tile type count equal or higher to board width, it means there is no deadlock
			if (tileTypeCount["blue"] >= Width || tileTypeCount["green"] >= Width || tileTypeCount["red"] >= Width || tileTypeCount["yellow"] >= Width)
			{
				return false;
			}
		}
		return true; // Has deadlock
	}
}
