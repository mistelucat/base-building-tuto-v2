using UnityEngine;
using System.Collections.Generic;

public class JobSpriteController : MonoBehaviour {

	//this bare-bones controller is mostly just going to piggyback
	//on FrunitureSpriteController because we don't yet fully know
	//what our job system is going to look like in the end


	FurnitureSpriteController fsc;
	Dictionary<Job, GameObject> jobGameObjectMap;

	// Use this for initialization
	void Start () {
		jobGameObjectMap = new Dictionary<Job, GameObject> ();
		fsc = GameObject.FindObjectOfType<FurnitureSpriteController> ();

		//FIXME no such thing as a job queue yet !
		WorldController.Instance.world.jobQueue.RegisterJobCreationCallback (OnJobCreated);
	}

	void OnJobCreated(Job job){
		//FIXME wecan only do furniture building jobs !!
		//TODO sprite

		GameObject job_go = new GameObject(); 

		if(jobGameObjectMap.ContainsKey(job)){
			Debug.LogError("OnJobCreated for a jobGO that already exist -- most likely a job being re queued, as opposed to created");
			return;
		}

		//Add our tile/go pair to the dictionary
		jobGameObjectMap.Add (job, job_go );

		job_go.name = "JOB_" + job.jobObjectType + "_" + job.tile.X + "_" + job.tile.Y;
		job_go.transform.position = new Vector3( job.tile.X, job.tile.Y, 0);
		job_go.transform.SetParent(this.transform, true);

		SpriteRenderer sr = job_go.AddComponent<SpriteRenderer> ();
		sr.sprite = fsc.GetSpriteForFurniture( job.jobObjectType );
		sr.color = new Color ( 0.5f, 1f, 0.5f, 0.5f );
		sr.sortingLayerName = "Jobs";

		job.RegisterJobCompleteCallback(OnJobEnded);
		job.RegisterJobCancelCallback(OnJobEnded);
	}

	void OnJobEnded(Job job){
		//this execute wheter a job was completed or cancelled
		//FIXME wecan only do furniture building jobs !!

		//TODO delete sprites

		GameObject job_go = jobGameObjectMap[job];

		job.UnregisterJobCompleteCallback (OnJobEnded);
		job.UnregisterJobCancelCallback (OnJobEnded);

		Destroy (job_go);
	}
}
