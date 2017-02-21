using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetViewerController : MonoBehaviour {

	private PlayerDataController m_PlayerDataController;
	private List<ShipAttributesData> m_ShipDataList;
	private int m_CurrentSelectedShipIndex;
	private GameObject m_CurrentSelectedShip;

	private string m_ShipFolderName = "Ships";

	public FleetViewerUIController m_FleetViewerUIController;
	public Transform m_ShipTransform;

	private Vector3 m_ShipPosition;
	private Quaternion m_ShipRotation;

	private bool m_ExittingFleetViewer;

	void Awake(){
		m_ExittingFleetViewer = false;

		m_ShipPosition = m_ShipTransform.position;
		m_ShipRotation = Quaternion.Euler (m_ShipTransform.eulerAngles);

		m_PlayerDataController = GameObject.FindGameObjectWithTag ("PlayerDataController").GetComponent<PlayerDataController>();
		m_ShipDataList = m_PlayerDataController.GetFleetData ();

		m_CurrentSelectedShipIndex = 0;
		DisplayShip (m_CurrentSelectedShipIndex);



	}

	/// <summary>
	/// Increments the current ship index and displays the new ship
	/// if we are at the end of the array we got to the beggining
	/// </summary>
	public void GoToNextShip(){
		
		if (m_CurrentSelectedShipIndex < m_ShipDataList.Count - 1) {
			
			m_CurrentSelectedShipIndex++;

		} else {
			
			m_CurrentSelectedShipIndex = 0;

		}

		DisplayShip (m_CurrentSelectedShipIndex);

	}

	/// <summary>
	/// decrements the current ship index and displays the new ship
	/// if we are at the beginning of the array we go to the end
	/// </summary>
	public void GoToPreviousShip(){
		
		if (m_CurrentSelectedShipIndex > 0) {

			m_CurrentSelectedShipIndex--;

		} else {

			m_CurrentSelectedShipIndex = m_ShipDataList.Count - 1;

		}

		DisplayShip (m_CurrentSelectedShipIndex);


	}
		

	/// <summary>
	/// This function instatntiates the ship in  the ship data list referred to by the ship index
	/// it will also remove the current selected ship and store the new ship's pointer in m_CurrentSelectedShip
	/// </summary>
	/// <param name="shipIndex">Ship index.</param>
	private void DisplayShip(int shipIndex){
		if (m_CurrentSelectedShip != null) {
			Destroy (m_CurrentSelectedShip);
		}
		m_CurrentSelectedShip = Instantiate (Resources.Load(m_ShipFolderName + "/" + m_ShipDataList [shipIndex].shipName), m_ShipPosition, m_ShipRotation) as GameObject;
		ShipStatusController currentShipStatusController = m_CurrentSelectedShip.GetComponent<ShipStatusController> ();
		currentShipStatusController.m_LifebarCanvas.enabled = false;
	}

	/// <summary>
	/// Call this function to go back to overworld by ending the FleetViewerLoop Coroutine in the game manager
	/// </summary>
	public void ExitFleetViewer(){
		m_ExittingFleetViewer = true;
	}

	/// <summary>
	/// Waits for fleet viewer exit.
	/// Is started by the game manager and ends when ExitFleetViewer is called
	/// </summary>
	public IEnumerator WaitForFleetViewerToExit(){
		while (!m_ExittingFleetViewer) {

			yield return null;

		}

		yield return null;

	}

	/// <summary>
	/// We can call this function from outside this class to get the current attributes of the ship being displayed
	/// </summary>
	/// <returns>The current ship attributes data.</returns>
	public ShipAttributesData GetCurrentShipAttributes(){
		return m_ShipDataList [m_CurrentSelectedShipIndex];
	}

}
