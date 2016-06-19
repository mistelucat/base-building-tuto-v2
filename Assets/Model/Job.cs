using UnityEngine;
using System.Collections.Generic;
using System;

public class Job {

	//this class holds info for a queued up job, which can include
	//things like placing furniture, movinf stored inventory,
	//working at a desk, and maybe fighting ennemy !!


	//ici on définit les grandes lignes qui vont déterminer des jobs

	//on veut que un job soit associé à un Tile, ce qui donnera un x et un y
	public Tile tile{ get; protected set;}
	//on veut que le job ait un temps d'accomplissement
	float jobTime;

	public string jobObjectType { get; protected set; }

	//CALLBACK !
	//on fait une action qui except a job to be past to it 
	//Quand le job est complete, on execute this code 
	Action<Job> cbJobComplete;

	Action<Job> cbJobCancel;

	//on fait un public constructor de job
	//on décide du temps de base pour réaliser le job est de 1s
	public Job ( Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 1f ) {
		this.tile = tile;
		this.jobObjectType = jobObjectType;
		this.cbJobComplete += cbJobComplete;
	}

	public void RegisterJobCompleteCallback(Action<Job> cb){
		cbJobComplete += cb;
	}

	public void RegisterJobCancelCallback(Action<Job> cb){
		cbJobCancel += cb;
	}

	public void UnregisterJobCompleteCallback(Action<Job> cb){
		cbJobComplete -= cb;
	}

	public void UnregisterJobCancelCallback(Action<Job> cb){
		cbJobCancel -= cb;
	}

	public void DoWork(float workTime) {
		jobTime -= workTime;

		if(jobTime <= 0) {
			if(cbJobComplete != null)
				cbJobComplete(this);
		}
	}

	public void CancelJob(){
		if (cbJobCancel != null)
			cbJobCancel (this);
	}
}

