using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a derived class of ShipFireController which is to be used for ships
/// that have only front firing capabilitis
/// </summary>
public class FrontFireController : ShipFireController {

	public Transform[] m_BowFirePositions;
	protected Vector3 m_GravityVector;


	override protected void Awake(){
		base.Awake ();
		m_GravityVector = Physics.gravity;
	}

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

			if (!m_CannonballController.m_IsAlive) {

				LaunchCannonball (firePosition, targetPosition);

				return true;
			}
			return false; //Target not within range
		}
		return false; //Cannonball already fired and isn't gone yet
	}
		

	override protected void LaunchCannonball(Vector3 firePosition, Vector3 targetPosition){
		Debug.Log ("targetPosition: "+ targetPosition);
		m_Cannonball.SetActive (true);
		m_Cannonball.transform.position = firePosition;
		m_CannonballController.GiveDamageAmount (m_Damage);
		m_CannonballController.SetEnemyLayer (m_EnemyTeamMask);
		StartCoroutine (m_CannonballController.BeginLifeTime ());
		m_CannonFireSound.Play ();
		m_CannonballRigidbody.velocity = calculateBallisticVelocity(firePosition, targetPosition);


	}

	private Vector3 calculateBallisticVelocity(Vector3 firePosition, Vector3 targetPosition){

		Vector3 fromFireToTarget = targetPosition - firePosition;
		Vector3 horizontalProjection = Vector3.ProjectOnPlane(fromFireToTarget,new Vector3(0f,1f,0f));
		float horizontalDistance = Vector3.Magnitude (horizontalProjection);
		float timeOfFlight = horizontalDistance / m_LaunchSpeed; //this part is unrealistic but should make it look nicer, the time of flight is determined as if it was just flying straight at the launch speed
		Vector3 horizontalVelocity = Vector3.Normalize(horizontalProjection) * m_LaunchSpeed;
		float verticalDistance = fromFireToTarget.y;
		Vector3 verticalVelocity = new Vector3 (0f, 1f, 0f) * (Physics.gravity.y * timeOfFlight / 2f - verticalDistance/timeOfFlight);
		return horizontalVelocity - verticalVelocity;
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
