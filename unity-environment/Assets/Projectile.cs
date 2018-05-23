using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public int origin; // number of player who shot this projectile
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag != "player" && collider.gameObject.tag != "projectile") {
			Destroy(this.gameObject);
		}
	}
}
