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
	private GameObject m_CurrentLocationButton;

	public GameObject[] m_LocationButtons;
	public GameObject m_PlayerSprite;

	public RectTransform m_PlayerSpriteRectTransform;

	public float m_PlayerSpriteXCoordinate;
	public float m_PlayerSpriteYCoordinate;
	public float m_PlayerSpriteWidth;
	public float m_PlayerSpriteHeight;

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

		m_CurrentLocationButton = m_LocationButtons[m_OverworldDataController.GetPlayerLocation ()];
		UpdatePlayerSpriteLocation ();

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
	/// It is intended to be called by a unity button.
	/// It will break the WaitForSelection loop and therby the overworld loop in the game manager thus starting the new level.
	/// </summary>
	/// <param name="levelIndex">the scene index of the level in the build editor</param>   
	/// <param name="buttonLocationIndex">the index of this location button in the location buttons array in the overworld controller.</param>
	public void SelectLocation(int buttonLocationIndex){
		if (m_CurrentLocationButton == m_LocationButtons[buttonLocationIndex]) {
			m_EventController.StartEventDialogue (0);
			//Debug.Log ("Selecting Location");
			//m_LocationSelected = true;
			//m_SelectedLevelIndex = m_OverworldDataController.GetRandomSceneIndexAtLocation(buttonLocationIndex);
		} else {
			//check if we can move there
			//move if posiible
			//trigger event;
			if (m_OverworldDataController.CheckIfLocationConnected (m_CurrentLocationButton.GetComponent<LocationButtonController> ().m_LocationIndex, buttonLocationIndex)) {
				SetCurrentLocationButton (buttonLocationIndex);
				UpdatePlayerSpriteLocation ();
				m_EventController.StartEventDialogue(m_OverworldDataController.GetRandomEventIndexAtLocation (buttonLocationIndex));
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
	/// Updates the position where the player's location sprite is rendered on the screen,
	/// We want to put it where the button the player is at is rendered
	/// </summary>
	private void UpdatePlayerSpriteLocation(){
		
		LocationButtonController currentButtonController = m_CurrentLocationButton.GetComponent<LocationButtonController> ();
		m_PlayerSpriteXCoordinate = currentButtonController.m_ButtonXCoordinate + .5f * currentButtonController.m_ButtonWidth;
		m_PlayerSpriteYCoordinate = currentButtonController.m_ButtonYCoordinate + 3f * currentButtonController.m_ButtonHeight;
		Rect spriteRect = new Rect(new Vector2(m_PlayerSpriteXCoordinate * Screen.width,  m_PlayerSpriteYCoordinate * Screen.height), new Vector2(m_PlayerSpriteWidth * Screen.height, m_PlayerSpriteHeight * Screen.width));
		m_PlayerSpriteRectTransform.anchoredPosition = spriteRect.position;
		m_PlayerSpriteRectTransform.anchorMax = new Vector2 (m_PlayerSpriteWidth, m_PlayerSpriteHeight);
		m_PlayerSpriteRectTransform.anchorMin = new Vector2 (0f, 0f);
		m_PlayerSpriteRectTransform.sizeDelta = new Vector2 (spriteRect.width, spriteRect.height);

	}

	/// <summary>
	/// Sets the current location button in this and the overworld data controller.
	/// </summary>
	/// <param name="indexOfLocationButton">Index of location button in the location buttons array.</param>
	private void SetCurrentLocationButton(int indexOfLocationButton){
		m_OverworldDataController.SetPlayerLocation (indexOfLocationButton);
		m_CurrentLocationButton = m_LocationButtons [indexOfLocationButton];
	}



}
