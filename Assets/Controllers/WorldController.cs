﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class WorldController : MonoBehaviour {

	//un static, c'est un truc commun à toute la classe worldcontroller 
	//pas seulement à tel ou tel objet worldcontroller
	//on fait un getter,comme ça Instance peut pas être modifié
	public static WorldController Instance { get; protected set; }

	//world et tile data qui peut être acess, mais pas modifie
	public World world { get; protected set; }


	// Use this for initialization
	//on utilise OnEnable pour qu'il s'exécute en PREMIER, avant start
	void OnEnable () {
		if (Instance != null) {
			Debug.LogError ("there shound never ne two world controllers dude.");
		}
		Instance = this;

	//créé le monde vide empty tiles
		world = new World ();

		//center the camera
		Camera.main.transform.position = new Vector3(world.Width/2, world.Height/2, Camera.main.transform.position.z );

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


		return world.GetTileAt(x, y);
	}
}
	