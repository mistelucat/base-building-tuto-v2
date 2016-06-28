using UnityEngine;
using System.Collections;
using System;

public class Character {

	//what character class he knows about itself


	//nouvelle fonction !! Mathf.Lerp 
	//public static float Lerp(float a, float b, float t);
	//Linearly inteTilerpolates between a and b by t.
	public float X { 
		get {
			return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
		}
	}

	public float Y { 
		get {
			return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);
		}
	}


	public Tile currTile { get; protected set;}
	Tile destTile; //if we aren't moving, then destTile = currTile
	Tile nextTile; //the nextile in the pathfinding sequence
	Path_AStar pathAStar;
	float movementPercentage; //Goes from 0 to 1 as we move from currTile to dest

	float speed = 5f; //Tiles per second

	Action<Character> cbCharacterChanged; 

	Job myJob;

	//on fait le public constructor for our caracter
	public Character(Tile tile) {
		//on détermine au début que son tile est celui où il se trouve, et où il veut aller
		//il bouge pas son cul
		currTile = destTile = nextTile = tile;
	}

	void Update_DoJob(float deltaTime){
		//do i have a job ? if i don't---
		if (myJob == null){
			//---let's just grab the first job in queue
			myJob = currTile.world.jobQueue.Dequeue();

			//if i have one---
			if (myJob != null) {
				//---We have a job ! let's go the the tile of that job

				//TODO check to see if the job is reachable

				destTile = myJob.tile;
				myJob.RegisterJobCancelCallback (OnJobEnded);
				myJob.RegisterJobCompleteCallback (OnJobEnded);
			}
		}


		//are we there yet?
		if (myJob != null && currTile == myJob.tile) {
		//if (pathAStar != null && pathAStar.Length() == 1) { //we are adjacent to the job site
			if (myJob != null) {
				myJob.DoWork (deltaTime);
			}
		}
	}

	public void AbandonJob(){
		nextTile = destTile = currTile;
		pathAStar = null;
		currTile.world.jobQueue.Enqueue(myJob);
		myJob = null;

	}

	void Update_DoMovement(float deltaTime){
		if (currTile == destTile) {
			pathAStar = null;
			return;//we're already were we want to be.
		}

		if(nextTile == null || nextTile == currTile){
			//get the next tile from the pathfinder
			if (pathAStar == null || pathAStar.Length() == 0 ) {
				//calculate a path from curr to dest
				pathAStar = new Path_AStar(currTile.world, currTile, destTile);
				if (pathAStar.Length () == 0) {
					Debug.LogError ("Path_AStar returned no path to destination !");
					AbandonJob ();
					pathAStar = null;
					return;
				}
			}
			//grab the next waypoint from the pathfinding system
			nextTile = pathAStar.Dequeue();

			if (nextTile == currTile) {
				Debug.LogError ("Update_DoMovement - nextTile is currTile ?");
			}
		}
	
/*		if(pathAStar.Length() == 1 ){
			return;
		}
*/
		//at this point we should have a valid nextTile  to move to !



		//on détermnie la distance entre les deux points theoreme de pythagore mec ??
		//for now it is euclidian, but when pathfinding system we do, manhattan or chebyshev system BRO !!
		//Pow !!!
		//public static float Pow(float f, float p);
		//Returns f raised to power p.
		//public static float Sqrt(float f); -- Returns square root of f.
		float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X-nextTile.X, 2) +  Mathf.Pow(currTile.Y-nextTile.Y, 2));

		//on détermine la distance par frame pour qu'elle soit égale quelque que soit le nombre de fps
		//how much distance can be traveled this update
		float distThisFrame = speed * deltaTime;

		// how much is that in terms of percentage to our destination ? (de 0 a 1)
		float percThisFrame = distThisFrame / distToTravel;

		//add that overall poercentage travelled
		movementPercentage += percThisFrame;

		if (movementPercentage >= 1) {
			//we have reached our destination

			//TODO : get the next tile from the pathfinding system
			//If there are no more tiles, then we have TRULKY reach our destination

			currTile = nextTile;
			movementPercentage = 0;
			//FIXME do we actually want to retain any overshot movement ?
		}

	}



	//Update le truc se fait à chaque frame
	//on fait un flat deltaTime comme ça c'est nous qui décidont du deltaTime, et pas unity
	//comme ça on pourra le modifier comme on veut
	public void Update(float deltaTime){
		//Debug.Log("Character Update");
		Update_DoJob(deltaTime);

		Update_DoMovement(deltaTime);

		if (cbCharacterChanged != null)
			cbCharacterChanged (this);
		
	}

	public void SetDestination(Tile tile) {
		if(currTile.IsNeighbour(tile, true) == false) {
			Debug.Log ("Character::SetDestination -- out destination tile isn't actually our neighbour.");
		}

		destTile = tile;
	}
	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}
	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}
	void OnJobEnded(Job j){
	//job completed or was canceled
		if (j != myJob) {
			Debug.LogError ("Character being told about job that isn't his. You forgot to un regesiter something.");
			return;
		}

		myJob = null;
	}
}
