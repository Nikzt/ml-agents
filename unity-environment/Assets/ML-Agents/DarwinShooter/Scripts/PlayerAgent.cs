using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAgent : Agent {
	Rigidbody rb;
	public float speed = 4f;
	public float HP;
	public int playerNum;
	public float shootCooldown = 0.4f;
	private float shootCooldownTimer = 0.4f;
	public int maxAmmo = 10;
	private int ammo;
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

	private float thresholdTarget = 0.5f;
	private float thresholdHP = 0.5f;

	void Start () {
		rb = GetComponent<Rigidbody>();	
		HP = 1.0f;
		ammo = maxAmmo;
		shootCooldownTimer = shootCooldown;
		otherPlayers = GameObject.FindGameObjectsWithTag("player");
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		coverPoints = GameObject.FindGameObjectsWithTag("cover");
		target = GetClosestPlayer();
		moveTarget = GetClosestPlayer();
		nav = GetComponent<NavMeshAgent>();
		StartCoroutine("HPRegen");
	}

	void Update() {
		shootCooldownTimer -= Time.deltaTime;
		timeSinceLastHit += Time.deltaTime;
		if (shootCooldownTimer <= 0f) {
			shotReady = true;
			Aim(target.transform.position, 0.3f);
			if (CheckLineOfSight(target)) {
				Shoot();
			}
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

	bool CheckLineOfSight(GameObject shootTarget) {
		RaycastHit hitPoint;
		Ray ray = new Ray();
		ray.direction = shootTarget.transform.position - transform.position;
		ray.origin = transform.position;
		if (Physics.Raycast(ray, out hitPoint, Mathf.Infinity)) {
			if (hitPoint.collider.tag == "player") {
				return true;
			}
		}
		return false;
	}

	void SwitchTarget(float threshold) {
		// target the closest player, the player with the least hp,
		// or the player that has done the most damage to me in the 
		// last few seconds
		GameObject closest = GetClosestPlayer();
		GameObject lowhp = GetLowestHealthPlayer();
		if (lowestHealthPlayerHP >= 1f) {
			target = closest;
		} else if (distanceToClosestPlayer >= threshold * lowestHealthPlayerHP) {
			target = lowhp;
		} else {
			target = closest;
		}

	}

	void SelectTactic(float[] act) {
       int action = Mathf.FloorToInt(act[0]);
       switch (action)
       {
           case 0:
				Chase();
				target = GetClosestPlayer();
               break;
           case 1:
		   		Chase();
				target = GetLowestHealthPlayer();
               break;
           case 2:
		   		TakeCover();
				target = GetClosestPlayer();
               break;
           case 3:
		   		TakeCover();
				target = GetLowestHealthPlayer();
               break;
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
		distanceToClosestCover = minDist / 20f;
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
		AddVectorObs(lowestHealthPlayerHP);
		AddVectorObs(distanceToClosestPlayer);
		AddVectorObs(distanceToClosestCover);
		AddVectorObs(HP);
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
					AddReward(-1f);
					AgentReset();
					Done();
				} else {
					agent.DealtDamage(0.1f);
					AddReward(-0.01f);
				}
				StartCoroutine(collision.gameObject.GetComponent<Projectile>().DelayedDestroy());
			}
		}
	}

	public void KilledPlayer() {
		prevKills = kills;
		kills += 1;
		AddReward(0.5f);
	}

	public void DealtDamage(float damage) {
		prevDamageDone = damageDone;
		damageDone += damage;
		AddReward(0.005f);
	}

	void Shoot() {

		// Shoot basic projectile

		if (ammo > 0) {

			Vector3 shotPos = transform.position;
			GameObject shot = Instantiate(projectileStandard, shotPos, Quaternion.identity);
			shot.GetComponent<Projectile>().origin = playerNum;
			shot.GetComponent<Rigidbody>().velocity = aimDir * 80f;
			shotReady = false;
			shootCooldownTimer = shootCooldown;
			ammo -= 1;
		} else {
			StartCoroutine("Reload");
		}
	}

	IEnumerator Reload() {
		yield return new WaitForSeconds(2f);
		ammo = maxAmmo;
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
		distanceToClosestPlayer = minDist / 20f;
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
		SelectTactic(vectorAction);
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
