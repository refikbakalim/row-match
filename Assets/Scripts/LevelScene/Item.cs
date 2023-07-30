using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for Tile icons and point values
// We created 4 items with this scriptable object
// Assigned the sprites and values in the inspector to use items later
[CreateAssetMenu(fileName = "Item", menuName = "Row-Match/Item")]
public sealed class Item : ScriptableObject
{
	public int value;
	public Sprite sprite;
}
