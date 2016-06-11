using UnityEngine;
using System.Collections.Generic;

public class World {

	Tile[,] tiles;

	Dictionary<string, InstalledObject> installedObjectPrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }

	public World(int width = 100, int height = 100){
		Width = width;
		Height = height;

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile (this, x, y);	
			}
		}

		Debug.Log ("World cree avec " + (width * height) + " tiles.");

		CreateInstalledObjectPrototypes ();
	}

	void CreateInstalledObjectPrototypes() {
		installedObjectPrototypes = new Dictionary<string, InstalledObject>();

		installedObjectPrototypes.Add("Wall", 
			InstalledObject.CreatePrototype(
			"Wall",
			0,
			1,
			1
		)
		);

	}

	//a fuction for thesting our the system
	public void RandomizeTiles() {
		Debug.Log ("RandomizeTiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {

				if (Random.Range (0, 2) == 0) {
					tiles[x, y].Type = TileType.Empty;
				} else {
					tiles[x, y].Type = TileType.Floor;	
				}
			}
		}	


	}


	public Tile GetTileAt(int x, int y)  {
		if( x > Width || x < 0 || y > Height || y < 0 ) {
			Debug.LogError("tile ("+x+" , "+y+") is out of raaange.");
				return null;
				}
		return tiles[x, y];
	}

}
