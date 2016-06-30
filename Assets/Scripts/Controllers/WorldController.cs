using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;


public class WorldController : MonoBehaviour {

	//un static, c'est un truc commun à toute la classe worldcontroller 
	//pas seulement à tel ou tel objet worldcontroller
	//on fait un getter,comme ça Instance peut pas être modifié
	public static WorldController Instance { get; protected set; }

	//world et tile data qui peut être acess, mais pas modifie
	public World world { get; protected set; }

	//static = belong to the class and not the instance of worldcontroller
	//kinda global parameter
	static bool loadWorld = false;

	// Use this for initialization
	//on utilise OnEnable pour qu'il s'exécute en PREMIER, avant start
	void OnEnable () {
		if (Instance != null) {
			Debug.LogError ("there shound never be two world controllers dude.");
		}
		Instance = this;

		if (loadWorld) {
			loadWorld = false;
			CreateWorldFromSaveFile ();
		} 
		else {
			CreateEmptyWorld ();
		}
	}
		
	//ici on fait le liens entre l'update de la classe world, qui est NOTRE update, et l'update de unity, parce que ici on est en monobeaviour
	void Update() {
		//TODO add pause/unpause, speed controls, calendar ect ect...
		world.Update(Time.deltaTime);

	}

	//ondétermine quel tile est à quelle coordonnée du monde, ce qui est facile
	//car leur nom correspond directement à leur coordonnée, par exemple Tile_5_5 a pour coord x 5 y 5 (z 0)
	public Tile GetTileAtWorldCoord(Vector3 coord){
		//Mathf.FloorToInt converti un float en int
		//Comme ça si notre curseur est à 5.125165 de coord x, on aura 5 x
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		//on va chopper l'instance worldController, on l'a appellé comme ça et rendu static 
		//et le world tile data qu'on a créé qui s'appelle world
		//gettileat c'est une class créée dans world


		return world.GetTileAt(x, y);
	}
	public void NewWorld(){
		Debug.Log ("NewWorld button was clicked you silly");
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void SaveWorld(){
		Debug.Log ("save world button was clicked");

		//turn an object into Xml
		XmlSerializer serializer = new XmlSerializer (typeof(World));
		//write that somewhere, actualling in memory
		TextWriter writer = new StringWriter();
		serializer.Serialize (writer, world);
		writer.Close ();

		Debug.Log(writer.ToString());

		PlayerPrefs.SetString("SaveGame00", writer.ToString());

	}

	public void LoadWorld(){
		Debug.Log ("load world button was clicked");
		//reload the scene to reset all data(and purge old references) 
		loadWorld = true;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void CreateEmptyWorld(){
		//créé le monde vide empty tiles
		world = new World (100, 100);

		//center the camera
		Camera.main.transform.position = new Vector3(world.Width/2, world.Height/2, Camera.main.transform.position.z );

	}

	void CreateWorldFromSaveFile(){
		Debug.Log ("CreateWorldFromSaveFile");
		//créé le monde from our save file daga


		XmlSerializer serializer = new XmlSerializer (typeof(World));
		TextReader reader = new StringReader (PlayerPrefs.GetString ("SaveGame00"));
		Debug.Log(reader.ToString());
		world = (World)serializer.Deserialize (reader);
		reader.Close ();

		//center the camera
		Camera.main.transform.position = new Vector3(world.Width/2, world.Height/2, Camera.main.transform.position.z );

	}
}
	