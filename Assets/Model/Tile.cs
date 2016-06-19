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

}
