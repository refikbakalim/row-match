using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Database that reads all items(Items created with Item.cs) from the Resources folder and stores them in a static array
public static class ItemDatabase
{
	public static Item[] Items { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] private static void Initialize() => Items = Resources.LoadAll<Item>("Items/");
}
