using UnityEngine;
using System.Collections;

//This class handles information pertaining to the projectiles that certain ships shoot
public class CannonballController : MonoBehaviour {

	public Rigidbody m_Rigidbody;
	public Collider m_Collider;
	public GameObject m_CollisionExplosionPrefab;
	public float m_MaxLifeTime;

	[HideInInspector] public bool m_IsAlive;

	protected LayerMask m_LayerMask;


	protected AudioSource m_ExplosionSound;
	protected ParticleSystem m_CollisionExplosion;
	protected float m_DamageAmount = 0f;
	protected int m_FriendlyLayer;
	protected int m_EnemyLayer;
	protected LayerMask m_FriendlyLayerMask;
	protected LayerMask m_EnemyLayerMask;

	protected void Start(){
		
	}

	protected void Update(){

	}

	virtual protected void Awake(){
		m_CollisionExplosion = Instantiate (m_CollisionExplosionPrefab).GetComponent<ParticleSystem> ();
		m_ExplosionSound = m_CollisionExplosion.GetComponent<AudioSource> ();
	}

	//If the cannonball collides with a ship the ship will be damage
	void OnCollisionEnter(Collision col){
		
		if (col.gameObject.layer != m_FriendlyLayer) { //this check prevents friendly fire
			
			if (col.gameObject.layer == LayerMask.NameToLayer ("PlayerShips") || col.gameObject.layer == LayerMask.NameToLayer ("ComShips")) {
				
				Explode ();
				col.gameObject.GetComponent<ShipStatusController> ().Damage (m_DamageAmount);
				gameObject.SetActive (false);

			} else {
				
				Explode ();
				gameObject.SetActive (false); //explode but don't call damage if hit something that's not a ship


			}
		}
	}

	/// <summary>
	/// this function gets called to activate various explosion effects of the cannonball
	/// </summary>
	virtual protected void Explode(){
		m_CollisionExplosion.transform.position = transform.position;
		m_CollisionExplosion.Play ();
		m_ExplosionSound.Play ();

	}

	//This function will be called by the Damagecontroller before firing to determine the damage the cannonball is to do.
	public void GiveDamageAmount(float amt){
		
		m_DamageAmount = amt;

	}

	/// <summary>
	/// This gets called when the cannonball is instantiated so as to not cause friendly fire
	/// </summary>
	public void SetEnemyLayer(LayerMask enemyLayerMask){
		
		m_EnemyLayerMask = enemyLayerMask;
		int playerMask = 1 << LayerMask.NameToLayer ("PlayerShips");
		int comMask = 1 << LayerMask.NameToLayer ("ComShips");

		if ((enemyLayerMask.value & playerMask) > 0) { //if enemymask and playermask are the same then this is a com ship
			
			m_FriendlyLayer = LayerMask.NameToLayer ("ComShips");
			m_EnemyLayer = LayerMask.NameToLayer ("PlayerShips");
			m_FriendlyLayerMask = comMask;

		} else {
			
			m_FriendlyLayer = LayerMask.NameToLayer ("PlayerShips");
			m_EnemyLayer = LayerMask.NameToLayer ("ComShips");
			m_FriendlyLayerMask = playerMask;

		}

	}

	//This coroutine starts a timer after which the cannonball will be set inactive
	public IEnumerator BeginLifeTime(){
		m_IsAlive = true;
		yield return new WaitForSeconds (m_MaxLifeTime);
		m_IsAlive = false;
		gameObject.SetActive (false);
	}
}
