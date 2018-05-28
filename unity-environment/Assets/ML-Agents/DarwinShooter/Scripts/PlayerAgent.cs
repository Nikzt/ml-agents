using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAgent : Agent {
	private PlayerControl pc;
	InfoUI info;
	void Start () {
		pc = GetComponent<PlayerControl>();
		RequestDecision();
		info = GameObject.Find("Text").GetComponent<InfoUI>();

	}

	void Update() {
		info.SetText(pc.threat.ToString());
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
		pc.stats = pc.stats.normalized;
		AddVectorObs(pc.stats[0]);
		AddVectorObs(pc.stats[1]);
		AddVectorObs(pc.stats[2]);
		pc.stats = Vector3.zero;
	}



	void UpdateParameters(float[] vectorAction) {
		pc.w = vectorAction;
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
		for (int i = 0; i < vectorAction.Length; i++) {
			vectorAction[i] = Mathf.Clamp(vectorAction[i], 0f, 1f);
		}
		UpdateParameters(vectorAction);
	}

	
}
