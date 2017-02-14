using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarBallController : CannonballController {

	public float m_ExplosionRadius;

	override protected void Explode(){
		base.Explode ();
		Collider[] hitShipColliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_EnemyLayerMask);
		for (int i = 0; i < hitShipColliders.Length; i++) {
			if (hitShipColliders [i].isTrigger == false) {
				hitShipColliders [i].GetComponent<ShipStatusController> ().Damage (m_DamageAmount);
			}
		}

	}
}
