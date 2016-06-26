using UnityEngine;
using System.Collections.Generic;

public class CharacterSpriteController : MonoBehaviour {




	Dictionary<Character, GameObject> characterGameObjectMap;
	Dictionary<string, Sprite> characterSprites;


	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	void Start () {
		LoadSprites ();

		// Instantiate our dictionnaray that tracks which GamObject is rendering which Tile data
		characterGameObjectMap = new Dictionary<Character, GameObject>();


		world.RegisterCharacterCreated(OnCharacterCreated);

		//DEBUG
		Character c = world.CreatedCharacter(world.GetTileAt(world.Width/2, world.Height/2));

		//c.SetDestination (world.GetTileAt (world.Width / 2+5, world.Height / 2));

	}
	void LoadSprites(){
		characterSprites = new Dictionary <string, Sprite>();
		//permet d'aller pécho directement dans le répertoire spécial "resources" de unity, qui se loade automatiquement dans le projet
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Characters/");

		//Debug.Log ("LOADED RESOURCE:");
		foreach (Sprite s in sprites) {
			//Debug.Log(s);
			characterSprites [s.name] = s;
		}

	}



	public void OnCharacterCreated( Character c ) {
		Debug.Log ("OnCharacterCreated");
		//creater a visueal GameObject linked ti this data

		//FIXME does not consider multitile objects nor ratated objects

		GameObject char_go = new GameObject(); 

		characterGameObjectMap.Add (c, char_go );

		char_go.name = "Character";
		char_go.transform.position = new Vector3( c.X, c.Y, 0);
		char_go.transform.SetParent(this.transform, true);

		SpriteRenderer sr = char_go.AddComponent<SpriteRenderer> ();
		sr.sprite = characterSprites["p1_front"];
		sr.sortingLayerName = "Characters";

		//register our callback so that GameObject gets updated whenever the object's type changes
		c.RegisterOnChangedCallback( OnCharacterChanged );

	}

	void OnCharacterChanged( Character c ) {
		//Debug.LogError("OnFurnitureChanged -- NOT IMPLEMENTED !");
		// make sure the furniture graphics are correct

		if (characterGameObjectMap.ContainsKey(c) == false) {
			Debug.LogError("OnCharactgerChanged -- trying to change visual for character not in our map");
			return;
		}
		GameObject char_go = characterGameObjectMap[c];
		//char_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(furn);
		char_go.transform.position = new Vector3(c.X, c.Y, 0);
	}



}
