using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public int origin; // number of player who shot this projectile
	private Rigidbody rb;
	void Start () {
		rb = GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag != "player" && collider.gameObject.tag != "projectile") {
			StartCoroutine("DelayedDestroy");
		}
	}
	
	public IEnumerator DelayedDestroy() {
		rb.velocity = Vector3.zero;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(0.5f);
		Destroy(this.gameObject);

	}
}
