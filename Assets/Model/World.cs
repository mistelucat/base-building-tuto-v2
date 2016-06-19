﻿using UnityEngine;
using System.Collections.Generic;
using System;

//world est le méta script des autres script, c'est le king BB !

public class World {

	Tile[,] tiles;

	Dictionary<string, Furniture> furniturePrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }


	//on se fait un callback (cb) des objets créés
	Action<Furniture> cbFurnitureCreated;
	Action<Tile> cbTileChanged;

	//c'est un data type, like a simple array
	//TODO most likely this will be replaced with a dedicated class for managing job queues (plural!)
	//that might also be semi static or self initializing or some damn thing
	//for now this is just a public member of world
	public JobQueue jobQueue;

	public World(int width = 100, int height = 100){
		jobQueue = new JobQueue();
		Width = width;
		Height = height;

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x, y] = new Tile (this, x, y);	
				//hey we want to know if you have changed
				tiles[x, y].RegisterTileTypeChangedCallback( OnTileChanged );
			}
		}

		Debug.Log ("World cree avec " + (width * height) + " tiles.");

		CreateInstalledObjectPrototypes ();
	}

	void CreateInstalledObjectPrototypes() {
		furniturePrototypes = new Dictionary<string, Furniture>();

		furniturePrototypes.Add("Wall", 
			Furniture.CreatePrototype(
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

	public void PlaceFurniture(string objectType, Tile t) {
		Debug.Log("PlaceInstalledObject");
		//TODO this function assumes 1x1 tiles only and no rotation


		if (furniturePrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("installedObjectPrototypes does'nt contain a proto for key:" + objectType);
			return;
		}

		Furniture obj = Furniture.PlaceInstance (furniturePrototypes [objectType], t);

		if (obj == null) {
			//failed to place object -- most likely there was already someting there.
			return;
		}

		if (cbFurnitureCreated != null) {
			cbFurnitureCreated (obj);
		}
	}
	public void RegisterFurnitureCreated (Action<Furniture> callbackfunc) {
		cbFurnitureCreated += callbackfunc;
	}

	public void UnregisterFurnitureCreated (Action<Furniture> callbackfunc) {
		cbFurnitureCreated -= callbackfunc;
	}

	public void RegisterTileChanged (Action<Tile> callbackfunc) {
		cbTileChanged += callbackfunc;
	}

	public void UnregisterTileChanged (Action<Tile> callbackfunc) {
		cbTileChanged -= callbackfunc;
	}

	void OnTileChanged(Tile t) {
		if (cbTileChanged == null)
			return;
		
		cbTileChanged (t);
	}

	public bool IsFurniturePlacementValid(string furnitureType, Tile t) {
		return furniturePrototypes[furnitureType].IsValidPosition(t);
	}

	public Furniture GetFurniturePrototype(string objectType) {
		if (furniturePrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("No furniture with type : " + objectType);
			return null;
		}

		return furniturePrototypes [objectType];
	}
}
