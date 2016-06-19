using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor.Events;
//using UnityEditor.UI;

public class BuildModeController : MonoBehaviour {


	bool buildModeIsObjects = false;
	TileType buildModeTile = TileType.Floor;
	string buildModeObjectType;


	// Use this for initialization
	void Start () {
	}	

	public void SetMode_BuildFloor( ) {
		buildModeIsObjects = false;
		buildModeTile = TileType.Floor;
	}
	public void SetMode_Bulldoze( ) {
		buildModeIsObjects = false;
		buildModeTile = TileType.Empty;
	}

	public void SetMode_BuildFurniture( string objectType ) {
		//wall pas a tile mais un installedobject qui est au dessus du tile
		buildModeIsObjects = true;
		buildModeObjectType = objectType;
	}

	public void DoBuild(Tile t){
		if (buildModeIsObjects == true) {
			//create the InstalledObject and assigh it to the tile

			//FIXME this instantly builds the furniture
			//WorldController.Instance.World.PlaceInstalledObject( buildModeObjectType, t );


			//can we build the furniture in the selected tile ?
			//run the ValidPlacement function !!
			string furnitureType = buildModeObjectType;

			if (
				WorldController.Instance.world.IsFurniturePlacementValid (furnitureType, t) &&
				t.PendingFurnitureJob == null

			) {
				//This tile positiohn is valid for this furniture

				//on the new job, when you complete, run la construction du furniture
				//mais on va plutôt utiliser un lambda
				//theJob.tile est équivalent à t
				Job j = new Job (t, furnitureType, (theJob) => { 
					WorldController.Instance.world.PlaceFurniture (furnitureType, theJob.tile);
					t.PendingFurnitureJob = null;
				});

				//FIXME i don't like having manually and explicitly set
				//Flags that prevent conflicts. it's too easy to forget to set/clear them
				t.PendingFurnitureJob = j;
				j.RegisterJobCancelCallback( (thejob) => { thejob.tile.PendingFurnitureJob = null; } );

				//add the job to the queue
				WorldController.Instance.world.jobQueue.Enqueue (j);
		
			}
		}
		else {
			//we are in thile-changing mode
			t.Type = buildModeTile;
		}
	}
}
