using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;

	Dictionary<string, InstalledObject> installedObjectPrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }


	//on se fait un callback (cb) des objets créés
	Action<InstalledObject> cbInstalledObjectCreated;



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
			1,
			true //Links to neighbours and sort of become "part of" a larger object
		)
		);

	}

	//a fuction for thesting our the system
	public void RandomizeTiles() {
		Debug.Log ("RandomizeTiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {

				if (UnityEngine.Random.Range (0, 2) == 0) {
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


	//on créé la fonction pour mettre des installedobjects !

	public void PlaceInstalledObject(string objectType, Tile t) {
		Debug.Log("PlaceInstalledObject");
		//TODO this function assumes 1x1 tiles only and no rotation


		if (installedObjectPrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("installedObjectPrototypes does'nt contain a proto for key:" + objectType);
			return;
		}

		InstalledObject obj = InstalledObject.PlaceInstance (installedObjectPrototypes [objectType], t);

		if (obj == null) {
			//failed to place object -- most likely there was already someting there.
			return;
		}

		if (cbInstalledObjectCreated != null) {
			cbInstalledObjectCreated (obj);
		}
	}
	public void RegisterInstalledObjectCreated (Action<InstalledObject> callbackfunc) {
		cbInstalledObjectCreated += callbackfunc;
	}

	public void UnregisterInstalledObjectCreated (Action<InstalledObject> callbackfunc) {
		cbInstalledObjectCreated -= callbackfunc;
	}
}
