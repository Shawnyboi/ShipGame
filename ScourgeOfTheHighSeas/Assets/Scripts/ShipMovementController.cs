using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//this class mostly controls information pertaining to the movement of the ship
public class ShipMovementController : MonoBehaviour {


	//These variables pertain to the automatic tracking of an enemy
	public ShipFireController m_ShipFireController;
	private bool m_CurrentlyTrackingTarget;
	private bool m_HaveCommandedDestination;

	private ShipAttributes m_ShipAttributes;

	//These variables are necessary to move the ship
	public Vector3 m_CurrentDestination;
	private float m_ShipSpeed;
	private float m_ShipTurningSpeed;
	private Rigidbody m_Rigidbody;
	public bool m_StandingGround;

	//these variables concern some ui the ship displays
	public Canvas m_SelectionCanvas;
	public GameObject m_Waypoint;
	public bool m_IsSelected;

	//this 
	private List<Vector3> m_SubWaypointList;

	//these variables pertain to the stun blocking function
	private float m_StunBlockTime;
	private bool m_IsStunBlocked;

	//this is a flag to prevent the turning coroutine from stacking
	private bool m_IsTurning;

	//caching the transform limits necesarry overhead b/c calling gameObject.transform is costly
	private Transform m_Transform;

	void Awake(){
		m_Transform = gameObject.GetComponent<Transform> ();
		m_ShipAttributes = gameObject.GetComponent<ShipAttributes> ();
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_ShipFireController = GetComponent<ShipFireController> ();

		m_CurrentDestination = m_Transform.position;
		m_SelectionCanvas.enabled = false;
		m_IsSelected = false;
		m_IsStunBlocked = false;
		m_Waypoint = Object.Instantiate (m_Waypoint);
		m_Waypoint.SetActive (false);
		m_IsTurning = false;
		m_CurrentlyTrackingTarget = false;

	}

	void Start(){
		m_ShipSpeed = m_ShipAttributes.m_Speed;
		m_ShipTurningSpeed = m_ShipAttributes.m_TurningSpeed;
		m_StunBlockTime = m_ShipAttributes.m_StunBlockTime;

	}

	//Used for physical calculations
	void FixedUpdate () {

		if (!m_IsStunBlocked) {
			if (Vector3.Magnitude (m_CurrentDestination - m_Transform.position) > 5f) { //if not at destination (5 is a magic number)
				Move ();

			}
		}
	}

	//Move toward current destination at the speed of the ship
	//Then check if you are close enough to the destination, if so set given destination flag to false
	//Can only move if turned in the right direction
	private void Move () {
		m_Rigidbody.MovePosition(m_Transform.position + m_Transform.forward * m_ShipSpeed * Time.deltaTime);
		if (Vector3.Magnitude (m_CurrentDestination - m_Transform.position) <= 6f) {// 6f is a magic number to let us know we are there
			SetCommandedDestinationFlag (false); //reached destination
		}
	}

	//Turn the bow of the ship toward the destination
	private void Turn () {	
		float angleToDest = Vector3.Angle (m_Transform.forward, m_CurrentDestination - m_Transform.position);
		Vector3 crossProduct = Vector3.Cross (m_Transform.forward, m_CurrentDestination - m_Transform.position);
		Quaternion deltaRotation;
		if (crossProduct.y < 0f) {
			deltaRotation = Quaternion.Euler (0f, -Time.smoothDeltaTime * m_ShipTurningSpeed, 0f); //turn to port

		} else {
			deltaRotation = Quaternion.Euler (0f, Time.smoothDeltaTime * m_ShipTurningSpeed, 0f); //turn to starboard

		}



		if (angleToDest > 10f) {//if not facing the right direction yet (10 is a magic number)
			m_Rigidbody.MoveRotation (m_Rigidbody.rotation * deltaRotation);

		}
	}

