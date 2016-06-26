using UnityEngine;
using System.Collections;
using System;



//TileType is the base type to the tile.
public enum TileType { Empty, Floor };

public class Tile {
	private TileType _type = TileType.Empty;
	public TileType Type {
		get { return _type; }
		set {
			TileType oldType = _type;
			_type = value;

			//call the callback and let things know we've changed.
			if(cbTileChanged != null && oldType != _type)
				cbTileChanged(this);
		}
	}

	Inventory inventory;
	public Furniture furniture { get; protected set; }

	public Job PendingFurnitureJob;


	public World world { get; protected set; }
	public int X { get; protected set; }
	public int Y { get; protected set; }

	public float movementCost {
		get{
			if (Type == TileType.Empty)
				return 0; //0 is unwalkable
			if (furniture == null)
				return 1;

			return 1 * furniture.movementCost;
		}
	}
	//la fonction qu'on appelle à chaque fois que notre data de tile change !
	//cb=callback the function we callback anytime our tile data changes
	//c'est super puissant mec, action instance ? similaire void method
	Action<Tile> cbTileChanged;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="world">World.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>

	public Tile( World world, int x, int y ) {
		this.world = world;
		this.X = x;
		this.Y = y;

	}
	/// <summary>
	/// Registers the tile type changed callback.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void RegisterTileTypeChangedCallback(Action<Tile> callback) {
		cbTileChanged += callback;

	}
	/// <summary>
	/// Uns the register tile type changed callback.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void UnRegisterTileTypeChangedCallback(Action<Tile> callback) {
	cbTileChanged -= callback;
	}

	public bool PlaceFurniture(Furniture objInstance) {
		if (objInstance == null) {
			//we are uninstalling whatever was here before
			furniture = null;
			return true;
		}

		//objIsntance isn't null
		if (furniture != null) {
			Debug.LogError ("trying to assign a furniture to a tile that already has one !!");
			return false;
		}

		//at this point, everything's fine !

		furniture = objInstance;
		return true;
	}

	//tells us if two tiles are adjacent or diagonal
	public bool IsNeighbour(Tile tile, bool diagOkay = false ) {
		//Firest are we on the same x column ? if so see if we differ in our y by exactly one
		//Mathf.Abs prends l'absolu, si c'est 1, ou -1 mathf.Abs rendra 1 
		if (this.X == tile.X && (Mathf.Abs( this.Y - tile.Y) == 1 ))
			return true;

		//now check on the same y row....
		if (this.Y == tile.Y && (Mathf.Abs( this.X - tile.X) == 1 ))
			return true;

		if (diagOkay) {
			if (this.X == tile.X + 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y-1 ))
				return true;
			if (this.X == tile.X - 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y-1 ))
				return true;
		}
		return false;
	}


	public Tile[] GetNeighbours(bool diagOkay = false){

		Tile[] ns;

		if (diagOkay == false) {
			ns = new Tile[4]; //Tile order : N E S W
		} else {
			ns = new Tile[8]; // Tile order N E S W NE SE SW NW
		}

		Tile n;
		n = world.GetTileAt (X, Y + 1);
		ns [0] = n; //Could be null, but that's okay !
		n = world.GetTileAt (X+1, Y);
		ns [1] = n;
		n = world.GetTileAt (X, Y - 1);
		ns [2] = n;
		n = world.GetTileAt (X-1, Y);
		ns [3] = n;

		if (diagOkay == true) {
			n = world.GetTileAt (X+1, Y + 1);
			ns [4] = n; //Could be null, but that's okay !
			n = world.GetTileAt (X+1, Y-1);
			ns [5] = n;
			n = world.GetTileAt (X-1, Y - 1);
			ns [6] = n;
			n = world.GetTileAt (X-1, Y+1);
			ns [7] = n;

		}
		//and finally return the neighbours
		return ns;
	}

}
