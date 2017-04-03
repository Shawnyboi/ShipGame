using UnityEngine;
using System.Collections;


//this class mostly controls information pertaining to the collider of the ships
public class ShipColliderController : MonoBehaviour {

	public Collider m_ShipCollider;
	public Canvas m_AttackedCanvas;

	private bool m_InFire;

	void Awake(){
		m_InFire = false;
		m_AttackedCanvas.enabled = false;
	}

	//Called whenever this objects collider enters a trigger collider
	void OnTriggerEnter(Collider otherCollider){
		/*
		if((otherCollider.gameObject.layer == LayerMask.NameToLayer("PlayerShips") || otherCollider.gameObject.layer == LayerMask.NameToLayer("ComShips"))
			&& otherCollider.gameObject.layer != this.gameObject.layer){

			Debug.Log("StunBlocked!");
			StartCoroutine (otherCollider.gameObject.GetComponent<ShipMovementController>().StunBlock(otherCollider));

		}
		*/

		if (otherCollider.gameObject.layer == LayerMask.NameToLayer ("Fire")) {
			Debug.Log ("entering fire");
			m_InFire = true;
		}
	}




	//called whenever this objects collider leaves a trigger collider
	void OnTriggerExit(Collider otherCollider){

		if (otherCollider.gameObject.layer == LayerMask.NameToLayer ("Fire")) {
			Debug.Log ("leaving fire");
			m_InFire = false;
		}

	}




	/// <summary>
	/// Returns the in fire flag to check f the ship should be burning
	/// </summary>
	public bool ShipIsInFire(){
		return m_InFire;

	}

	/// <summary>
	/// This will get called when this object is attacked
	/// Basically it just blinks a red circle under the object
	/// </summary>
	public IEnumerator BlinkAttackedIndicator(){
		m_AttackedCanvas.enabled = true;
		yield return new WaitForSeconds (.2f);
		m_AttackedCanvas.enabled = false;
		yield return new WaitForSeconds (.2f);
		m_AttackedCanvas.enabled = true;
		yield return new WaitForSeconds (.2f);
		m_AttackedCanvas.enabled = false;

	}



}
