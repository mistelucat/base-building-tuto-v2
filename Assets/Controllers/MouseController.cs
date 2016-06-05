using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {

	public GameObject circleCursorPrefab;

	// créé une valeur lasrtFramePosition comme une coordonnée 3d position et direction
	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	//choppe le point qui sera mise en mémoire pour le drag and drop
	Vector3 dragStartPosition;
	//on créé une liste de gameobjects qui s'appelle dragpreviewmescouilles
	List<GameObject> dragPreviewGameObjects;

	// Use this for initialization
	void Start () {
		dragPreviewGameObjects = new List<GameObject> ();
	}	
	
	// Update is called once per frame
	void Update () {
		//Camera.main pécho la caméra qui est tagged MainCamera
		//ScreenToWorldPoint transforme un point sur le screen en point to the world
		currFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		//c'est de la 2d, on veut pas gérer de z position
		currFramePosition.z = 0;

		//UpdateCursor ();
		UpdateDragging ();
		UpdateCameraMovement ();

		//on veut la position de la souris sur la frame actuelle
		//on utilise pas currentframeposition, parce que on peut avoir bougé la caméra !
		lastFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;

	}

/*	void UpdateCursor() {

		//update the circle cursor position
		Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord (currFramePosition);
		if (tileUnderMouse != null) {
			circleCursor.SetActive (true);
			Vector3 cursorPosition = new Vector3 (tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
		} else {
			//on désactive circlecursor si le mouse est au dessus d'un tile null
			circleCursor.SetActive (false);
		}
	}
	*/

	void UpdateDragging() {
		
		//Start drag
		//down, ça veut dire que on a cliqué sur les frames précédentes
		if (Input.GetMouseButtonDown (0)) {
			dragStartPosition = currFramePosition;
		}

		int start_x = Mathf.FloorToInt (dragStartPosition.x);
		int end_x = Mathf.FloorToInt (currFramePosition.x);
		int start_y = Mathf.FloorToInt (dragStartPosition.y);
		int end_y = Mathf.FloorToInt (currFramePosition.y);

		// on swappe si en drag & drop vers la gauche la "mauvaise" direction
		//perso je trouve ça sale

		if (end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}
		if (end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		//clean up old drag previews
		//tant que y'a un gameobject dans dragpreviewmescouilles, (si y'en a un avec l'entree [0] ) tu l'enlève et le delete
		while(dragPreviewGameObjects.Count > 0) {
			GameObject go = dragPreviewGameObjects[0];
			dragPreviewGameObjects.RemoveAt(0);
			//au lieu de le delete, on va le despawn du simplepool, comme ça ça évite de créer détruire à mort plein d'éléments
			//ça fait économiser des ressources
			SimplePool.Despawn (go);
		}

		//juste le getmousedbutton, ça veut dire qu'on viens JUSTE de clique, sur CETTE frame
		if (Input.GetMouseButton (0)) {
			//display a preview of the drag area
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						//display the building hint on top of this tile positon
						//Quaternion.identity c'est pour créer un vector sans rotation
						// on créé un gameobject qui s'appelle go, et on l'ajoute (add) a la liste de gameobject dragpreviewmescouilles
						//et on le créé avec le script en CC simplepool
						GameObject go = SimplePool.Spawn( circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity );

						//ça permet juste de faire en sorte que les novueaux objets go soient apparentés à mousecontroller
						//c'est plus propre dans la hierarchy des objets créés
						go.transform.SetParent (this.transform, true);

						dragPreviewGameObjects.Add (go);
					}
				}
			}
		}


		//end drag (drop)
		if (Input.GetMouseButtonUp (0)) {

			//go on loope through all the tiles
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						t.Type = Tile.TileType.Floor;
					}
				}
			}
		}
	}

		void UpdateCameraMovement() {

		//Handle screen panning on right or middle mouse button
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) {
			
			//comme on est en void update, on peut savoir la différence entre la position du curseur
			//sur la frame actuelle, et la frame précédente
			Vector3 diff = lastFramePosition - currFramePosition;
			//et on bouge la position de la caméra de cette valeur diff
			Camera.main.transform.Translate (diff);

		}
	}
			
		//lastFramePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		//lastFramePosition.z = 0;

}
