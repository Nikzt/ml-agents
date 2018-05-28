using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAgent : Agent {
	private PlayerControl pc;
	void Start () {
		pc = GetComponent<PlayerControl>();
	}

	void Update() {
	}

	public override void AgentReset()
	{
		// Move player to random position
		pc.PlayerReset();
	}

	public override void AgentOnDone() {
	}

	public override void CollectObservations()
	{
		AddVectorObs(pc.distanceToClosestPlayer);
		AddVectorObs(pc.HP);
		AddVectorObs(pc.lowestHealthPlayerHP);
		AddVectorObs(pc.distanceToClosestCover);
	}



	void UpdateParameters(float[] vectorAction) {
		pc.w = vectorAction;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		UpdateParameters(vectorAction);
	}

	
}
