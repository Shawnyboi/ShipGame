using UnityEngine;
using System.Collections;

//This class handles information pertaining to the projectiles that certain ships shoot
public class CannonballController : MonoBehaviour {

	public Rigidbody m_Rigidbody;
	public Collider m_Collider;
	public GameObject m_CollisionExplosionPrefab;
	public float m_MaxLifeTime;

	[HideInInspector] public bool m_IsAlive;

	private AudioSource m_ExplosionSound;
	private ParticleSystem m_CollisionExplosion;
	private float m_DamageAmount = 0f;

	void Start(){
		
	}

	private void Awake(){
		m_CollisionExplosion = Instantiate (m_CollisionExplosionPrefab).GetComponent<ParticleSystem> ();
		m_ExplosionSound = m_CollisionExplosion.GetComponent<AudioSource> ();
	}

	//If the cannonball collides with a ship the ship will be damage
	void OnCollisionEnter(Collision col){
		if (col.gameObject.layer == LayerMask.NameToLayer ("PlayerShips") || col.gameObject.layer == LayerMask.NameToLayer ("ComShips")) {
			m_CollisionExplosion.transform.position = transform.position;
			m_CollisionExplosion.Play ();
			m_ExplosionSound.Play ();
			col.gameObject.GetComponent<ShipStatusController> ().Damage (m_DamageAmount);

			gameObject.SetActive (false);
		} else {
			m_CollisionExplosion.transform.position = transform.position;
			m_CollisionExplosion.Play ();
			m_ExplosionSound.Play ();
			gameObject.SetActive (false); //explode but don't call damage if hit something that's not a ship

		}
	}

	//This function will be called by the Damagecontroller before firing to determine the damage the cannonball is to do.
	public void GiveDamageAmount(float amt){
		m_DamageAmount = amt;
	}

	//This coroutine starts a timer after which the cannonball will be set inactive
	public IEnumerator BeginLifeTime(){
		m_IsAlive = true;
		yield return new WaitForSeconds (m_MaxLifeTime);
		m_IsAlive = false;
		gameObject.SetActive (false);
	}
}
