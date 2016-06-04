using UnityEngine;
using System.Collections;

public class World {

	Tile[,] tiles;
	int width;
	int height;

	public World(int width = 100, int height = 100){
		this.width = width;
		this.height = height;

		tiles = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile (this, x, y);	
			}
		}
		Debug.Log ("World cree avec " + (width * height) + " tiles.");
	}

	public Tile GetTileAt(int x, int y)  {
		if( x > width || x < 0 || y > height || y < 0 ) {
			Debug.LogError("tile ("+x+" , "+y+") is out of raaange.");
				return null;
				}
		return tiles[x, y];
	}

}
