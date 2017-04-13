 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be responsible for all the functionality of the overworld map itself
/// It will be tied closely to the OverworldDatacontroller and the PlayerDataController
/// </summary>
public class OverworldController : MonoBehaviour {

	public OverworldDataController m_OverworldDataController;
	public PlayerDataController m_PlayerDataController;

	private EventController m_EventController;
	private bool m_LocationSelected;
	private int m_SelectedLevelIndex;
	private GameObject m_CurrentLocation;

	public GameObject[] m_Locations;
	public GameObject m_PlayerAvatar;

	public int m_GenericLocationDialogueIndex;

	//public RectTransform m_PlayerSpriteRectTransform;

	//public float m_PlayerSpriteXCoordinate;
	//public float m_PlayerSpriteYCoordinate;
	//public float m_PlayerSpriteWidth;
	//public float m_PlayerSpriteHeight;

	public int m_RepairCost;

	private Rect m_GoldBoxRect;
	private int m_CurrentPlayerGold;

	private int m_FleetViewerSceneIndex;


	void Awake(){

		m_GoldBoxRect = new Rect (Screen.width * .40f, Screen.height * .05f, Screen.width * .2f, Screen.height * .05f);

		m_LocationSelected = false;
		m_FleetViewerSceneIndex = 3;

		if (m_EventController != null) {
			m_EventController = null;
		}		
	}



	void Start(){
		


		m_EventController = GameObject.FindGameObjectWithTag ("EventController").GetComponent<EventController> ();

		m_CurrentPlayerGold = m_PlayerDataController.GetPlayerGold ();

		m_CurrentLocation = m_Locations[m_OverworldDataController.GetPlayerLocation ()];
		UpdatePlayerAvatarLocation ();

	}
	/// <summary>
	/// this coroutine checks every frame if a new location is selected if it is the coroutine returns
	/// We use this to signal to the game manager when to switch scenes
	/// </summary>
	public IEnumerator WaitForLocationSelection(){
		while (!m_LocationSelected) {
			
			yield return null;

		}
		yield return null;

	}

	void OnGUI(){
		
		GUI.Box (m_GoldBoxRect, "Booty: " + m_CurrentPlayerGold);

	}


	/// <summary>
	/// This function sets the selected location.
	/// It will break the WaitForSelection loop and therby the overworld loop in the game manager thus starting the new level.
	/// </summary>
	/// <param name="levelIndex">the scene index of the level in the build editor</param>   
	/// <param name="locationIndex">the index of this location in the locations array in the overworld controller.</param>
	public void SelectLocation(int locationIndex){
		
		if (m_CurrentLocation == m_Locations[locationIndex]) {
			
			m_EventController.StartEventDialogue (m_GenericLocationDialogueIndex);
					
		} else {
			
			//check if we can move there
			//move if posiible
			//trigger event;
			if (m_OverworldDataController.CheckIfLocationConnected (m_CurrentLocation.GetComponent<LocationMarkerController> ().m_LocationIndex, locationIndex)) {
				Debug.Log ("moving to location index " + locationIndex);
				SetCurrentLocation (locationIndex);
				UpdatePlayerAvatarLocation ();
				m_EventController.StartEventDialogue (m_OverworldDataController.GetRandomEventIndexAtLocation (locationIndex));
			
			} else {
				Debug.Log ("location index " + locationIndex + " is not connected to this location");
			}
		}
	}
	/// <summary>
	/// Sets the selected level index variable in order to cause the game manager to load it
	/// </summary>
	/// <param name="sceneIndex">Scene index.</param>
	public void SetSelectedLevelSceneIndex(int sceneIndex){
		
		Debug.Log ("Setting level scene index confirmed " + sceneIndex);
		m_SelectedLevelIndex = sceneIndex;
		m_LocationSelected = true;

	}

	/// <summary>
	/// Sets the selected level index variable to the fleet viewer scene in order to cause the game manager to load it
	/// </summary>
	public void SetSelectedSceneToFleetViewer(){
		
		m_SelectedLevelIndex = m_FleetViewerSceneIndex;
		m_LocationSelected = true;
	}

	/// <summary>
	/// Gets the index of the selected scene.
	/// </summary>
	/// <returns>The selected level scene index.</returns>
	public int GetSelectedLevelSceneIndex(){
		
		return m_SelectedLevelIndex;

	}

	/// <summary>
	/// Deducts an amount of gold from the players stash
	/// </summary>
	public void SpendGold(int amount){
		
		m_PlayerDataController.ChangePlayerGold (-amount);
		m_CurrentPlayerGold = m_PlayerDataController.GetPlayerGold ();

	}

	/// <summary>
	/// Adds an amount of gold to the players stash
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void EarnGold(int amount){
		
		m_PlayerDataController.ChangePlayerGold (amount);
		m_CurrentPlayerGold = m_PlayerDataController.GetPlayerGold ();

	}

	/// <summary>
	/// Gets called when the player buys a ship at the shop
	/// adds the ship to the player fleet if the player has enough money
    /// returns false if you don't have enough
	/// </summary>
	public bool BuyShip(string shipType, int shipCost){
		
		if (m_CurrentPlayerGold >= shipCost) {
			
			m_PlayerDataController.AddShipToFleet (shipType);
			SpendGold (shipCost);
			return true;

		} else {
			
			return false;

		}
	}

	/// <summary>
	/// Gets called when a player buys an upgrade
	/// adds the upgrade to the available upgrades if the player has enough funds
	/// returns false if you don't have enough
	/// </summary>
	public bool BuyUpgrade(string upgradeType, int upgradeCost){

		if (m_CurrentPlayerGold >= upgradeCost) {

			m_PlayerDataController.AddUpgrade (upgradeType);
			SpendGold (upgradeCost);
			return true;

		} else {
			
			return false;

		}
	}


	/// <summary>
	/// Gets called when the repair ships option is chosen at some location
	/// </summary>
	/// <returns><c>true</c>, if had enough money to repair, <c>false</c> otherwise.</returns>
	public bool GoToRepair(){
		
		Debug.Log ("calling go to repair");

		if (m_CurrentPlayerGold >= m_RepairCost) {
			
			m_PlayerDataController.RepairFleet ();
			SpendGold (m_RepairCost);
			return true;

		} else {
			
			return false;

		}
	}

	/// <summary>
	/// Save the game.
	/// </summary>
	public void Save(){
		
		GameDataController gameDataController = new GameDataController ();
		gameDataController.Save (m_OverworldDataController, m_PlayerDataController);

	}

	/// <summary>
	/// Sets the current location in this and the overworld data controller.
	/// </summary>
	/// <param name="indexOfLocation">Index of location in the locations array.</param>
	private void SetCurrentLocation(int indexOfLocation){
		
		m_OverworldDataController.SetPlayerLocation (indexOfLocation);
		m_CurrentLocation = m_Locations [indexOfLocation];

	}

	/// <summary>
	/// Move the player location indicating avatar to where it should be as dictated by the current player location
	/// </summary>
	private void UpdatePlayerAvatarLocation(){
		
		m_PlayerAvatar.transform.position = m_CurrentLocation.transform.position + new Vector3 (-15.0f, 0.0f, 0.0f);

	}



}
