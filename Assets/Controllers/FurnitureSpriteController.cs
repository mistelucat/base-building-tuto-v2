using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class FurnitureSpriteController : MonoBehaviour {


	Dictionary<Furniture, GameObject> furnitureGameObjectMap;
	Dictionary<string, Sprite> furnitureSprites;


	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	//on utilise OnEnable pour qu'il s'exécute en PREMIER, avant start
	void Start () {
		LoadSprites ();
	
		// Instantiate our dictionnaray that tracks which GamObject is rendering which Tile data
		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

	


		world.RegisterFurnitureCreated(OnFurnitureCreated);
	}
		
	void LoadSprites(){
		furnitureSprites = new Dictionary <string, Sprite>();
		//permet d'aller pécho directement dans le répertoire spécial "resources" de unity, qui se loade automatiquement dans le projet
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furniture/");

		Debug.Log ("LOADED RESOURCE:");
		foreach (Sprite s in sprites) {
			Debug.Log(s);
			furnitureSprites [s.name] = s;
		}

	}


		
	public void OnFurnitureCreated( Furniture furn ) {
		//creater a visueal GameObject linked ti this data

		//FIXME does not consider multitile objects nor ratated objects

		GameObject furn_go = new GameObject(); 

		furnitureGameObjectMap.Add (furn, furn_go );

		furn_go.name = furn.objectType + "_" + furn.tile.X + "_" + furn.tile.Y;
		furn_go.transform.position = new Vector3( furn.tile.X, furn.tile.Y, 0);
		furn_go.transform.SetParent(this.transform, true);

		furn_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);


		//register our callback so that GameObject gets updated whenever the object's type changes
		furn.RegisterOnChangedCallback( OnFurnitureChanged );
	 
	}

	void OnFurnitureChanged( Furniture furn ) {
		//Debug.LogError("OnFurnitureChanged -- NOT IMPLEMENTED !");
		// make sure the furniture graphics are correct

		if (furnitureGameObjectMap.ContainsKey(furn) == false) {
			Debug.LogError("OnFurnitureChanged -- trying to change visual for furniture not in our map");
			return;
		}
		GameObject furn_go = furnitureGameObjectMap[furn];
		furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
	}



	public Sprite GetSpriteForFurniture(Furniture obj) {
		if (obj.linksToNeighbour == false) {
			return furnitureSprites [obj.objectType];
		}

		//otherwise, the sprite name is more complicated
		string spriteName = obj.objectType + "_";

		//check for neighbours North, east, South, West (clockwise)
		int x = obj.tile.X;
		int y = obj.tile.Y;
		Tile t;

		t = world.GetTileAt(x, y + 1);
		if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
			spriteName += "N";
		}	
		t = world.GetTileAt(x+1, y);
		if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
			spriteName += "E";
		}
		t = world.GetTileAt(x, y - 1);
		if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
			spriteName += "S";
		}
		t = world.GetTileAt(x-1, y);
		if(t != null && t.furniture != null && t.furniture.objectType == obj.objectType){
			spriteName += "W";
		}

		//on fait un mesage d'erreur si il y a des tiles manquants
		if (furnitureSprites.ContainsKey (spriteName) == false) {
			Debug.LogError ("GetSpriteForInstalledObject -- no sprites withe name: " + spriteName);
			return null;
		}

		//for example if this object has all four neightbourgds of the same ttype,
		//then the string will look like Wall_NESW

		return furnitureSprites[spriteName];
	

	}

	public Sprite GetSpriteForFurniture(string objectType) {
		if (furnitureSprites.ContainsKey(objectType)) {
			return furnitureSprites [objectType];
		}

		if (furnitureSprites.ContainsKey(objectType+"_")) {
			return furnitureSprites [objectType+"_"];
		}
		Debug.LogError ("GetSpriteForFurniture -- no sprites withe name: " + objectType);
		return null;

	}
}
	