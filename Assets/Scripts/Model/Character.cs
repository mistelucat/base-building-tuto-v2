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
			return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
		}
	}

	public float Y { 
		get {
			return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
		}
	}


	public Tile currTile { get; protected set;}
	Tile destTile; //if we aren't moving, then destTile = currTile
	float movementPercentage; //Goes from 0 to 1 as we move from currTile to dest

	float speed = 2f; //Tiles per second

	Action<Character> cbCharacterChanged; 

	Job myJob;

	//on fait le public constructor for our caracter
	public Character(Tile tile) {
		//on détermine au début que son tile est celui où il se trouve, et où il veut aller
		//il bouge pas son cul
		currTile = destTile = tile;
	}

	//Update le truc se fait à chaque frame
	//on fait un flat deltaTime comme ça c'est nous qui décidont du deltaTime, et pas unity
	//comme ça on pourra le modifier comme on veut
	public void Update(float deltaTime){
		//Debug.Log("Character Update");

		//do i have a job ? if i don't---
		if (myJob == null){
			//---let's just grab the first job in queue
			myJob = currTile.world.jobQueue.Dequeue();

			//if i have one---
			if (myJob != null) {
				//---We have a job ! let's go the the tile of that job
				destTile = myJob.tile;
				myJob.RegisterJobCancelCallback (OnJobEnded);
				myJob.RegisterJobCompleteCallback (OnJobEnded);
			}
		}


		//are we there yet?
		if (currTile == destTile) {
			if (myJob != null) {
				myJob.DoWork (deltaTime);
			}
			return;

		}
		//on détermnie la distance entre les deux points theoreme de pythagore mec ??
		//for now it is euclidian, but when pathfinding system we do, manhattan or chebyshev system BRO !!
		//Pow !!!
		//public static float Pow(float f, float p);
		//Returns f raised to power p.
		//public static float Sqrt(float f); -- Returns square root of f.
		float distToTravel = Mathf.Sqrt(Mathf.Pow(currTile.X-destTile.X, 2) +  Mathf.Pow(currTile.Y-destTile.Y, 2));

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

			currTile = destTile;
			movementPercentage = 0;
			//FIXME do we actually want to retain any overshot movement ?
		}

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
