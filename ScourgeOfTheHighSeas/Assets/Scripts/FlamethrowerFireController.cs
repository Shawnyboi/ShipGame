using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerFireController : ShipFireController {

	public GameObject m_FireStream;
	public GameObject m_Fireball;

	override protected void Awake(){
		base.Awake ();
		m_FireStream = Instantiate (m_FireStream);
		m_FireStream.SetActive (false);
	}

	/// <summary>
	/// This is a slight alteration to the base function so that we fire a firestream insteaad of a fireball
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	override protected bool Fire(Vector3 targetPosition){
		
		string targetPositionOrientation = DetermineTargetOrientation (targetPosition);
		Vector3 firePosition;

		if (targetPositionOrientation == "starboard") {

			if (m_StarboardFirePositions != null) {

				firePosition = m_StarboardFirePositions [Random.Range (0, m_StarboardFirePositions.Length)].position; //we get a fire position to shoot from on starboard

			} else {

				return false;//no starboard fire positions

			}

		} else if (targetPositionOrientation == "port") {

			if (m_PortFirePositions != null) {

				firePosition = m_PortFirePositions [Random.Range (0, m_PortFirePositions.Length)].position; //we get a fire position to shoot from on port

			} else {

				return false;//no port fire positions
			}

		} else {

			return false; //orientation not port or starboard so no firing is possible
		}

		//fire the firestream
		if(Vector3.Distance(m_Transform.position, targetPosition) < m_Range){

			StartCoroutine(LaunchFireStream (firePosition, targetPosition));
			return true;

		}
		return false; //Target not within range
	}

	private IEnumerator LaunchFireStream (Vector3 firePosition, Vector3 targetPosition){

		Vector3 vectorToTarget = targetPosition - firePosition;
		Quaternion lookRotation = Quaternion.LookRotation (vectorToTarget);
		m_FireStream.transform.position = firePosition;
		m_FireStream.transform.rotation = lookRotation;
		m_CannonFireSound.Play ();
		m_FireStream.SetActive (true);
		yield return new WaitForSeconds (1.0f);
		Instantiate (m_Fireball, targetPosition, Quaternion.identity);
		yield return new WaitForSeconds (1.0f);
		m_FireStream.SetActive (false);

	}
}
	


