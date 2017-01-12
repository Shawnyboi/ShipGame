using UnityEngine;
using System.Collections;

//this class is defunct
public class ShipController : MonoBehaviour {


	public Vector3 m_CurrentDestination;
	public float m_ShipSpeed = 10f;
	public float m_ShipTurningSpeed = 10f;
	public Canvas m_SelectionCanvas;

	private Rigidbody m_Rigidbody;
	//private bool m_IsSelected;

	void Awake(){
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_CurrentDestination = transform.position;
		m_SelectionCanvas.enabled = false;
		//m_IsSelected = false;


	}


	
	// Update is called once per frame
	void Update () {
	
	}

	//Used for physical calculations
	void FixedUpdate () {

		if (Vector3.Magnitude(m_CurrentDestination - transform.position) > 1) { //This is a vector 3 so it can never be null we might make it so that the destination isn't a vector 3 in the future or get rid of this check
			Move ();
			Turn ();
		}

	}

	//Move toward current destination at the speed of the ship
	//Then check if you are close enough to the destination, if so set current destination to null
	//Can only move if turned in the right direction
	private void Move () {
		float angleToDest = Vector3.Angle (transform.forward, m_CurrentDestination - transform.position);//The vector from the ship to dest(second parameter) is destination position - ship position
		if(Mathf.Abs(angleToDest) < 10f){//the bow angle must be within a certain bound of the direction angle for the ship to move	
			m_Rigidbody.MovePosition(transform.position + transform.forward * m_ShipSpeed * Time.deltaTime);
		}
	}

	//Turn the bow of the ship toward the destination
	//takes target angle as a parameter
	private void Turn () {	
		float angleToDest = Vector3.Angle (transform.forward, m_CurrentDestination - transform.position);
		Vector3 crossProduct = Vector3.Cross (transform.forward, m_CurrentDestination - transform.position);
		float signedAngleToDest;
		if (crossProduct.y < 0f) {
			signedAngleToDest = -angleToDest;
		} else {
			signedAngleToDest = angleToDest;
		}

		Quaternion deltaRotation = Quaternion.Euler (0f, signedAngleToDest * Time.deltaTime, 0f);


		if (angleToDest > 10f) {
			m_Rigidbody.MoveRotation (m_Rigidbody.rotation * deltaRotation);
		}
	}
		

	//Set a new destination as this objects current destination
	//also returns that point as a vector3
	public Vector3 SetDestination(Vector3 newDest){

		m_CurrentDestination = newDest;

		return newDest;

	}

	//Do something when the ship is selected by the player
	public void Select(){
		m_SelectionCanvas.enabled = true;
		//m_IsSelected = true;
	}

	//Do something when the ship is deselected by the player
	public void Deselect(){
		m_SelectionCanvas.enabled = false;
		//m_IsSelected = false;
	}
}
