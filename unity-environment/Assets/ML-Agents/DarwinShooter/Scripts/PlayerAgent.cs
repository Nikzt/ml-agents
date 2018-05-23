using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAgent : Agent {
	Rigidbody rb;
	public float speed = 4f;
	public float HP;
	public int playerNum;
	private float shootCooldown = 0.4f;
	public float shootCooldownTimer = 0.4f;
	private bool shotReady = true;
	private int prevKills = 0;
	private int kills = 0;
	private float prevDamageDone = 0f;
	private float damageDone = 0f;
	public bool dead = false;
	public GameObject projectileStandard;
	private GameObject[] otherPlayers;
	public GameObject[] spawnPoints;

	private GameObject target;
	private GameObject moveTarget;
	private NavMeshAgent nav;

	private float distanceToClosestPlayer;
	private float distanceToClosestCover;
	private float lowestHealthPlayerHP;
	private Vector3 aimDir;

	private GameObject[] coverPoints;

	private float timeSinceLastHit = 3f;

	void Start () {
		rb = GetComponent<Rigidbody>();	
		HP = 1.0f;
		shootCooldownTimer = shootCooldown;
		otherPlayers = GameObject.FindGameObjectsWithTag("player");
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		coverPoints = GameObject.FindGameObjectsWithTag("cover");
		target = GetClosestPlayer();
		nav = GetComponent<NavMeshAgent>();
		StartCoroutine("HPRegen");
	}

	void Update() {
		shootCooldownTimer -= Time.deltaTime;
		timeSinceLastHit += Time.deltaTime;
		if (shootCooldownTimer <= 0f) {
			shotReady = true;
			Aim(target.transform.position, 0.3f);
			Shoot();
			Move();
		}
	}

	IEnumerator HPRegen() {
		while(true) {
			if (timeSinceLastHit >= 3f) {
				if (HP <= 0.9f) {
					HP += 0.1f;
				} else {
					HP = 1.0f;
				}
			}
			yield return new WaitForSeconds(1f);
		}
	}

	void SwitchTarget(float threshold) {
		// target the closest player, the player with the least hp,
		// or the player that has done the most damage to me in the 
		// last few seconds
		GameObject closest = GetClosestPlayer();
		GameObject lowhp = GetLowestHealthPlayer();
		if (lowestHealthPlayerHP >= 1f) {
			target = closest;
		} else if (distanceToClosestPlayer - lowestHealthPlayerHP <= threshold) {
			target = lowhp;
		} else {
			target = closest;
		}

	}

	void SwitchMovement(float threshold) {
		// change the movement strategy between chasing a player and
		// and taking cover

		if (HP < threshold) {
			TakeCover();
		} else {
			Chase();
		}
	}

	void TakeCover() {
		GameObject cover = GetClosestCover();
		if (cover != null) {
			moveTarget = cover;
		} else {
			Chase();
		}
	}

	void Chase() {
		moveTarget = target;
	}

	GameObject GetClosestCover() {
		float minDist = 99999f;
		GameObject closestCover = null;
		foreach(GameObject cover in coverPoints)
		{
			float dist = Vector3.Distance(cover.transform.position, transform.position);
			if (dist <= minDist && cover.GetComponent<CoverPoint>().CheckCover(this.gameObject)) {
				minDist = dist;
				closestCover = cover;
			}
		}
		distanceToClosestCover = minDist / 100f;
		return closestCover;
	}


	public override void AgentReset()
	{
		// Move player to random position
		this.transform.position = spawnPoints[Random.Range(0, 8)].transform.position;
		this.rb.angularVelocity = Vector3.zero;
        this.rb.velocity = Vector3.zero;
		this.HP = 1.0f;
	}

	public override void CollectObservations()
	{
		// Relative positions to all other players
		// Squash values into [-1,1] range by dividing by half the stage width
	//	foreach(GameObject player in otherPlayers)
	//	{
	//		Vector3 playerPosition = player.transform.position - this.transform.position;
	//		if (player.name != this.name) {
	//			AddVectorObs(playerPosition.x / 20);
	//			AddVectorObs(playerPosition.z / 20);
	//			AddVectorObs(player.GetComponent<PlayerAgent>().HP);
	//		}
	//	}
	//	AddVectorObs(HP);
		AddVectorObs(lowestHealthPlayerHP);
		AddVectorObs(distanceToClosestPlayer);
		// Whether each player is in line of sight or not (raycast?)

		// Distances to each wall
	}

	void OnTriggerEnter(Collider collision) {

		// take damage if hit by enemy projectile
		if (collision.gameObject.tag == "projectile"){
			if (collision.gameObject.GetComponent<Projectile>().origin != playerNum) {
				HP -= 0.1f;
				timeSinceLastHit = 0f;
				GameObject player = FindPlayerWithNum(collision.gameObject.GetComponent<Projectile>().origin);
				PlayerAgent agent = player.GetComponent<PlayerAgent>();
				if (HP <= 0f) {
					agent.KilledPlayer();
					dead = true;
				} else {
					agent.DealtDamage(0.1f);
				}
				Destroy(collision.gameObject);
			}
		}
	}

	public void KilledPlayer() {
		prevKills = kills;
		kills += 1;
	}

	public void DealtDamage(float damage) {
		prevDamageDone = damageDone;
		damageDone += damage;
	}

	void Shoot() {

		// Shoot basic projectile

		Vector3 shotPos = transform.position;
		GameObject shot = Instantiate(projectileStandard, shotPos, Quaternion.identity);
		shot.GetComponent<Projectile>().origin = playerNum;
		shot.GetComponent<Rigidbody>().velocity = aimDir * 80f;
		shotReady = false;
		shootCooldownTimer = shootCooldown;
	}

	void Aim(Vector3 target, float error) {
		// Face target with random error in rotation (imperfect aim)
		Vector3 dir = target - transform.position;
		dir = dir.normalized;	
		dir.x += (Random.value - 0.5f) * error;
		dir.z += (Random.value - 0.5f) * error;
		aimDir = dir.normalized;

	}

	GameObject GetClosestPlayer() {
		float minDist = 99999f;
		GameObject closestPlayer = null;
		foreach(GameObject player in otherPlayers)
		{
			if (player.GetComponent<PlayerAgent>().playerNum != playerNum) {
				float dist = Vector3.Distance(player.transform.position, transform.position);
				if (dist <= minDist) {
					minDist = dist;
					closestPlayer = player;
				}
			}
		}
		distanceToClosestPlayer = minDist / 100f;
		return closestPlayer;
	}

	GameObject GetLowestHealthPlayer() {
		float minHealth = 1f;
		GameObject lowestHealthPlayer = null;
		foreach(GameObject player in otherPlayers)
		{
			if (player.GetComponent<PlayerAgent>().playerNum != playerNum) {
				float health = player.GetComponent<PlayerAgent>().HP;
				if (health <= minHealth) {
					minHealth = health;
					lowestHealthPlayer = player;
				}
			}
		}
		lowestHealthPlayerHP = minHealth;
		return lowestHealthPlayer;
	}

	void Move() {
		nav.SetDestination(moveTarget.transform.position);
	}

	public override void AgentAction(float[] vectorAction, string textAction)
	{
	    // killed a player
	    if (kills > prevKills)
	    {
	        AddReward(1.0f);
			prevKills = kills;
	    }
	
	    // hit a player
	    if (damageDone > prevDamageDone)
	    {
	        AddReward(0.1f);
			prevDamageDone = damageDone;
	    }


	    // player died
	    if (dead == true)
	    {
	        AddReward(-1.0f);
			dead = false;
			Done();
	    }

	    float threshold = Mathf.Clamp(vectorAction[0], -1, 1);
//		Debug.Log(threshold);
		SwitchTarget(threshold);
		SwitchMovement(0.7f);
	 }

	 GameObject FindPlayerWithNum(int num) {
		 foreach (GameObject player in otherPlayers) {
			 if (player.GetComponent<PlayerAgent>().playerNum == num) {
				 return player;
			 }
		 }
		 return null;
	 }
	
}
