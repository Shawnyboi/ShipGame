﻿using UnityEngine;
using System.Collections;

//this class controlls information pertaining to the ships firing mechanism
public class ShipFireController : MonoBehaviour {

	public Transform[] m_PortFirePositions;
	public Transform[] m_StarboardFirePositions;

	protected ShipAttributes m_ShipAttributes;

	public GameObject m_Cannonball;
	protected float m_Range;
	protected float m_Damage;
	protected float m_ReloadTime;
	protected float m_LaunchSpeed;
	protected float m_PercentAccuracy;
	protected float m_ShotVarianceDegrees;
	public AudioSource m_CannonFireSound;

	protected GameObject m_Target;
	protected bool m_Attacking;

	protected string m_Disposition;
	protected LayerMask m_EnemyTeamMask;

	protected Transform m_Transform;

	virtual protected void Awake(){
		m_Transform = gameObject.GetComponent<Transform> ();
		m_ShipAttributes = gameObject.GetComponent<ShipAttributes> ();
		m_Disposition = "aggressive";
		m_Attacking = false;
		m_Cannonball = Instantiate (m_Cannonball);
		m_Cannonball.SetActive (false);


	}

	protected void Start(){

		m_ShotVarianceDegrees = m_ShipAttributes.m_ShotVarianceDegrees;
		m_PercentAccuracy = m_ShipAttributes.m_PercentAccuracy;
		m_Range = m_ShipAttributes.m_Range;
		m_Damage = m_ShipAttributes.m_Damage;
		m_ReloadTime = m_ShipAttributes.m_ReloadTime;
		m_LaunchSpeed = m_ShipAttributes.m_LaunchSpeed;
		
		if(gameObject.layer == LayerMask.NameToLayer("PlayerShips")){
			
			m_EnemyTeamMask = 1 << LayerMask.NameToLayer("ComShips");

		}else if(gameObject.layer == LayerMask.NameToLayer("ComShips")){
			
			m_EnemyTeamMask = 1 << LayerMask.NameToLayer("PlayerShips"); //We make the enemy team mask a layer mask for the team that is not the team of this object

		}
	}
	
	// Update is called once per frame
	protected void Update () {
		if (m_Target != null) {
			
			if (m_Target.activeSelf == false) {
				Debug.Log ("Setting target to null");

				SetTargetToNull (); //If we ever find our target has become inactive we set the pointer to null

			}
		}

		if (m_Target == null) {
			
			ScanForEnemy ();

		}
		
	
	}

	//This function will be called by the PlayerController when this ship is selected to give it 
	//a ship to target
	public void SetNewTarget(GameObject newTarget){
		m_Target = newTarget;
		if (!m_Attacking) {
			StartCoroutine (Attack (newTarget));
		}
	}

	/// <summary>
	/// Sets the disposition of this ship. Choices are defensive, aggressive, and peaceful
	/// //passing in a string  other than defensive, aggressive or peaceful will do nothing
	/// </summary>
	public void SetDisposition(string disposition){
		if ((disposition == "defensive" || disposition == "aggressive" || disposition == "peaceful")) {
			m_Disposition = disposition;
		}
	}

	/// <summary>
	/// This function which is caled in update if the ship has no target detects if there is 
	/// an enemy within range and then acts based on the current disposition of this ship (Aggressive, Defensive, Peaceful)
	/// </summary>
	protected void ScanForEnemy(){
		if (m_Disposition != "peaceful") {
			if (m_Target == null) {
				
				Collider[] enemyCollidersInRange = Physics.OverlapSphere (m_Transform.position, m_Range, m_EnemyTeamMask); //Detect all enemy ships' colliders within range

				if(enemyCollidersInRange.Length > 0){
					
					SetNewTarget(enemyCollidersInRange [0].gameObject);//if there is at least one get the first one detected and set it as this objects target

					if (m_Disposition == "aggressive" && !gameObject.GetComponent<ShipMovementController> ().HasCommandedDestination ()) {
						
						StartCoroutine (gameObject.GetComponent<ShipMovementController> ().TrackTarget ()); //if we are in the agressive diposition we go after the target

					} else if (m_Disposition == "defensive" && !gameObject.GetComponent<ShipMovementController> ().HasCommandedDestination ()) {

						StartCoroutine (gameObject.GetComponent<ShipMovementController> ().DefensiveTracking ()); //if we are defensive just track the target wthout moving

					}
				}

			}

		}


	}

	//Fire the cannonball toawrd target position
	//returns true if cannon fired
	protected virtual bool Fire(Vector3 targetPosition){

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

		//fire the cannonball
		if(Vector3.Distance(m_Transform.position, targetPosition) < m_Range){
			
			if (!m_Cannonball.GetComponent<CannonballController> ().m_IsAlive) {

				LaunchCannonball (firePosition, targetPosition);

				return true;
			}
			return false; //Target not within range
		}
		return false; //Cannonball already fired and isn't gone yet
	}

	/// <summary>
	/// Call this function to take care of everything that comes with firing the cannonball
	/// </summary>
	/// <param name="firePosition">Fire position.</param>
	/// <param name="targetPosition">Target position.</param>
	protected virtual void LaunchCannonball(Vector3 firePosition, Vector3 targetPosition){

		m_Cannonball.SetActive (true);
		m_Cannonball.transform.position = firePosition;
		m_Cannonball.GetComponent<CannonballController> ().GiveDamageAmount (m_Damage);
		m_Cannonball.GetComponent<CannonballController> ().SetEnemyLayer (m_EnemyTeamMask);
		StartCoroutine (m_Cannonball.GetComponent<CannonballController> ().BeginLifeTime ());
		Vector3 trajectory = CalculateTrajectory (targetPosition, firePosition);
		trajectory = VariateTrajectory (trajectory);
		m_CannonFireSound.Play ();
		m_Cannonball.GetComponent<Rigidbody> ().velocity = m_LaunchSpeed * trajectory;

	}

