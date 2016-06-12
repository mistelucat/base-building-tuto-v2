using UnityEngine;
using System.Collections;
using System;

//installedObjectsd are thiunhgds like walls doors anf furnitures

public class InstalledObject {
	
	//this reperesents the BASE tile of the object, but in practice, large objects may actually occupy multiple tiles

	public Tile tile { get; protected set; }

	//contrairement a tile, on prédéfini pas tous les types d'installedobjects par un enum
	//du coup on définis juste son objectType comme étant un string (bha WAISSS)
	public string  objectType { get; protected set; }

	// c'est un multiplier de vitesse, donc 2 tu vas à moitié de vitesse (zarb)
	// tile types and other environmental effects may be combined
	//for example a rough tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3) 
	// would have a total movement cost of (2+3+3 = 8), so you woud move through this tile at 1/8th normal speed
	//SPECIAL if movementCost = 0 then this tile is impassible (like WAAAAAAAAAALLLLL)
	float movementCost;

	//for example, a sofa might be 3x2 
	int width;
	int height;

	public bool linksToNeighbour{ get; protected set; }

	Action<InstalledObject> cbOnChanged;

	// TODO : implement larger objects
	// TODO implement object roation

	protected InstalledObject(){
		
	}


	static public InstalledObject CreatePrototype( string objectType, float movementCost = 1f, int width=1, int height=1, bool linksToNeighbour=false ) {
		InstalledObject obj = new InstalledObject ();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;
		obj.linksToNeighbour = linksToNeighbour;

		return obj;
	}

	static public InstalledObject PlaceInstance( InstalledObject proto, Tile tile ) {
		InstalledObject obj = new InstalledObject ();

		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;
		obj.linksToNeighbour = proto.linksToNeighbour;

		obj.tile = tile;

		//FIXME: this assume we are 1x1 !!
		if( tile.PlaceObject(obj) == false ) {
			//for some reason we weren't able to place our oublject in this tile
			//probably it was already occupied

			//do NOT return our newly instantiated object. 
			//It will wbe garbaged collected
			return null;

		}

		return obj;
	}

	public void RegisterOnChangedCallback(Action<InstalledObject> callbackFunc){
		cbOnChanged += callbackFunc;
	}

	public void UnregisterOnCHangeCallback(Action<InstalledObject> callbackFunc){
		cbOnChanged -= callbackFunc;
	}

}
