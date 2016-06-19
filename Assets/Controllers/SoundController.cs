using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	float soundCooldown = 0;

	// Use this for initialization
	void Start () {

		WorldController.Instance.world.RegisterFurnitureCreated( OnFurnitureCreated );

		WorldController.Instance.world.RegisterTileChanged( OnTileChanged );
	}
	
	// Update is called once per frame
	void Update () {
		soundCooldown -= Time.deltaTime;
	
	}

	void OnTileChanged( Tile tile_data ) {
		//FIXME

		//si le son est en cours d'exécution depuis moins d'1/10 de secoonde, on return, bail
		if (soundCooldown > 0)
			return;

		AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
		// Camera.main.transform.position on joue le son au top de la camera
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		soundCooldown = 0.1f; //1/10 de seconde
	}

	public void OnFurnitureCreated( Furniture furn ){
		//FIXME
		if (soundCooldown > 0)
			return;
		
		AudioClip ac = Resources.Load<AudioClip>("Sounds/"+ furn.objectType +"_OnCreated");

		if (ac == null) {
			//WTF ? what do we do ?
			//Since there's no specific sound for whatever furniture this, juste unse a default sound, wall_oncreatedsound

			ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");

		}

		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		soundCooldown = 0.1f;
	}

}