	//This function determines which side of the ship the target is on and returns the side as a string
	//It uses the difference of the forward angle and the position of the target relative to te ship
	//there are 4 sides, the bow, the stern, the port, and the starboard each occupying 90 degree arcs around
	//the four cardinal directions relative to the ship
	protected string DetermineTargetOrientation(Vector3 targetPosition){

		float signedAngle;
		float unsignedAngle = Vector3.Angle (m_Transform.forward, targetPosition - m_Transform.position);
		Vector3 crossProduct = Vector3.Cross (m_Transform.forward, targetPosition - m_Transform.position);

		if (crossProduct.y > 0) {
			
			signedAngle = -unsignedAngle;

		} else {
			
			signedAngle = unsignedAngle;
		}

		if (unsignedAngle <= 45f) {
			
			return "bow";

		} else if (unsignedAngle >= 135f) {
			
			return "stern";

		} else if (signedAngle > 45f && signedAngle < 135f) {
			
			return "port";

		} else if (signedAngle < -45f && signedAngle > -135f) {
			
			return "starboard";

		}else{
			
			return "";//this should never be returned
		}

	}


	//This function is called to determine which direction to set the cannonball's velocity
	//Note that since all we want is the direction the return vector is normalized
	protected  Vector3 CalculateTrajectory(Vector3 targetPosition, Vector3 firePosition){
		
		Vector3 trajectory = targetPosition - firePosition;
		trajectory.y += 1; //We raise the cannonball a little when it fire, makes it look nicer
		trajectory = Vector3.Normalize (trajectory);
		return trajectory;
	}

	/// <summary>
	/// This function is called to subject the ships shots to some variance;
	/// Randomly a shot will misfire and be shot in the wrong direction
	/// </summary>
	/// <returns>The variated trajectory.</returns>
	/// <param name="originalTrajectory">Original trajectory.</param>
	protected Vector3 VariateTrajectory(Vector3 originalTrajectory){
		if (Random.Range (0f, 100f) > m_PercentAccuracy) {
			Quaternion randomRotation = Quaternion.Euler (Random.Range (-m_ShotVarianceDegrees, m_ShotVarianceDegrees), Random.Range (m_ShotVarianceDegrees, m_ShotVarianceDegrees), 0f);
			return randomRotation * originalTrajectory;
		} else {
			return originalTrajectory;
		}
	}



	/// <summary>
	/// This Coroutine will take care of determining when to fire 
	/// It also puts a delay in between shots equal to the reload time
	/// </summary>
	protected IEnumerator Attack(GameObject newTarget){
		
		m_Attacking = true;

		while (m_Target != null) {
			
			if (m_Target == null) {
				break; //if the target becomes null in the middle of the attack we want to break out of the while loop
			}

			if (Fire (m_Target.transform.position)) {
				   
				yield return new WaitForSeconds (m_ReloadTime);
			}

			yield return null;
		
		}

		yield return null;
		m_Attacking = false;
	}

	/// <summary>
	/// Sets the target of this ship to null.
	/// </summary>
	public void SetTargetToNull(){
		Debug.Log ("Setting target to null");

		this.m_Target = null;

	}

	/// <summary>
	/// this lets other classes know if the ship has a target
	/// </summary>
	/// <returns><c>true</c>, if has target currently <c>false</c> otherwise.</returns>
	public bool CurrentlyHasTarget(){
		
		if (m_Target != null) {
			
			return true;

		} else {
			
			return false;

		}
	}

	/// <summary>
	/// this function can be called to determine if the target is within the range
	/// </summary>
	/// <returns><c>true</c>, if target is within range, <c>false</c> otherwise.</returns>
	public bool TargetWithinRange(){
		
		if (m_Target != null) {
			
			return (Vector3.Distance (this.m_Transform.position, m_Target.transform.position) < m_Range);

		} else {
			
			return false;

		}
	}

	/// <summary>
	/// this lets other controllers know the targets position
	/// if there for some reason is no target it returns its own position
	/// </summary>
	public Vector3 GetTargetPosition(){
		
		if(m_Target != null){
			
			return m_Target.transform.position;

		}else{
			
			return this.m_Transform.position;

		}
	}

	/// <summary>
	/// returns the angle between the closest attacking side  and the target
	/// </summary>
	public virtual float GetAttackAngle(){
		
		if (m_Target != null) {
			
			float signedAngle;
			float unsignedAngle = Vector3.Angle (m_Transform.forward, m_Target.transform.position - m_Transform.position);
			Vector3 crossProduct = Vector3.Cross (m_Transform.forward, m_Target.transform.position - m_Transform.position);

			if (crossProduct.y > 0) {
				
				signedAngle = -unsignedAngle;

			} else {
				
				signedAngle = unsignedAngle;

			}

			if (signedAngle > 0) { // closer to starboard (90 degrees)
				
				return 90f - signedAngle;

			} else { //closer to port (-90 degrees)
				
				return -90f - signedAngle;

			}

		} else {
			
			return 0f;

		}
			

	}
}
