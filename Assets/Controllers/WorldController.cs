using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {
	
	//un static, c'est un truc commun à toute la classe worldcontroller 
	//pas seulement à tel ou tel objet worldcontroller

	//on fait un getter,comme ça Instance peut pas être modifié
	public static WorldController Instance { get; protected set; }

	public Sprite floorSprite;

	//world et tile data qui peut être acess, mais pas modifie
	public World World { get; protected set; }


	// Use this for initialization
	void Start () {
		if (Instance != null) {
			Debug.LogError ("there shound never ne two world controllers dude.");
		}
		Instance = this;

	//créé le monde vide empty tiles
		World = new World ();


		//create a GameObject for each of our tiles, so they show visually.
		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				//get the tile data
				Tile tile_data = World.GetTileAt(x, y);

				//crée un tile gameobject et l'ajoute à notre scene
				GameObject tile_go = new GameObject (); 
				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0);
				//on dit que les tiles sont les child de this qui est le worldcontroller en ce qui concerne 
				//transform CAD sa position rotation et scale
				tile_go.transform.SetParent(this.transform, true);

				//add a sprite renderer, but don't bother setting a sprite
				//because now all the tiles are empty 
				tile_go.AddComponent<SpriteRenderer>();

				// un lambda est une fonction anonyme on-the-fly
				// () => {}  c'est pareil que faire void foo(){} c'est faire une nouvelle fonction qui fait rien
				// sauf qu'elle a pas de nom elle est anonyme 
				tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
			}
		}

		World.RandomizeTiles();
	}
		


	// Update is called once per frame
	void Update () {

	}


	void OnTileTypeChanged(Tile tile_data, GameObject tile_go) {

		if (tile_data.Type == Tile.TileType.Floor) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = floorSprite;
		} else if (tile_data.Type == Tile.TileType.Empty) {
			tile_go.GetComponent<SpriteRenderer> ().sprite = null;
		} else {
			Debug.LogError ("OnTileTypeChanged - Pas reconnu tile type.");
		}

	}

}
	