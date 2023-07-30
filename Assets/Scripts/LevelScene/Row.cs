using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A row is a collection of tiles with a size of grid width
public sealed class Row : MonoBehaviour
{
	public Tile[] tiles = new Tile[StaticData.grid_width];
}
