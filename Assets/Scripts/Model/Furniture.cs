using UnityEngine;
using System.Collections;
using System;

//installedObjectsd are thiunhgds like walls doors anf furnitures

public class Furniture {
	
	//this reperesents the BASE tile of the object, but in practice, large objects may actually occupy multiple tiles

	public Tile tile { get; protected set; }

	//contrairement a tile, on prédéfini pas tous les types de furniture par un enum
	//du coup on définis juste son objectType comme étant un string (bha WAISSS)
	public string  objectType { get; protected set; }

	// c'est un multiplier de vitesse, donc 2 tu vas à moitié de vitesse (zarb)
	// tile types and other environmental effects may be combined
	//for example a rough tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3) 
	// would have a total movement cost of (2+3+3 = 8), so you woud move through this tile at 1/8th normal speed
	//SPECIAL if movementCost = 0 then this tile is impassible (like WAAAAAAAAAALLLLL)
	public float movementCost { get; protected set;}

	//for example, a sofa might be 3x2 
	int width;
	int height;


	public bool linksToNeighbour{ get; protected set; }

	Action<Furniture> cbOnChanged;

	Func<Tile, bool> funcPositionValidation;

	// TODO : implement larger objects
	// TODO implement object roation

	protected Furniture(){
		
	}


	static public Furniture CreatePrototype( string objectType, float movementCost = 1f, int width=1, int height=1, bool linksToNeighbour=false ) {
		Furniture obj = new Furniture ();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;
		obj.linksToNeighbour = linksToNeighbour;

		obj.funcPositionValidation = obj.__IsValisPosition;

		return obj;
	}

	static public Furniture PlaceInstance( Furniture proto, Tile tile ) {
		if (proto.funcPositionValidation (tile) == false) {
			Debug.LogError ("placeInstance -- Position Validity Function returned FALSE");
			return null;
		}

		//we know our placement destination is valid

		Furniture obj = new Furniture ();

		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;
		obj.linksToNeighbour = proto.linksToNeighbour;

		obj.tile = tile;

		//FIXME: this assume we are 1x1 !!
		if( tile.PlaceFurniture(obj) == false ) {
			//for some reason we weren't able to place our oublject in this tile
			//probably it was already occupied

			//do NOT return our newly instantiated object. 
			//It will wbe garbaged collected
			return null;

		}

		if (obj.linksToNeighbour) {
			//this type of furniture links itself to its neighbours,
			//so we should inform our neighbours that they have a new buddy
			//just trigger their OnChangedCallback

			Tile t;
			int x = tile.X;
			int y = tile.Y;

			t = tile.world.GetTileAt(x, y + 1);
			if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
				//we have a northern neighbour with the same object type as us, so tell it that it has changed by firing its callback
				t.furniture.cbOnChanged (t.furniture);
			}	
			t = tile.world.GetTileAt(x+1, y);
			if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
				t.furniture.cbOnChanged (t.furniture);
			}
			t = tile.world.GetTileAt(x, y - 1);
			if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
				t.furniture.cbOnChanged (t.furniture);
			}
			t = tile.world.GetTileAt(x-1, y);
			if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
				t.furniture.cbOnChanged (t.furniture);
			}


		}

		return obj;
	}

	public void RegisterOnChangedCallback(Action<Furniture> callbackFunc){
		cbOnChanged += callbackFunc;
	}

	public void UnregisterOnCHangeCallback(Action<Furniture> callbackFunc){
		cbOnChanged -= callbackFunc;
	}

	public bool IsValidPosition(Tile t){
		return funcPositionValidation (t);
	}

	// FIXME these fuctions should never be called directly
	//so souldh'ht be public fucntion furniture
	public bool __IsValisPosition(Tile t){
		//make sure tile is floor
		if( t.Type != TileType.Floor ) {
			return false;
		}
	
		//make sure tile does'nt already have furniture
		if (t.furniture != null) {
			return false;
		}

		return true;
	}

	public bool __IsValidPosition_Door(Tile t)  {
		if (__IsValisPosition (t) == false)
			return false;
		//make sure we have a pair of E/W or N/S walls

		return true;
	}
}
