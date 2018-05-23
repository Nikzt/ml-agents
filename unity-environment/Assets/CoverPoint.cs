using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour {

	private GameObject[] players;

	void Start () {
		players = GameObject.FindGameObjectsWithTag("player");
	}

	// Check if this cover would hide the player from all other players
	public bool CheckCover(GameObject seeker) {
		RaycastHit hitPoint;
		Ray ray = new Ray();
		foreach (GameObject player in players) {
			if (player != seeker) {
				ray.direction = player.transform.position - transform.position;
				ray.origin = transform.position;
				if (Physics.Raycast(ray, out hitPoint, Mathf.Infinity)) {
					if (hitPoint.collider.tag == "player") {
						return false;
					}
				}
			}
		}
		return true;
	}
	

}
