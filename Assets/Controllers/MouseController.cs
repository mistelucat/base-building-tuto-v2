using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {



	public GameObject circleCursor;

	// créé une valeur lasrtFramePosition comme une coordonnée 3d position et direction
	Vector3 lastFramePosition;

	//choppe le point qui sera mise en mémoire pour le drag and drop
	Vector3 dragStartPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//Camera.main pécho la caméra qui est tagged MainCamera
		//ScreenToWorldPoint transforme un point sur le screen en point to the world
		Vector3 currFramePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		//c'est de la 2d, on veut pas gérer de z position
		currFramePosition.z = 0;
	

		//update the circle cursor position
		Tile tileUnderMouse = GetTileAtWorldCoord(currFramePosition);
		if (tileUnderMouse != null) {
			circleCursor.SetActive (true);
			Vector3 cursorPosition = new Vector3 (tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
		} 
		else {
			//on désactive circlecursor si le mouse est au dessus d'un tile null
			circleCursor.SetActive (false);
		}

		//Start drag
		if (Input.GetMouseButtonDown (0) ) {
			dragStartPosition = currFramePosition;

		}

		//end drag (drop)
		if ( Input.GetMouseButtonUp(0) ) {
			int start_x = Mathf.FloorToInt (dragStartPosition.x );
			int end_x = Mathf.FloorToInt (currFramePosition.x );

			// on swappe si en drag & drop vers la gauche
			if (end_x < start_x) {
				int tmp = end_x;
				end_x = start_x;
				start_x = tmp;

			}

			int start_y = Mathf.FloorToInt (dragStartPosition.y );
			int end_y = Mathf.FloorToInt (currFramePosition.y );
		

			// on swappe si en drag & drop vers la gauche
			if (end_y < start_y) {
				int tmp = end_y;
				end_y = start_y;
				start_y = tmp;

			}

			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						t.Type = Tile.TileType.Floor;
					}
				}

		
			}

		}

		//Handle screen dragging on right or middle mouse button
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {
			
			//comme on est en void update, on peut savoir la différence entre la position du curseur
			//sur la frame actuelle, et la frame précédente
			Vector3 diff = lastFramePosition - currFramePosition;
			//et on bouge la position de la caméra de cette valeur diff
			Camera.main.transform.Translate( diff );


		}
			
		lastFramePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		lastFramePosition.z = 0;

	}

	//on détermine quel tile est à quelle coordonnée du monde, ce qui est facile
	//car leur nom correspond directement à leur coordonnée, par exemple Tile_5_5 a pour coord x 5 y 5 (z 0)
	Tile GetTileAtWorldCoord(Vector3 coord){
		//Mathf.FloorToInt converti un float en int
		//Comme ça si notre curseur est à 5.125165 de coord x, on aura 5 x
		int x = Mathf.FloorToInt(coord.x);
		int y = Mathf.FloorToInt(coord.y);

		// pécho le premier gameobject dans la hiérarchie de type WOrldController
		//GameObject.FindObjectOfType<WorldController>();
		//mais on fait autre chose

		//on va chopper l'instance worldController, on l'a appellé comme ça et rendu static 
		//et le world tile data qu'on a créé qui s'appelle world
		//gettileat c'est une class créée dans world


		return WorldController.Instance.World.GetTileAt(x, y);
	}
}

