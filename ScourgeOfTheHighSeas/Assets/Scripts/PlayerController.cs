using UnityEngine;
using System.Collections;

//this ship handles the players actions
public class PlayerController : MonoBehaviour {

	//The player controller must be able to access the level manager of each level so that 
	//it can be told which ships belong to it
	public LevelManager m_LevelManager;

	//public GameObject[] m_SelectedObjectArray;
	public GameObject m_SelectedObject;

	//The player controls the camera with its input so it is necessarry to have a reference to it
	public Camera m_PlayerCamera;

	//these variables pertain to the play pause functionality of the game
	private bool m_VirtuallyPaused;
	private float m_VirtualPauseTimeScaleFactor;


	private void Awake(){
		m_VirtualPauseTimeScaleFactor = .0001f;
		m_PlayerCamera = Camera.main;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			SelectObject ();
		} else if (Input.GetMouseButtonUp (1)) {
			if (GiveActionCommand ()) {
				//action command given
			} else {
				GiveMoveCommand ();
			}
		}

		if (Input.GetKeyDown("escape")){
			Application.Quit();
		}

		if (Input.GetKeyDown("p")){
			if(!m_VirtuallyPaused){
				VirtualPause (true);
			}else{
				VirtualPause (false);
			}
		}

		if (Input.GetKeyDown ("z")) {
			SetSelectedObjectDisposition ("aggressive");
		}

		if (Input.GetKeyDown ("x")) {
			SetSelectedObjectDisposition ("defensive");
		}

		if (Input.GetKeyDown ("c")) {
			SetSelectedObjectDisposition ("peaceful");
		}
	
	}

	//first deselect currently selected object
	//Send out a ray and check if it collides with a selectable object like a ship
	//Store the first hit object in this Player's m_Selected Object
	public bool SelectObject(){

		if (m_SelectedObject){
			if (m_SelectedObject.layer == LayerMask.NameToLayer ("PlayerShips")) {
				m_SelectedObject.GetComponent<ShipMovementController> ().Deselect ();//Deselect currently selected ship
			}
		}
			


		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);    /*the ray is sent from the camera to the mouse*/
		RaycastHit hitObject;                                                       /*the raycast hit object contains info about the object hit*/

		if (Physics.Raycast(rayToObject, out hitObject)){
			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("PlayerShips")) {//check if hit object is a player ship by checking the layer its on
				m_SelectedObject = hitObject.collider.gameObject;
				m_SelectedObject.GetComponent<ShipMovementController> ().Select ();
				return true;//ship hit by raycast and stored in selectedObject
			} else {
				m_SelectedObject = null;
				return false;//ship not hit, not stored
			}
		} else {
			m_SelectedObject = null;
			return false;//ship not hit, not stored
		}
	}


	//Send out a ray and see if it collides with an actionable object
	//If so give corresponding command, if not return false.
	private bool GiveActionCommand(){
		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);    /*the ray is sent from the camera to the mouse*/
		RaycastHit hitObject;                                                       /*the raycast hit object contains info about the object hit*/

		if (Physics.Raycast (rayToObject, out hitObject)) {
			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("ComShips")) {//check if hit object is an enemy ship by checking the layer its on
				if(m_SelectedObject != null){


					GameObject targettedObject = hitObject.collider.gameObject;//get right clicked on object
					StartCoroutine(targettedObject.GetComponent<ShipColliderController>().BlinkAttackedIndicator());

					m_SelectedObject.GetComponent<ShipFireController> ().SetNewTarget (targettedObject);//give selected object that object as a target
					m_SelectedObject.GetComponent<ShipMovementController> ().SetDestination (m_SelectedObject.transform.position);
					m_SelectedObject.GetComponent<ShipMovementController> ().SetCommandedDestinationFlag (false);
					StartCoroutine (m_SelectedObject.GetComponent<ShipMovementController> ().TrackTarget ());


					return true; //When true is returned a move command will not be given
				}
				return false; //right clicked on ship but had no selected object
			}
			return false;//right clicked on something that wasn't a ship

		}

		return false;//didn't right click on anything
	}
		




	/// <summary>
	/// Send out ray and see if it collides with the field Set the x,z point of collision as the destination of the selected object
	/// </summary>
	/// <returns><c>true</c>, if move command was given, <c>false</c> otherwise.</returns>
	private bool GiveMoveCommand(){

		Ray rayToField = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitField;

		if (Physics.Raycast (rayToField, out hitField)) {
			if (hitField.collider.gameObject.layer == LayerMask.NameToLayer ("Field")) {
				if (m_SelectedObject) {
					ShipMovementController ship = m_SelectedObject.GetComponent<ShipMovementController>();
					if (ship) {	
						ship.SetDestination (hitField.point); //if we clicked on the field and we have a ship selected, we tell that ship to go to the destination.
						ship.GetComponent<ShipMovementController>().SetCommandedDestinationFlag(true); //we let it know it has got a direct move command
						return true;
					} else {
						return false;//selected object is not a ship
					}
				} else {
					return false;//no selected object
				}
			} else {
				return false;//raycast hit something that wasn't a field
			}

		} else {
			return false;//raycast hit nothing
		}
	}

	/// <summary>
	/// Pauses so that the game objects do not move around but commands can stll be given and ships still be selected
	/// </summary>
	/// <param name="pausing">If set to <c>true</c> pause.</param>
	private void VirtualPause(bool pausing){
		if (pausing) {
			m_VirtuallyPaused = true;
			Time.timeScale *= m_VirtualPauseTimeScaleFactor;
			//Time.fixedDeltaTime *= m_VirtualPauseTimeScaleFactor;
			//m_PlayerCamera.gameObject.GetComponent<CameraController> ().CompensateForVirtualPause (1f / m_VirtualPauseTimeScaleFactor);
		} else {
			m_VirtuallyPaused = false;
			Time.timeScale = 1f;
			//Time.fixedDeltaTime *= 1f / m_VirtualPauseTimeScaleFactor;
			//m_PlayerCamera.gameObject.GetComponent<CameraController> ().CompensateForVirtualPause (m_VirtualPauseTimeScaleFactor);
		}
	}

	/// <summary>
	/// Sets the diposition of the selected object, will be called due to user input
	/// </summary>
	/// <param name="disposition">Disposition to be set. It can be either "defensive", "aggressive", or "peaceful"</param>
	private void SetSelectedObjectDisposition(string disposition){
		if (m_SelectedObject != null) {
			m_SelectedObject.GetComponent<ShipFireController> ().SetDisposition (disposition);
		}
	}

}
