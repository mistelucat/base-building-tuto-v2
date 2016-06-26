using UnityEngine;
using System.Collections.Generic;
using System;

//world est le méta script des autres script, c'est le king BB !

public class World   {

	Tile[,] tiles;

	//on créé une liste d  charaters de la classe character !!!
	List<Character> characters;

	//The pathfinding graph uised to navigate our world map
	public Path_TileGraph tileGraph;

	Dictionary<string, Furniture> furniturePrototypes;

	public int Width { get; protected set; }
	public int Height { get; protected set; }


	//on se fait un callback (cb) des objets créés
	Action<Furniture> cbFurnitureCreated;
	Action<Character> cbCharacterCreated;
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

		tiles = new Tile[Width, Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				tiles[x, y] = new Tile (this, x, y);	
				//hey we want to know if you have changed
				tiles[x, y].RegisterTileTypeChangedCallback( OnTileChanged );
			}
		}

		Debug.Log ("World cree avec " + (Width * Height) + " tiles.");

		CreateFurniturePrototypes ();

		//on instantie la liste de characters
		characters = new List<Character> ();
		}

	public void Update(float deltaTime) {
		foreach (Character c in characters) {
			c.Update (deltaTime);
		}
	}

	public Character CreatedCharacter(Tile t) {
		Character c = new Character (t);
		//on l'ajoute à la liste
		characters.Add (c);
		if(cbCharacterCreated != null)
			cbCharacterCreated (c);

		return c;
	}

	void CreateFurniturePrototypes() {
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



	public void SetupPathFindingExample() {
		Debug.Log ("SetuPathFindingExample");
		//make a set of floor walls to test pathfinding with
		int l=Width / 2 - 5;
		int b = Height / 2 - 5;

		for (int x = l - 5; x < l + 15; x++) {
			for (int y = b - 5; y < b + 15; y++) {
				tiles [x, y].Type = TileType.Floor;

				if (x == l || x == (l + 9) || y == b || y == (b + 9)) {
					if (x != (l + 9) && y != (b + 4)) {
						PlaceFurniture ("Wall", tiles [x, y]);
					}
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
			InvalidateTileGraph ();
		}
	}
	public void RegisterFurnitureCreated (Action<Furniture> callbackfunc) {
		cbFurnitureCreated += callbackfunc;
	}

	public void UnregisterFurnitureCreated (Action<Furniture> callbackfunc) {
		cbFurnitureCreated -= callbackfunc;
	}

	public void RegisterCharacterCreated (Action<Character> callbackfunc) {
		cbCharacterCreated += callbackfunc;
	}

	public void UnregisterCharacterCreated (Action<Character> callbackfunc) {
		cbCharacterCreated -= callbackfunc;
	}
	public void RegisterTileChanged (Action<Tile> callbackfunc) {
		cbTileChanged += callbackfunc;
	}

	public void UnregisterTileChanged (Action<Tile> callbackfunc) {
		cbTileChanged -= callbackfunc;
	}

	//get called whenever any tile changes
	void OnTileChanged(Tile t) {
		if (cbTileChanged == null)
			return;
		
		cbTileChanged (t);
		InvalidateTileGraph ();
	}

	//this shound be calles whenever a change to the world means that our old pathfinding info is invalid
	public void InvalidateTileGraph(){
		tileGraph = null;
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