	/// <summary>
	/// Turn the bow of the ship in the direction of desired angle
	/// Note that only the sign of the angle really matters
	/// turns port for negative, starboard for positive
	/// </summary>
	/// <param name="desiredAngle">Desired angle.</param>
	private void Turn(float desiredAngle){
		Quaternion deltaRotation;
		if (desiredAngle < 0f) {
			deltaRotation = Quaternion.Euler (0f, -Time.smoothDeltaTime * m_ShipTurningSpeed, 0f); //turn to port

		} else {
			deltaRotation = Quaternion.Euler (0f, Time.smoothDeltaTime * m_ShipTurningSpeed, 0f); //turn to starboard

		}

		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * deltaRotation);

	}


	//Set a new destination as this objects current destination
	//also returns that point as a vector3
	public Vector3 SetDestination(Vector3 newDest){
		m_StandingGround = false;
		m_CurrentDestination = newDest;
		StartCoroutine (RenderWaypoint ());
		if (!m_IsTurning) {
			StartCoroutine (HandleTurning ());
		}
		return newDest;

	}

	/// <summary>
	/// This performs various things when the ship is selected (light up selection canvas and set selected flag to true)
	/// returns false and does nothing else if object was already selected
	/// </summary>
	public bool Select(){
		
		if (!m_IsSelected) {
			
			m_SelectionCanvas.enabled = true;
			m_IsSelected = true;
			return true;

		}

		return false;

	}

	//Do something when the ship is deselected by the player
	public void Deselect(){
		m_SelectionCanvas.enabled = false;
		m_IsSelected = false;
	}

	/*
	//For when the ship bumps into another ship it will become stun blocked and not be able to move for some time
	public IEnumerator StunBlock(Collider blockerCollider){

		if (blockerCollider == this.gameObject.GetComponent<ShipColliderController> ().m_TriggerCollider) {
			m_IsStunBlocked = true;
			this.gameObject.GetComponent<ShipStatusController> ().SetStunBlockStatus (true);
			m_CurrentDestination = m_Transform.position;		
			yield return new WaitForSeconds (m_StunBlockTime);
			m_IsStunBlocked = false;
			this.gameObject.GetComponent<ShipStatusController> ().SetStunBlockStatus (false);

		}
	}
	*/

	/// <summary>
	/// This coroutine will handle the turning concurrently to the movement in order to make it look smoother
	/// </summary>
	private IEnumerator HandleTurning(){
		while (Vector3.Magnitude (m_CurrentDestination - m_Transform.position) > 5) {
			m_IsTurning = true;		
			Turn ();
			yield return null;
		}
		m_IsTurning = false;
	}

	/// <summary>
	/// This coroutine turns the fire positions of the ship toward the target
	/// </summary>
	private IEnumerator HandleAttackTurning(){
		while (Mathf.Abs(m_ShipFireController.GetAttackAngle ()) > 1f) {//while angle between target and attack position are not zero (The 1f is a magic number appromiately equal to zero)
			m_IsTurning = true;
			Turn (m_ShipFireController.GetAttackAngle ());
			yield return null;
		}
		m_IsTurning = false;
	}


	/// <summary>
	/// This coroutine take care of displaying the waypoint at the correct position until the ship gets to it's position
	/// </summary>
	private IEnumerator RenderWaypoint (){
		m_Waypoint.transform.position = m_CurrentDestination;
		while (Vector3.Magnitude (m_CurrentDestination - m_Transform.position) > 5) {
			if (m_IsSelected) {
				m_Waypoint.SetActive (true);
			} else {
				m_Waypoint.SetActive (false);
			}
			yield return null;
		}
		m_Waypoint.SetActive (false);

	}

	/// <summary>
	/// this sets the commanded destination flag in the move controller. This flag is necesarry to give direct move command
	/// destinations precedence over automatic tracking destinations. This should be called in player controller when move command is given
	/// </summary>
	/// <param name="flag">If set to <c>true</c> m_HaveCommandedDestination set to true.</param>
	public void SetCommandedDestinationFlag(bool flag){
		m_HaveCommandedDestination = flag;
	}


	/// <summary>
	/// Determines whether this instance has commanded destination.
	/// </summary>
	/// <returns><c>true</c> if this instance has commanded destination; otherwise, <c>false</c>.</returns>
	public bool HasCommandedDestination(){
		return m_HaveCommandedDestination;
	}

	/// <summary>
	/// This coroutine will make it so that when a ship has a target it will automatically move toward it and turn to fire on it
	/// if it is called while already running it will do nothing
	/// </summary>
	public IEnumerator TrackTarget(){
		if (!m_CurrentlyTrackingTarget) {//checks to make sure the coroutine isn't already running
			SetDestination (m_ShipFireController.GetTargetPosition ());
			m_CurrentlyTrackingTarget = true;

			while (m_ShipFireController.CurrentlyHasTarget () && !this.m_HaveCommandedDestination && gameObject.activeSelf == true ) {

				if (!m_ShipFireController.TargetWithinRange ()) { //if we have a target and no commanded destination we want to automatically move toward the target
					SetDestination (m_ShipFireController.GetTargetPosition ());
					yield return null;

				} else { //if already in range, stop moving and turn to attack
					SetDestination (m_Transform.position);

					if (Mathf.Abs(m_ShipFireController.GetAttackAngle ()) > 1f) {//if attack angle is not approximately zero 1f is a magic number about equal to zero
						
						if (!m_IsTurning) {
							
							yield return StartCoroutine (HandleAttackTurning ());

						}
					}
					yield return null;
				}
				yield return null;

			}
			yield return null;
			m_CurrentlyTrackingTarget = false;//end coroutine and set running flag so it can be called again in the future
		} else {
			yield return null;//end coroutine without doing anything because it was already tracking
		}
	}


	/// <summary>
	/// This Coroutine tracks the target but it does not actually move the ship, only rotates it
	/// </summary>
	public IEnumerator DefensiveTracking(){

		if (!m_CurrentlyTrackingTarget) {
			m_CurrentlyTrackingTarget = true;
			while (m_ShipFireController.CurrentlyHasTarget () && !this.m_HaveCommandedDestination && gameObject.activeSelf == true && m_ShipFireController.TargetWithinRange()) {//if target or this become null end tracking. also end it if this object gets a move command
				
				if (Mathf.Abs (m_ShipFireController.GetAttackAngle ()) > 1f) {//if attack angle is not approximately zero 1f is a magic number about equal to zero

					if (!m_IsTurning) {
						yield return StartCoroutine (HandleAttackTurning ());
					}
					yield return null;
				} 
				yield return null;
			}
			yield return null;
			m_ShipFireController.SetTargetToNull (); // set target to null when it goes out of range
			m_CurrentlyTrackingTarget = false;//set this flag false when exiting coroutine
		} else {
			yield return null;//if already tracking go throught this CR without doing anything
		}
	}


}
