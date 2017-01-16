using UnityEngine;
using System.Collections;

//this class controlls information pertaining to the ships firing mechanism
public class ShipFireController : MonoBehaviour {

	public Transform[] m_PortFirePositions;
	public Transform[] m_StarboardFirePositions;

	public GameObject m_Cannonball;
	private float m_Range;
	private float m_Damage;
	private float m_ReloadTime;
	private float m_LaunchSpeed;
	private float m_PercentAccuracy;
	private float m_ShotVarianceDegrees;
	public AudioSource m_CannonFireSound;

	private GameObject m_Target;
	private bool m_Attacking;

	private string m_Disposition;
	private LayerMask m_EnemyTeamMask;

	void Awake(){
		m_ShotVarianceDegrees = gameObject.GetComponent<ShipAttributes> ().m_ShotVarianceDegrees;
		m_PercentAccuracy = gameObject.GetComponent<ShipAttributes>().m_PercentAccuracy;
		m_Range = gameObject.GetComponent<ShipAttributes> ().m_Range;
		m_Damage = gameObject.GetComponent<ShipAttributes> ().m_Damage;
		m_ReloadTime = gameObject.GetComponent<ShipAttributes> ().m_ReloadTime;
		m_LaunchSpeed = gameObject.GetComponent<ShipAttributes> ().m_LaunchSpeed;
		m_Disposition = "aggressive";
		m_Attacking = false;
		m_Cannonball = Instantiate (m_Cannonball);
		m_Cannonball.SetActive (false);


	}

	void Start(){
		
		if(gameObject.layer == LayerMask.NameToLayer("PlayerShips")){
			
			m_EnemyTeamMask = 1 << LayerMask.NameToLayer("ComShips");

		}else if(gameObject.layer == LayerMask.NameToLayer("ComShips")){
			
			m_EnemyTeamMask = 1 << LayerMask.NameToLayer("PlayerShips"); //We make the enemy team mask a layer mask for the team that is not the team of this object

		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (m_Target != null) {
			
			if (m_Target.activeSelf == false) {
				
				m_Target = null; //If we ever find our target has become inactive we set the pointer to null

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
			StartCoroutine (Attack ());
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
	private void ScanForEnemy(){
		if (m_Disposition != "peaceful") {
			if (m_Target == null) {
				
				Collider[] enemyCollidersInRange = Physics.OverlapSphere (this.gameObject.transform.position, m_Range, m_EnemyTeamMask); //Detect all enemy ships' colliders within range

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
	private bool Fire(Vector3 targetPosition){

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
		if(Vector3.Distance(gameObject.transform.position, targetPosition) < m_Range){
			
			if (!m_Cannonball.GetComponent<CannonballController> ().m_IsAlive) {
				
				m_Cannonball.SetActive (true);
				m_Cannonball.transform.position = firePosition;
				m_Cannonball.GetComponent<CannonballController> ().GiveDamageAmount (m_Damage);
				StartCoroutine (m_Cannonball.GetComponent<CannonballController> ().BeginLifeTime ());
				Vector3 trajectory = CalculateTrajectory (targetPosition, firePosition);
				trajectory = VariateTrajectory (trajectory);
				m_CannonFireSound.Play ();
				m_Cannonball.GetComponent<Rigidbody> ().velocity = m_LaunchSpeed * trajectory;

				return true;
			}
			return false; //Target not within range
		}
		return false; //Cannonball already fired and isn't gone yet


		

	}

	//This function determines which side of the ship the target is on and returns the side as a string
	//It uses the difference of the forward angle and the position of the target relative to te ship
	//there are 4 sides, the bow, the stern, the port, and the starboard each occupying 90 degree arcs around
	//the four cardinal directions relative to the ship
	private string DetermineTargetOrientation(Vector3 targetPosition){

		float signedAngle;
		float unsignedAngle = Vector3.Angle (gameObject.transform.forward, targetPosition - gameObject.transform.position);
		Vector3 crossProduct = Vector3.Cross (gameObject.transform.forward, targetPosition - gameObject.transform.position);

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
	private Vector3 CalculateTrajectory(Vector3 targetPosition, Vector3 firePosition){
		
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
	private Vector3 VariateTrajectory(Vector3 originalTrajectory){
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
	private IEnumerator Attack(){
		
		m_Attacking = true;

		while (m_Target != null) {
			
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
			
			return (Vector3.Distance (this.gameObject.transform.position, m_Target.transform.position) < m_Range);

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
			
			return this.gameObject.transform.position;

		}
	}

	/// <summary>
	/// returns the angle between the closest attacking side  and the target
	/// </summary>
	public float GetAttackAngle(){
		
		if (m_Target != null) {
			
			float signedAngle;
			float unsignedAngle = Vector3.Angle (gameObject.transform.forward, m_Target.transform.position - gameObject.transform.position);
			Vector3 crossProduct = Vector3.Cross (gameObject.transform.forward, m_Target.transform.position - gameObject.transform.position);

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
