﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetViewerController : MonoBehaviour {

	private PlayerDataController m_PlayerDataController;
	private List<ShipAttributesData> m_ShipDataList;
	private List<ShipUpgrade> m_UpgradeList;
	private int m_CurrentSelectedShipIndex;
	private GameObject m_CurrentSelectedShip;

	private string m_ShipFolderName = "Ships";

	public FleetViewerUIController m_FleetViewerUIController;
	public Transform m_ShipTransform;

	private Vector3 m_ShipPosition;
	private Quaternion m_ShipRotation;

	private bool m_ExittingFleetViewer;
	private bool m_DisplayingShip;

	void Awake(){
		m_ExittingFleetViewer = false;

		m_ShipPosition = m_ShipTransform.position;
		m_ShipRotation = Quaternion.Euler (m_ShipTransform.eulerAngles);

		m_PlayerDataController = GameObject.FindGameObjectWithTag ("PlayerDataController").GetComponent<PlayerDataController>();
		m_ShipDataList = m_PlayerDataController.GetFleetData ();
		m_UpgradeList = m_PlayerDataController.GetUpgradeData ();
		Debug.Log ("m_UpgradeList in FleetViewerController has count " + m_UpgradeList.Count); 

		m_CurrentSelectedShipIndex = 0;
		m_DisplayingShip = false;

		if (!m_DisplayingShip) {
			StartCoroutine (DisplayShip (m_CurrentSelectedShipIndex));
		}



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

		if (!m_DisplayingShip) {
			StartCoroutine (DisplayShip (m_CurrentSelectedShipIndex));
		}
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

		if (!m_DisplayingShip) {
			StartCoroutine (DisplayShip (m_CurrentSelectedShipIndex));
		}

	}
		
	/// <summary>
	/// This coroutine instantiates the ship in  the ship data list referred to by the ship index
	/// it will also remove the current selected ship and store the new ship's pointer in m_CurrentSelectedShip
	/// </summary>
	/// <param name="shipIndex">Ship index.</param>
	private IEnumerator DisplayShip(int shipIndex){
		m_DisplayingShip = true;
		if (m_CurrentSelectedShip != null) {

			Destroy (m_CurrentSelectedShip);

			yield return new WaitForSeconds (.2f); //if we are switching ships we wait a second before displaying it

		}
		m_CurrentSelectedShip = Instantiate (Resources.Load(m_ShipFolderName + "/" + m_ShipDataList [shipIndex].shipName), m_ShipPosition, m_ShipRotation) as GameObject;
		ShipStatusController currentShipStatusController = m_CurrentSelectedShip.GetComponent<ShipStatusController> ();
		currentShipStatusController.m_LifebarCanvas.enabled = false;
		m_DisplayingShip = false;
		yield return null;

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

	/// <summary>
	/// Gets the list of ship upgrades in the current fleet viewerxc  
	/// </summary>
	/// <returns>The upgrade list.</returns>
	public List<ShipUpgrade> GetUpgradeList(){
		return m_UpgradeList;
	}

	/// <summary>
	/// Takes a ship upgrade object and it apllies it to the ship currently being looked at in the fleet viewer
	/// returns false if upgrade is not able to be applied
	/// </summary>
	public bool ApplyUpgradeToCurrentShip(ShipUpgrade upgrade){
		if (upgrade.AugmentShip (m_ShipDataList [m_CurrentSelectedShipIndex])) {
			DisplayShip (m_CurrentSelectedShipIndex);
			return true;
		} else {
			return false;
		}
	}

}
