using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a derived class of ShipFireController which is to be used for ships
/// that have only front firing capabilitis
/// </summary>
public class FrontFireController : ShipFireController {

	public Transform[] m_BowFirePositions;

	/// <summary>
	/// This overrides the original fire function so that the ship only fires from the bow
	/// </summary>
	override protected bool Fire(Vector3 targetPosition){

		string targetPositionOrientation = DetermineTargetOrientation (targetPosition);
		Vector3 firePosition;

		if (targetPositionOrientation == "bow") {

			if (m_BowFirePositions != null) {

				firePosition = m_BowFirePositions [Random.Range (0, m_StarboardFirePositions.Length)].position;

			} else {

				return false;//no firing positions

			}

		} else {

			return false;//orientation not at bow so firing is not possible

		}

		if(Vector3.Distance(m_Transform.position, targetPosition) < m_Range){

			if (!m_Cannonball.GetComponent<CannonballController> ().m_IsAlive) {

				LaunchCannonball (firePosition, targetPosition);

				return true;
			}
			return false; //Target not within range
		}
		return false; //Cannonball already fired and isn't gone yet
	}
		

	override protected void LaunchCannonball(Vector3 firePosition, Vector3 targetPosition){
		m_Cannonball.SetActive (true);
		m_Cannonball.transform.position = firePosition;
		m_Cannonball.GetComponent<CannonballController> ().GiveDamageAmount (m_Damage);
		m_Cannonball.GetComponent<CannonballController> ().SetTeamLayer (this.gameObject.layer);
		StartCoroutine (m_Cannonball.GetComponent<CannonballController> ().BeginLifeTime ());
		m_CannonFireSound.Play ();
		m_Cannonball.GetComponent<Rigidbody> ().velocity = calculateBallisticVelocity(firePosition, targetPosition) * m_BowFirePositions[0].forward;


	}

	private float calculateBallisticVelocity(Vector3 firePosition, Vector3 targetPosition){
		float distance = Vector3.Distance (firePosition, targetPosition); //since we are not accounting for initial height our answer will be slightly off
		Debug.Log("Distance = " + distance);
		Debug.Log("angle = " +  m_BowFirePositions [0].transform.rotation.eulerAngles.x);
		Debug.Log ("acc due to gravity " + -Physics.gravity.y);
		Debug.Log( "velocity = " + Mathf.Sqrt (distance * (-Physics.gravity.y) / Mathf.Sin (2 * (360f - m_BowFirePositions [0].transform.rotation.eulerAngles.x))));
		return Mathf.Sqrt (distance * (-Physics.gravity.y) / Mathf.Sin (2 * (360f - m_BowFirePositions [0].transform.rotation.eulerAngles.x)));
	}


	/// <summary>
	/// This ovverrides the original getattack angle so we return the angle to the bow instead of
	/// port and starboard
	/// </summary>
	override public float GetAttackAngle(){

		if (m_Target != null) {

			float signedAngle;
			float unsignedAngle = Vector3.Angle (m_Transform.forward, m_Target.transform.position - m_Transform.position);
			Vector3 crossProduct = Vector3.Cross (m_Transform.forward, m_Target.transform.position - m_Transform.position);

			if (crossProduct.y > 0) {

				signedAngle = -unsignedAngle;

			} else {

				signedAngle = unsignedAngle;

			}

			return -signedAngle;

		} else {

			return 0f;

		}


	}
		
}
