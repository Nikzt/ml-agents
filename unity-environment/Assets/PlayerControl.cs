using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour {

	// Object/Component References

	Rigidbody rb;
	public GameObject projectileStandard;
	public Projectile projectile;
	private NavMeshAgent nav;
	private GameController gameController;
	Renderer rend;
	PlayerAgent pa;

	// Player Stats

	public float speed = 4f;
	public int playerNum;
	public float shootCooldown = 0.4f;
	public int maxAmmo = 10;
	private int kills = 0;
	private float damageDone = 0f;
	private float damageTaken;
	private float deaths;

	// Player Resources

	public float HP;
	private float shootCooldownTimer = 0.4f;
	private int ammo;
	private bool shotReady = true;
	public bool reloading = false;
	private float timeSinceLastHit = 3f;

	// Observations

	public float distanceToClosestPlayer;
	public float distanceToClosestCover;
	public float lowestHealthPlayerHP;
	private GameObject[] coverPoints;
	private GameObject[] otherPlayers;
	public GameObject[] spawnPoints;
	public float threat;
	private float[] killPotential;

	public Vector3 stats;

	// Targeting

	private GameObject attackTarget;
	private GameObject moveTarget;
	private Vector3 aimDir;

	// Other

	public bool isAgent;
	public string state;
	public float[] w;

	void Start () {
		rb = GetComponent<Rigidbody>();	
		HP = 1.0f; // HP goes from 0 to 1
		otherPlayers = GetOtherPlayers();
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		coverPoints = GameObject.FindGameObjectsWithTag("cover");
		projectile = Instantiate(projectileStandard).GetComponent<Projectile>();

		// Initially start chasing closest player
		attackTarget = GetClosestPlayer();
		moveTarget = GetClosestPlayer();
		state = "ranged";

		nav = GetComponent<NavMeshAgent>();
		rend = GetComponent<Renderer>();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();

		ammo = maxAmmo;
		shootCooldownTimer = shootCooldown;
		killPotential = new float[otherPlayers.Length];

		StartCoroutine("HPRegen");

		stats = Vector3.zero;

		if (GetComponent<PlayerAgent>() == null) {
			isAgent = false;
			w = new float[] {0.5f, 0.5f,0.5f, 0.5f,0.5f, 0.5f};
		} else {
			isAgent = true;
			pa = GetComponent<PlayerAgent>();
		}


	}

	GameObject[] GetOtherPlayers() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("player");
		GameObject[] oPlayers = new GameObject[players.Length - 1];
		int i = 0;
		foreach (GameObject player in players) {
			PlayerControl agent = player.GetComponent<PlayerControl>();
			if (agent.playerNum != playerNum) {
				oPlayers[i] = player;
				i++;
			}
		}
		return oPlayers;
	}

	void Update() {
		shootCooldownTimer -= Time.deltaTime;
		timeSinceLastHit += Time.deltaTime;
		EvaluateThreat();
		EvaluateKillPotential();
		SelectTarget();
		SelectTactic();
		if (shootCooldownTimer <= 0f) {
			shotReady = true;
			Aim(attackTarget.transform.position, 0.3f);
			if (CheckLineOfSight(attackTarget)) {
				Shoot();
			}
			Move();
		}
	}

	// DECISION MAKING --------------------------------------------------------------------

	void EvaluateThreat() {
		threat = (1f - HP) * (w[3] * (NumInLineOfSight() / 3f) + w[4] * (1f - (ammo / maxAmmo)) + w[5] * distanceToClosestCover);
	}

	void EvaluateKillPotential() {
		int i = 0;
		foreach (GameObject player in otherPlayers) {
			killPotential[i] = 0f;
			killPotential[i] += w[0] * (1f - player.GetComponent<PlayerControl>().HP);
			float dist = Vector3.Distance(player.transform.position, transform.position);
			killPotential[i] += w[1] * (1f - Mathf.Clamp(dist / 20f, 0f, 1f));
			if (CheckLineOfSight(player)) {
				killPotential[i] += w[2];
			}
		}
	}

	void DecideReload() {
		if (NumInLineOfSight() == 0 && ammo < 8) {
			StartCoroutine(Reload());
		} 
	}


	void SelectTactic() {
		if (threat <= 0f) {
			Chase();
			stats[0] += 0.01f;
			rend.material.SetColor("_Color", Color.red);
		} else if (threat >= 0.3f && threat < 1f) {
			RangedAttack();
			stats[1] += 0.01f;
			rend.material.SetColor("_Color", Color.yellow);
		} else {
			TakeCover();
			stats[2] += 0.01f;
			rend.material.SetColor("_Color", Color.blue);
		}
	}

	void SelectTarget() {
		float maxkp = 0f;
		int maxind = 0;
		for (int i = 0; i < otherPlayers.Length; i++) {
			if (killPotential[i] >= maxkp) {
				maxkp = killPotential[i];
				maxind = i;
			}	
		}
		attackTarget = otherPlayers[maxind];
	}

	// ACTIONS -------------------------------------------------------

	void TakeCover() {
		GameObject cover = GetClosestCover();
		if (cover != null) {
			moveTarget = cover;
			state = "cover";
		} else {
			Chase();
		}
	}

	void Chase() {
		moveTarget = attackTarget;
		state = "chase";
	}

	void RangedAttack() {
		moveTarget = attackTarget;
		state = "ranged";
	}

	IEnumerator Reload() {
		reloading = true;
		yield return new WaitForSeconds(2f);
		ammo = maxAmmo;
		reloading = false;
	 }


	void UpdateParameters(float[] vectorAction) {
		w = vectorAction;
	}

	// BASIC ACTIONS --------------------------------------------------------

	void Aim(Vector3 target, float error) {
		// Face target with random error in rotation (imperfect aim)
		Vector3 dir = target - transform.position;
		dir = dir.normalized;	
		dir.x += (Random.value - 0.5f) * error;
		dir.z += (Random.value - 0.5f) * error;
		aimDir = dir.normalized;

	}

	void Move() {
		if (state == "ranged") {
			if (CheckLineOfSight(moveTarget)) {
				nav.SetDestination(transform.position);
			} else {
				nav.SetDestination(moveTarget.transform.position);
			}
		} else {
			nav.SetDestination(moveTarget.transform.position);
		}
	}

	bool Shoot() {

		// Shoot basic projectile

		if (ammo > 0 & !reloading) {

			RaycastHit hitPoint;
			Ray ray = new Ray();
			ray.direction = aimDir;
			ray.origin = transform.position + aimDir * 0.5f;
			shotReady = false;
			shootCooldownTimer = shootCooldown;
			ammo -= 1;
			if (Physics.Raycast(ray, out hitPoint, Mathf.Infinity)) {
				projectile.DrawShot(ray.origin, hitPoint.point);
				if (hitPoint.collider.tag == "player") {
					hitPoint.collider.gameObject.GetComponent<PlayerControl>().Hit(playerNum);
					return true;
				}
			}
			return false;
		} else {
			StartCoroutine("Reload");
			return false;
		}
	}

	 // OBSERVATIONS --------------------------------------------------------

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

	int NumInLineOfSight() {
		int count = 0;
		foreach (GameObject player in otherPlayers) {
			if (CheckLineOfSight(player)) {
				count += 1;
			}
		}
		return count;
	}


	 GameObject FindPlayerWithNum(int num) {
		 foreach (GameObject player in otherPlayers) {
			 if (player.GetComponent<PlayerControl>().playerNum == num) {
				 return player;
			 }
		 }
		 return null;
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
		distanceToClosestCover = Mathf.Clamp(minDist / 20f, 0f, 1f) ;
		return closestCover;
	}



	GameObject GetClosestPlayer() {
		float minDist = 99999f;
		GameObject closestPlayer = null;
		foreach(GameObject player in otherPlayers)
		{
			if (player.GetComponent<PlayerControl>().playerNum != playerNum) {
				float dist = Vector3.Distance(player.transform.position, transform.position);
				if (dist <= minDist) {
					minDist = dist;
					closestPlayer = player;
				}
			}
		}
		distanceToClosestPlayer = Mathf.Clamp(minDist / 20f, 0f, 1f);
		return closestPlayer;
	}

	GameObject GetLowestHealthPlayer() {
		float minHealth = 1f;
		GameObject lowestHealthPlayer = null;
		foreach(GameObject player in otherPlayers)
		{
			if (player.GetComponent<PlayerControl>().playerNum != playerNum) {
				float health = player.GetComponent<PlayerControl>().HP;
				if (health <= minHealth) {
					minHealth = health;
					lowestHealthPlayer = player;
				}
			}
		}
		lowestHealthPlayerHP = minHealth;
		return lowestHealthPlayer;
	}

	public void KilledPlayer() {
		kills += 1;
		if (isAgent) {
			pa.AddReward(1f);
		}
		gameController.UpdateScore(playerNum);
	}

	public void DealtDamage(float damage) {
		damageDone += damage;
		if (isAgent) {
			pa.AddReward(0.005f);
		}
	}
	
	// OTHER -------------------------------------------------------------------

	public void Hit(int pnum) {

		// take damage if hit by enemy projectile
		HP -= 0.1f;
		timeSinceLastHit = 0f;
		GameObject player = FindPlayerWithNum(pnum);
		PlayerControl agent = player.GetComponent<PlayerControl>();
		if (HP <= 0f) {
			agent.KilledPlayer();
			if (isAgent) {
				pa.AddReward(-0.1f);
				PlayerReset();
			} else {
				PlayerReset();
			}
		} else {
			agent.DealtDamage(0.1f);
			damageTaken += 0.1f;
		}
	}

	IEnumerator HPRegen() {
		// Gain 0.1 health per second if havent taken damage in 3 seconds
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


	public void PlayerReset()
	{
		// Move player to random position
		this.transform.position = spawnPoints[Random.Range(0, 8)].transform.position;
		this.rb.angularVelocity = Vector3.zero;
        this.rb.velocity = Vector3.zero;
		this.HP = 1.0f;
		this.deaths += 1f;
	}


	
}
