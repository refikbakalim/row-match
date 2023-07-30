using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public sealed class Tile : MonoBehaviour
{
	public int x;
	public int y;

	public string type;

	private Item item;
	public Item Item
	{
		get => item;
		set
		{
			if (item == value)
				return;
			item = value;
			icon.sprite = item.sprite;
		}
	}

	public Image icon;

	public Tile Left => x > 0 ? Board.Instance.tiles[x - 1, y] : null;
	public Tile Right => x < Board.Instance.Width - 1 ? Board.Instance.tiles[x + 1, y] : null;
	public Tile Up => y < Board.Instance.Height - 1 ? Board.Instance.tiles[x, y + 1] : null;
	public Tile Down => y > 0 ? Board.Instance.tiles[x, y - 1] : null;

	public Tile[] swapNeighbours => new Tile[] { Left, Right, Up, Down };
	public Tile[] rowNeighbours => new Tile[] { Left, Right };

	public static Tile firstTile;

	public static bool canSwap = false;
	public bool canMove = true;

	// Returns a list of all connected tiles in a row
	public List<Tile> GetRowConnectedTiles(List<Tile> exclude = null)
	{
		var result = new List<Tile> { this, };

		// Exclude this tile from the list so it won't be checked again
		if (exclude == null) exclude = new List<Tile> { this, };
		else exclude.Add(this);


		foreach (var neighbour in rowNeighbours)
		{
			if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) //
				continue;

			result.AddRange(neighbour.GetRowConnectedTiles(exclude)); // Add all connected tiles to the list recursively
		}
		return result;
	}


	//////////// Swipe Controller with mouse events ////////////

	// When pressed down on a tile, it will be stored in firstTile to swap
	// And this tile's canSwap will be set to true
	public void OnMouseDown()
	{
		if (!canMove) return; // If this tile is not already completed
		firstTile = this;
		canSwap = true;
	}

	// When mouse is released, swap already happened or there is no swap
	// so set canswap to false and reset tiles
	public void OnMouseUp()
	{
		canSwap = false;
		firstTile = null;
	}

	// When mouse enters a tile, check if first tile is already set
	// If first tile is set, this means that mouse is swiped between two tiles
	// If both tiles can swap and move, swap tiles
	public async void OnMouseEnter()
	{
		if (!canSwap || !canMove) return;
		if (firstTile == this || firstTile == null) return;

		canSwap = false; // Set canSwap to false to prevent multiple swaps
		if (!swapNeighbours.Contains(firstTile)) return; // If tiles are not neighbours don't swap
		await Board.Instance.Swap(firstTile, this);
	}
}