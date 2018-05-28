using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControl : MonoBehaviour {

	// Object/Component References

	Rigidbody rb;
	public GameObject projectileStandard;
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

	// Targeting

	private GameObject target;
	private GameObject moveTarget;
	private Vector3 aimDir;

	// Other

	bool isAgent;
	public string state;
	public float[] w;

	void Start () {
		rb = GetComponent<Rigidbody>();	
		HP = 1.0f; // HP goes from 0 to 1
		otherPlayers = GameObject.FindGameObjectsWithTag("player");
		spawnPoints = GameObject.FindGameObjectsWithTag("spawn");
		coverPoints = GameObject.FindGameObjectsWithTag("cover");

		// Initially start chasing closest player
		target = GetClosestPlayer();
		moveTarget = GetClosestPlayer();

		nav = GetComponent<NavMeshAgent>();
		rend = GetComponent<Renderer>();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();

		ammo = maxAmmo;
		shootCooldownTimer = shootCooldown;

		StartCoroutine("HPRegen");

		if (GetComponent<PlayerAgent>() == null) {
			isAgent = false;
			w = new float[] {0.5f, 0.5f,0.5f, 0.5f,0.5f, 0.5f};
		} else {
			isAgent = true;
			pa = GetComponent<PlayerAgent>();
		}

	}

	void Update() {
		shootCooldownTimer -= Time.deltaTime;
		timeSinceLastHit += Time.deltaTime;
		SelectTarget();
		SelectTactic();
		if (shootCooldownTimer <= 0f) {
			shotReady = true;
			Aim(target.transform.position, 0.3f);
			if (CheckLineOfSight(target)) {
				Shoot();
			}
			Move();
		}
	}

	void SelectTactic() {
		float v1 = EvaluateTactic("chase");
		float v2 = EvaluateTactic("cover");
		if (v1 > v2) {
			Chase();

			rend.material.SetColor("_Color", Color.red);
		} else {
			TakeCover();
			rend.material.SetColor("_Color", Color.blue);
		}
	}

	void SelectTarget() {
		GameObject closest = GetClosestPlayer();
		GameObject lowhp = GetLowestHealthPlayer();
		float v1 = distanceToClosestPlayer;
		float v2 = lowestHealthPlayerHP;
		if (w[4] * v1 < w[5] * v2) {
			target = GetClosestPlayer();
		} else {
			target = GetLowestHealthPlayer();
		}
	}

	float EvaluateTactic(string tactic) {
		GetClosestCover();
		GetClosestPlayer();
		if (tactic == "chase") {
			return w[0] * HP + w[1] * distanceToClosestCover + w[2] * (1f - distanceToClosestPlayer) + w[3] * (1f - lowestHealthPlayerHP); 
		} else if (tactic == "cover") {
			return w[0] * (1f - HP) + w[1] * (1f - distanceToClosestCover) + w[2] * distanceToClosestPlayer + w[3] * lowestHealthPlayerHP; 
		} else {
			return 0f;
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
		nav.SetDestination(moveTarget.transform.position);
	}

	void Shoot() {

		// Shoot basic projectile

		if (ammo > 0 & !reloading) {

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

	void OnTriggerEnter(Collider collision) {

		// take damage if hit by enemy projectile
		if (collision.gameObject.tag == "projectile"){
			if (collision.gameObject.GetComponent<Projectile>().origin != playerNum) {
				HP -= 0.1f;
				timeSinceLastHit = 0f;
				GameObject player = FindPlayerWithNum(collision.gameObject.GetComponent<Projectile>().origin);
				PlayerControl agent = player.GetComponent<PlayerControl>();
				if (HP <= 0f) {
					agent.KilledPlayer();
					PlayerReset();
					if (isAgent) {
						pa.Done();
						pa.AddReward(-1f);
					}
				} else {
					agent.DealtDamage(0.1f);
					damageTaken += 0.1f;
				}
				StartCoroutine(collision.gameObject.GetComponent<Projectile>().DelayedDestroy());
			}
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
