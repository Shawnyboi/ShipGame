using UnityEngine;
using System.Collections;

//this class deals with information pertaining too the movement of the camera 
public class CameraController : MonoBehaviour {

	public Camera m_MainCamera;
	private Transform m_MainCameraTransform;
	public float m_CameraSpeed;
	public bool m_MouseEnabled;

	void Awake(){
		m_MainCameraTransform = m_MainCamera.gameObject.GetComponent<Transform> ();
	}



	void FixedUpdate () {

		Vector3 inputVector = new Vector3 (Input.GetAxis("Horizontal"), 0f, Input.GetAxis ("Vertical"));//take input from axis before from mouse so as to 
																										//not destroy the mouse scrolling information
		if (m_MouseEnabled) {
			if (Input.mousePosition.x > Screen.width * .98f) {//if mouse at right side of screen scroll right
				inputVector.x = 1f;
			} else if (Input.mousePosition.x < Screen.width * .02f) { //if at left scroll left
				inputVector.x = -1f;
			}

			if (Input.mousePosition.y > Screen.height * .98f) {//if mouse at top of screen scroll up
				inputVector.y = 1f;
			} else if (Input.mousePosition.y < Screen.height * .02f) {//if at bottom scroll down
				inputVector.y = -1f;	
			}
		}

		Vector3 deltaMovement = CalculateCameraMovement (inputVector);	
		MoveCamera (deltaMovement);
	}

	//Move the camera as calculated by CalculateCameraMovement
	private void MoveCamera(Vector3 deltaMovement){
		m_MainCameraTransform.position = m_MainCameraTransform.position + deltaMovement;
	}


	//Calculate how the camera should be moved from the given input
	private Vector3 CalculateCameraMovement(Vector3 inputVector){

		Vector3 rotatedInput = Quaternion.Euler (0, m_MainCameraTransform.eulerAngles.y, 0) * inputVector; //The input must be rotated to account for the direction the camera is facing
		return rotatedInput * m_CameraSpeed * Time.unscaledDeltaTime; //then we multiply by the camera's scroll speed

	}

	/// <summary>
	/// This function compensates for virtual pause by mulitplying camera speed by the inverse of the virtual pause time scale factor. It is intended to be called by the player object
	/// IT CURRENTLY DOESNT WORK
	/// </summary>
	/// <param name="timeScaleFactor">the factor with which you intend to multiply the cameraspeed.</param>
	//public void CompensateForVirtualPause(float timeScaleFactor){
	//	m_CameraSpeed *= timeScaleFactor;
	//	Debug.Log ("New Camera Speed " + m_CameraSpeed);
	//}



}
