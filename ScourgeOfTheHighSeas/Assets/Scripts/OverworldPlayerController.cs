using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This handles the player input when the overworld scene is active
/// </summary>
public class OverworldPlayerController : MonoBehaviour {

	public Camera m_PlayerCamera;
	public OverworldController m_OverworldController;

	void Update(){

		RayToLocationMarker ();

		if (Input.GetMouseButtonDown (0)) {

			SelectLocation ();

		}
	}

	/// <summary>
	/// Gets called when the player clicks in the overworld.
	/// If you click a location the player should go there.
	/// </summary>
	private void SelectLocation(){

		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitObject;

		if (Physics.Raycast (rayToObject, out hitObject)) {

			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("LocationMarkers")) {

				int locationIndexSelected = hitObject.collider.gameObject.GetComponent<LocationMarkerController> ().GetLocationIndex ();

				if (m_OverworldController != null) {
					
					Debug.Log ("Selecting Location " + locationIndexSelected);						
					m_OverworldController.SelectLocation (locationIndexSelected);

				}

			}

		}

	}

	/// <summary>
	/// This function which will be called every frame is intended to let the location markers know that they are
	/// being moused over, which should cause them to light up and display their name.
	/// </summary>
	private void RayToLocationMarker(){
		
		Ray rayToObject = m_PlayerCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitObject;

		if (Physics.Raycast (rayToObject, out hitObject)) {

			if (hitObject.collider.gameObject.layer == LayerMask.NameToLayer ("LocationMarkers")) {

				LocationMarkerController locationMarker = hitObject.collider.gameObject.GetComponent<LocationMarkerController> ();
				locationMarker.Highlight ();
			}

		}

	}
}
