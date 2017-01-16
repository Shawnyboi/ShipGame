using UnityEngine;
using System.Collections;

//this ship handles the players actions
public class PlayerController : MonoBehaviour {

	//The player controller must be able to access the level manager of each level so that 
	//it can be told which ships belong to it
	public LevelManager m_LevelManager;

	//the array containing the objects selected by the player and a tally of its
	public GameObject[] m_SelectedObjectArray;
	private int m_NumSelectedObjects;


	//The player controls the camera with its input so it is necessarry to have a reference to it
	public Camera m_PlayerCamera;

	//these variables pertain to the play pause functionality of the game
	private bool m_VirtuallyPaused;
	private float m_VirtualPauseTimeScaleFactor;

	//These variables deal with the selection box function
	public Canvas m_SelectionBoxCanvas;
	public RectTransform m_SelectionBoxPanelTransform;
	private bool m_SelectionBoxOn;

	//This is the layer mask for the ships on this team
	private LayerMask m_TeamLayerMask;

	//this is how far apart the ships set themselves when in formation
	public float m_ShipSeparation;




	private void Awake(){
		m_VirtualPauseTimeScaleFactor = .0001f;
		m_PlayerCamera = Camera.main;
		m_TeamLayerMask = 1 << LayerMask.NameToLayer ("PlayerShips");
		m_SelectionBoxOn = false;
		m_SelectionBoxCanvas.enabled = false;
		m_ShipSeparation = 10f;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButton(0)){

			StartCoroutine (SelectionBox ());
		
		} else if (Input.GetMouseButtonUp (0)) {
			
			if (Input.GetKey ("left shift")) { //If holding the shift button down we can select multiple objects
				
				SelectObject (true); //shift select = true

			} else {
				
				SelectObject (false);//shift select = false

			}

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

	/// <summary>
	///first deselect currently selected object 
	/// Send out a ray and check if it collides with a selectable object like a ship
	///  Store the first hit object in this Player's m_Selected Object
	/// </summary>
	/// <returns><c>true</c>, if object was selected, <c>false</c> otherwise.</returns>
	/// <param name="shiftSelect">If set to <c>true</c> shift select was used.</param>
	public bool SelectObject(bool shiftSelect = false){

		if (HasSelectedObject() && !shiftSelect){
			
			DeselectAllObjects ();

		}
			


		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);    /*the ray is sent from the camera to the mouse*/
		RaycastHit hitObject;                                                       /*the raycast hit object contains info about the object hit*/

		if (Physics.Raycast(rayToObject, out hitObject)){
			
			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("PlayerShips")) {//check if hit object is a player ship by checking the layer its on
				
				if (!shiftSelect) {
					
					m_SelectedObjectArray [0] = hitObject.collider.gameObject;
					m_SelectedObjectArray [0].GetComponent<ShipMovementController> ().Select ();
					m_NumSelectedObjects = 1;
					return true;//ship hit by raycast and stored in SelectedObjectArray

				} else if (m_NumSelectedObjects < m_SelectedObjectArray.Length) { //If we use shift select then we add the object to the selected array as oppose to just making it the only selected object
					
					if (hitObject.collider.gameObject.GetComponent<ShipMovementController>().m_IsSelected == false) {
						
						m_SelectedObjectArray [m_NumSelectedObjects] = hitObject.collider.gameObject;
						m_SelectedObjectArray [m_NumSelectedObjects].GetComponent<ShipMovementController> ().Select ();
						m_NumSelectedObjects += 1;
						return true;//object shift selected

					} else {
						
						return false;//object shift selected but was already selected

					}
				}else{
					return false; //shift selected but object already selected
				}

			} else {
				
				DeselectAllObjects ();
				return false;//object hit with raycast but not a ship that belongs to the player

			}

		} else {
			
			DeselectAllObjects ();
			return false;//hit nothing with raycast

		}
	}

	/// <summary>
	/// This function uses an overlap box to select multiple object at once.
	/// It will be called after hold clicking and dragging a box on the screen
	/// </summary>
	/// <param name="originalMousePosition">Original mouse position in world space.</param>
	/// <param name="finalMousePosition">Final mouse position in world space.</param>
	private void BoxSelect(Vector3 originalMousePosition, Vector3 finalMousePosition){
		Collider[] hitShipColliders;
		Vector3 boxCenter = (finalMousePosition + originalMousePosition) / 2f; //the avg of the two mouse points
		//boxCenter.z = 0; // the z coord must be where the camera is

		/*the overlap box extents are basically the width and height of the moused box and then extended as far as the camera in the z direction*/
		Vector3 overlapBoxExtents = new Vector3 (Mathf.Abs ((finalMousePosition.x - originalMousePosition.x) ), Mathf.Abs ((finalMousePosition.y - originalMousePosition.y)), Camera.main.farClipPlane);


		hitShipColliders = Physics.OverlapBox (boxCenter, overlapBoxExtents, Camera.main.transform.rotation, m_TeamLayerMask);
		//GameObject cube = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),boxCenter, Camera.main.transform.rotation) as GameObject;
		//cube.transform.localScale = overlapBoxExtents;

		DeselectAllObjects ();
		for (int i = 0; i < hitShipColliders.Length; i++){
			if (!hitShipColliders [i].isTrigger) { //each ship has two colliders, we want the non trigger one so as to not count each ship twice
				m_SelectedObjectArray [m_NumSelectedObjects] = hitShipColliders [i].gameObject;
				m_SelectedObjectArray [m_NumSelectedObjects].GetComponent<ShipMovementController> ().Select ();
				m_NumSelectedObjects += 1;
			}
				
		}		
	}


	/// <summary>
	/// returns true if there is at least one selected object. THis is determined by looping throught the array
	/// </summary>
	/// <returns><c>true</c> if this instance has selected object; otherwise, <c>false</c>.</returns>
	private bool HasSelectedObject(){
		if (m_NumSelectedObjects == 0) {
			return false;
		} else {
			return true;
		}
	}

	/// <summary>
	/// This sets the length of the selected object array
	/// should be set to the num of ships in the level
	/// </summary>
	public void SetMaxNumShips(int numShips){
		m_SelectedObjectArray = new GameObject [numShips];

	}

	/// <summary>
	/// When this is called it will loop through the selected object array and set all objects to null
	/// </summary>
	private void DeselectAllObjects(){
		
		m_NumSelectedObjects = 0;

		for (int i = 0; i < m_SelectedObjectArray.Length; i++) {
			
			if (m_SelectedObjectArray [i] != null) {
				m_SelectedObjectArray [i].GetComponent<ShipMovementController> ().Deselect ();
				m_SelectedObjectArray [i] = null;

			}
		}
	}

	/// <summary>
	/// This function will check if we have inactive objects in our selected object array and if so set them to nulls
	/// This is becasue having inactive ships selected causes errors
	/// After this we will reset the array so all objects are at the front
	/// </summary>
	public void DeselectDeadShips(){
		
		for (int i = 0; i <m_SelectedObjectArray.Length; i++) { //loop through and set all inactive objects to false
			
			if (m_SelectedObjectArray [i] != null) {
				
				if (m_SelectedObjectArray [i].activeSelf == false) {
					
					m_SelectedObjectArray [i] = null;
					m_NumSelectedObjects -= 1;
				}
			}
		}

		GameObject[] tempArray = new GameObject[m_SelectedObjectArray.Length]; //Set up an empty array that we can place non null objects into
		int tempArrayIndex = 0;

		for (int i = 0; i < m_SelectedObjectArray.Length; i++) { //Loop through and copy all noon null objects into the front of the temp array
			
			if (m_SelectedObjectArray [i] != null) {
				
				tempArray [tempArrayIndex] = m_SelectedObjectArray [i];
				tempArrayIndex++;
			}
		}

		m_SelectedObjectArray = tempArray; //Make the temp array our new m_SelectedObjectArray

	}


	/// <summary>
	/// Send out a ray and see if it collides with an actionable object If so give corresponding command, if not return false.
	/// </summary>
	/// <returns><c>true</c>, if action command was given, <c>false</c> otherwise.</returns>
	private bool GiveActionCommand(){
		
		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);    /*the ray is sent from the camera to the mouse*/
		RaycastHit hitObject;                                                       /*the raycast hit object contains info about the object hit*/

		if (Physics.Raycast (rayToObject, out hitObject)) {
			
			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("ComShips")) {//check if hit object is an enemy ship by checking the layer its on
				
				if(HasSelectedObject()){


					GameObject targettedObject = hitObject.collider.gameObject;//get right clicked on object
					StartCoroutine(targettedObject.GetComponent<ShipColliderController>().BlinkAttackedIndicator());

					for (int i = 0; i < m_NumSelectedObjects; i++) {

						m_SelectedObjectArray[i].GetComponent<ShipFireController> ().SetNewTarget (targettedObject);//give selected object that object as a target
						m_SelectedObjectArray[i].GetComponent<ShipMovementController> ().SetDestination (m_SelectedObjectArray[i].transform.position);//cancel current destination
						m_SelectedObjectArray[i].GetComponent<ShipMovementController> ().SetCommandedDestinationFlag (false);
						StartCoroutine (m_SelectedObjectArray[i].GetComponent<ShipMovementController> ().TrackTarget ());//start tracking
					
					}

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
				
				if (HasSelectedObject()) {

					for (int i = 0; i < m_NumSelectedObjects; i++) { //loop through all selected ships
						
						ShipMovementController ship = m_SelectedObjectArray[i].GetComponent<ShipMovementController> ();

						if (ship) {	
							ship.SetDestination(DetermineFormationDestination(hitField.point, i));//if we clicked on the field and we have a ship selected, we tell that ship to go to the destination based on the formation
							ship.GetComponent<ShipMovementController> ().SetCommandedDestinationFlag (true); //we let it know it has got a direct move command
						}
					}
					return true; //return true cause move command was given


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
	/// Determines where to set a ship destination based on where it belongs in a formation of several ships
	/// Current implementation only allows for horizontal line formation
	/// </summary>
	/// <param name="selectedPoint">Selected point.</param>
	/// <param name="indexInArray">Index in array.</param>
	private Vector3 DetermineFormationDestination(Vector3 selectedPoint, int indexInArray){
		Vector3 avePos = FindAverageSelectedObjectPostition ();
		Vector3 formationFwd = Vector3.Normalize (selectedPoint - avePos);
		Vector3 formationRight = Quaternion.Euler (0f, 90f, 0f) * formationFwd;
		Vector3 formationLeft = Quaternion.Euler (0f, -90f, 0f) * formationFwd;
		if (indexInArray == 0) {//first in array
			return selectedPoint;
		}else if (indexInArray % 2 == 0) {//even index
			return selectedPoint + (indexInArray - 1) * m_ShipSeparation * formationRight;
		} else {//odd index
			return selectedPoint + indexInArray * m_ShipSeparation * formationLeft;
		}


	}

	/// <summary>
	/// This fucntion returns the average of all the positions in the selected object array.
	/// It should be used when determining all the waypoints for a formational move order
	/// </summary>
	/// <returns>The average selected object postition.</returns>
	private Vector3 FindAverageSelectedObjectPostition(){
		Vector3 avePos = new Vector3 (0f,0f,0f);
		for (int i = 0; i < m_NumSelectedObjects; i++) {
			avePos = avePos + m_SelectedObjectArray [i].transform.position;
		}
		return avePos / m_NumSelectedObjects;
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
		
		for (int i = 0; i < m_NumSelectedObjects; i++){
			
			if (m_SelectedObjectArray[i] != null) {
				
				m_SelectedObjectArray[i].GetComponent<ShipFireController> ().SetDisposition (disposition);

			}
		}
	}

	/// <summary>
	/// This coroutine will serve to create the selection box and call the function that does the selection with the right values
	/// It keeps track of where the mouse was when you started holding and where it was when you let go
	/// </summary>
	private IEnumerator SelectionBox(){

		if (!m_SelectionBoxOn) {


			m_SelectionBoxOn = true;
			m_SelectionBoxCanvas.enabled = true;
			Vector3 originalMousePosition = m_PlayerCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 currentMousePosition = m_PlayerCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 originalMousePositionRatio = m_PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
			Vector3 currentMousePositionRatio = m_PlayerCamera.ScreenToViewportPoint(Input.mousePosition);

			Vector2 anchorMinVector = new Vector2 (Mathf.Min (originalMousePositionRatio.x, currentMousePositionRatio.x), Mathf.Min (originalMousePositionRatio.y, currentMousePositionRatio.y));
			Vector2 anchorMaxVector = new Vector2 (Mathf.Max (originalMousePositionRatio.x, currentMousePositionRatio.x), Mathf.Max (originalMousePositionRatio.y, currentMousePositionRatio.y));
				
			m_SelectionBoxPanelTransform.anchorMin = anchorMinVector;
			m_SelectionBoxPanelTransform.anchorMax = anchorMaxVector;


				 
			while (Input.GetMouseButton (0)) {


				
				currentMousePosition = m_PlayerCamera.ScreenToWorldPoint(Input.mousePosition);
				currentMousePositionRatio = m_PlayerCamera.ScreenToViewportPoint(Input.mousePosition);

				anchorMinVector.x = Mathf.Min (originalMousePositionRatio.x, currentMousePositionRatio.x);
				anchorMinVector.y = Mathf.Min (originalMousePositionRatio.y, currentMousePositionRatio.y);	
				anchorMaxVector.x = Mathf.Max (originalMousePositionRatio.x, currentMousePositionRatio.x);
				anchorMaxVector.y = Mathf.Max (originalMousePositionRatio.y, currentMousePositionRatio.y);

				m_SelectionBoxPanelTransform.anchorMax = anchorMaxVector;
				m_SelectionBoxPanelTransform.anchorMin = anchorMinVector;


				//DrawBox
				yield return null;

			}

			BoxSelect (originalMousePosition, currentMousePosition);//call box select when we stopp holding mouse
			m_SelectionBoxCanvas.enabled = false;
			m_SelectionBoxOn = false; //Set the running flag of before exitting
			yield return null;

		} else {
			
			yield return null;//Selection box was already running so we do nothing

		}
	}



}
