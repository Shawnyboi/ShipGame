using UnityEngine;
using System.Collections;


//this class mostly controls information pertaining to the collider of the ships
public class ShipColliderController : MonoBehaviour {

	public Collider m_TriggerCollider;
	public Canvas m_AttackedCanvas;

	void Awake(){
		m_AttackedCanvas.enabled = false;
	}

	//Called whenever a collider enters a trigger collider
	void OnTriggerEnter(Collider otherCollider){
		if(otherCollider.gameObject.layer == LayerMask.NameToLayer("PlayerShips") || otherCollider.gameObject.layer == LayerMask.NameToLayer("ComShips")){
			Debug.Log("StunBlocked!");
			StartCoroutine (otherCollider.gameObject.GetComponent<ShipMovementController>().StunBlock(otherCollider));


		}
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
