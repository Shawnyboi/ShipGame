using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a derived class of shipfirecontroller which is to be used for self destructing ships
/// </summary>
public class BombShipFireController :ShipFireController {

	public float m_ExplosionRadius;
	public ParticleSystem m_Explosion;
	public AudioSource m_ExplosionSound;
	public GameObject m_ExplosionPrefab;

	override protected void Awake(){
		
		m_Transform = gameObject.GetComponent<Transform> ();
		m_ShipAttributes = gameObject.GetComponent<ShipAttributes> ();
		m_Disposition = "peaceful";
		m_Attacking = false;
		m_Explosion = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem> ();
		m_ExplosionSound = m_Explosion.GetComponent<AudioSource> ();

	}

	//The fire function for this ship just calls the detonate function when within range;
	override protected bool Fire(Vector3 targetPosition){
		
		if (Vector3.Distance (m_Transform.position, targetPosition) < m_Range) {
			
			Detonate ();
			return true;

		} else {
			
			return false;

		}

	}

	//the detonate function damages enemy ships around the bomb ship and kills the bomb ship itself
	private void Detonate(){
		
		Collider[] hitShipColliders = Physics.OverlapSphere (m_Transform.position, m_ExplosionRadius, m_EnemyTeamMask);
		m_Explosion.transform.position = m_Transform.position;
		m_Explosion.Play ();
		m_ExplosionSound.Play ();

		for (int i = 0; i < hitShipColliders.Length; i++) {
			if (hitShipColliders [i].isTrigger == false) { //we only want the collider to be counted once
				hitShipColliders [i].gameObject.GetComponent<ShipStatusController> ().Damage (m_Damage);
			}
		}

		this.gameObject.GetComponent<ShipStatusController> ().Damage (1000000f); //kill self by damaging with really large number
	}

}
