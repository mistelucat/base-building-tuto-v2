using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class TileSpriteController : MonoBehaviour {


	public Sprite floorSprite; //FIXME
	public Sprite emptySprite; //FIXME


	Dictionary<Tile, GameObject> tileGameObjectMap;

	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	//on utilise OnEnable pour qu'il s'exécute en PREMIER, avant start
	void Start () {

		// Instantiate our dictionnaray that tracks which GamObject is rendering which Tile data
		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		//create a GameObject for each of our tiles, so they show visually.
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				//get the tile data
				Tile tile_data = world.GetTileAt(x, y);

				//crée un tile GameObject et l'ajoute à notre scene
				GameObject tile_go = new GameObject (); 

				// Add our tile/GO pair tot he dictionnary
				tileGameObjectMap.Add (tile_data, tile_go);

				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				//on dit que les tiles sont les child de this qui est le worldcontroller en ce qui concerne 
				//transform CAD sa position rotation et scale
				tile_go.transform.SetParent(this.transform, true);


				// Add a sprite renderer add a default sprite for empty tiles
				tile_go.AddComponent<SpriteRenderer>().sprite = emptySprite ;


			}
		}


		//register our callback so that GameObject gets updated whenever the tile's type changes
		world.RegisterTileChanged( OnTileChanged );
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
			tile_data.UnRegisterTileTypeChangedCallback( OnTileChanged );
			// et enfin destroy the visual GameObject
			Destroy( tile_go );
		}

		//presumably, afther this func gets called, we'd be calling another to build all the GameObjects for the tiules on the new floor/level

	}



	//this function should be called automatically whenever a tile's data gets changed
	void OnTileChanged(Tile tile_data ) {

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
		




}
	