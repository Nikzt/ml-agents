using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public float timeLimit;
	public float gameTime;
	public int[] scores;
	GameObject[] players;
	void Start () {
		players = GameObject.FindGameObjectsWithTag("player");
		scores = new int[4];
		scores[0] = 0;
		scores[1] = 0;
		scores[2] = 0;
		scores[3] = 0;
		gameTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		gameTime += Time.deltaTime;	
		if (gameTime >= timeLimit) {
			ResetGame();
		}

	}
	void ResetGame() {
		scores[0] = 0;
		scores[1] = 0;
		scores[2] = 0;
		scores[3] = 0;
		foreach (GameObject player in players) {
			PlayerAgent agent = player.GetComponent<PlayerAgent>();
			agent.AgentReset();
		}
		Debug.Log("Restarted Game");
		gameTime = 0f;
		
	}
	public void UpdateScore(int playerNum) {
		scores[playerNum - 1] += 1;
	}
}
