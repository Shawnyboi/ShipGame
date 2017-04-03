using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour {

	public float m_Lifetime;
	public float m_DamagePerSecond;

	void Awake(){
		StartCoroutine (BeginLifetime());
	}

	void OnTriggerEnter(Collider otherCollider){
		Debug.Log ("on collision enter called in fireball");
		GameObject otherObject = otherCollider.gameObject;
		if (otherObject.layer == LayerMask.NameToLayer ("PlayerShips") || otherObject.layer == LayerMask.NameToLayer ("ComShips")) {
			Debug.Log ("calling burn ship");
			StartCoroutine(otherObject.GetComponent<ShipStatusController> ().BurnShip (m_DamagePerSecond));

		}
	}

	//This coroutine starts a timer after which the fireball will be destroyed
	public IEnumerator BeginLifetime(){
		yield return new WaitForSeconds (m_Lifetime);
		Destroy (gameObject);
	}



}
