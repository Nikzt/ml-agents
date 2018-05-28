using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public int origin; // number of player who shot this projectile
	public float timeOut;
	private float timeActive;
	LineRenderer lr;
	void Start () {
		lr = GetComponent<LineRenderer>();
		
	}
	
	public void DrawShot(Vector3 origin, Vector3 target) {
		lr.positionCount = 2;
		lr.SetPosition(0, origin);
		lr.SetPosition(1, target);
		StartCoroutine("DelayedDestroy");
	}
	
	public IEnumerator DelayedDestroy() {
		yield return new WaitForSeconds(0.2f);
		lr.positionCount = 0;	
	}
}
