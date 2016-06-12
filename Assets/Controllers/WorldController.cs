using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class WorldController : MonoBehaviour {


	
	//un static, c'est un truc commun à toute la classe worldcontroller 
	//pas seulement à tel ou tel objet worldcontroller

	//on fait un getter,comme ça Instance peut pas être modifié
	public static WorldController Instance { get; protected set; }

	public Sprite floorSprite; //FIXME


	Dictionary<Tile, GameObject> tileGameObjectMap;
	Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;
	Dictionary<string, Sprite> installedObjectSprites;

	//world et tile data qui peut être acess, mais pas modifie
	public World World { get; protected set; }


	// Use this for initialization
	void Start () {

		installedObjectSprites = new Dictionary <string, Sprite>();
		//permet d'aller pécho directement dans le répertoire spécial "resources" de unity, qui se loade automatiquement dans le projet
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects/");

		foreach (Sprite s in sprites) {
			installedObjectSprites [s.name] = s;
		}


		if (Instance != null) {
			Debug.LogError ("there shound never ne two world controllers dude.");
		}
		Instance = this;

	//créé le monde vide empty tiles
		World = new World ();

		World.RegisterInstalledObjectCreated (OnInstalledObjectCreated);



		// Instantiate our dictionnaray that tracks which GamObject is rendering which Tile data
		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();


		//create a GameObject for each of our tiles, so they show visually.
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				//get the tile data
				Tile tile_data = World.GetTileAt(x, y);

				//crée un tile GameObject et l'ajoute à notre scene
				GameObject tile_go = new GameObject (); 

				// Add our tile/GO pair tot he dictionnary
				tileGameObjectMap.Add (tile_data, tile_go);

				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				//on dit que les tiles sont les child de this qui est le worldcontroller en ce qui concerne 
				//transform CAD sa position rotation et scale
				tile_go.transform.SetParent(this.transform, true);

				//add a sprite renderer, but don't bother setting a sprite
				//because now all the tiles are empty 
				tile_go.AddComponent<SpriteRenderer>();

				//register our callback so that GameObject gets updated whenever the tile's type changes
				tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged );
			}
		}

		World.RandomizeTiles();
	}
		


	// Update is called once per frame
	void Update () {

	}

	// exemple, -- not currently used !
	void DestroyTileGameObjects() {
		//pourrait être utililisé quand on change floors/levels
		//ondoit détruire tous les visuels GameObjects mais pas le tile data !

		while (tileGameObjectMap.Count > 0) {
			Tile tile_data = tileGameObjectMap.Keys.First ();
			GameObject tile_go = tileGameObjectMap [tile_data];
			//remove the mair from the map
			tileGameObjectMap.Remove(tile_data);
			//unregister the callback !
			tile_data.UnRegisterTileTypeChangedCallback( OnTileTypeChanged );
			// et enfin destroy the visual GameObject
			Destroy( tile_go );
		}

		//presumably, afther this func gets called, we'd be calling another to build all the GameObjects for the tiules on the new floor/level

	}



	//this function should be called automatically whenever a tile's type gets changed
	void OnTileTypeChanged(Tile tile_data ) {

		//on fait un check si y'a pas de key au [tile_data]
		if (tileGameObjectMap.ContainsKey (tile_data) == false) {
			Debug.LogError ("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionnary ? or maybe to forget to unregister a callback ?");
			return;
		}

		GameObject tile_go = tileGameObjectMap[tile_data];

		//on fait un check si GameObject est pas null
		if (tile_go == null) {
			Debug.LogError ("tileGameObjectMap returned GameObject is null -- did you forget to add the tile to the dictionnary ? or maybe to forget to unregister a callback ?");
			return;
		}

		if (tile_data.Type == TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = floorSprite;
		} else if (tile_data.Type == TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = null;
		} else {
			Debug.LogError ("OnTileTypeChanged - Pas reconnu tile type.");
		}

	}


	//on détermine quel tile est à quelle coordonnée du monde, ce qui est facile
	//car leur nom correspond directement à leur coordonnée, par exemple Tile_5_5 a pour coord x 5 y 5 (z 0)
	public Tile GetTileAtWorldCoord(Vector3 coord){
		//Mathf.FloorToInt converti un float en int
		//Comme ça si notre curseur est à 5.125165 de coord x, on aura 5 x
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		//on va chopper l'instance worldController, on l'a appellé comme ça et rendu static 
		//et le world tile data qu'on a créé qui s'appelle world
		//gettileat c'est une class créée dans world


		return World.GetTileAt(x, y);
	}

	public void OnInstalledObjectCreated( InstalledObject obj ) {
		//Debug.Log ("OnInstalledObjectCreated");
		//creater a visueal GameObject linked ti this data

		//FIXME does not consider multitile objects nor ratated objects

		GameObject obj_go = new GameObject(); 

		installedObjectGameObjectMap.Add (obj, obj_go );

		obj_go.name = obj.objectType + "_" + obj.tile.X + "_" + obj.tile.Y;
		obj_go.transform.position = new Vector3( obj.tile.X, obj.tile.Y, 0);
		obj_go.transform.SetParent(this.transform, true);

		//FIXME We assume taht the object must ve awall, so use the hardcoded reference to the wall sprite
		obj_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(obj);


		//register our callback so that GameObject gets updated whenever the object's type changes
		obj.RegisterOnChangedCallback( OnInstalledObjectChanged );
	 
	}

	Sprite GetSpriteForInstalledObject(InstalledObject obj) {
		if (obj.linksToNeighbour == false) {
			return installedObjectSprites [obj.objectType];
		}

		//otherwise, the sprite name is more complicated
		string spriteName = obj.objectType + "_";

		//check for neighbours North, east, South, West (clockwise)
		int x = obj.tile.X;
		int y = obj.tile.Y;
		Tile t;

		t = World.GetTileAt(x, y + 1);
		if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType){
			spriteName += "N";
		}	
		t = World.GetTileAt(x+1, y);
		if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType){
			spriteName += "E";
		}
		t = World.GetTileAt(x, y - 1);
		if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType){
			spriteName += "S";
		}
		t = World.GetTileAt(x-1, y);
		if(t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType){
			spriteName += "W";
		}

		//on fait un mesage d'erreur si il y a des tiles manquants
		if (installedObjectSprites.ContainsKey (spriteName) == false) {
			Debug.LogError ("GetSpriteForInstalledObject -- no sprites withe name: " + spriteName);
			return null;
		}

		//for example if this object has all four neightbourgds of the same ttype,
		//then the string will look like Wall_NESW

		return installedObjectSprites[spriteName];
	

	}

	void OnInstalledObjectChanged( InstalledObject obj ) {
		Debug.LogError("OnInstalledObjectChanged -- NOT IMPLEMENTED !");
	}
}
	