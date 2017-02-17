using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be responsible for all the functionality of the overworld map itself
/// It will be tied closely to the OverworldDatacontroller and the PlayerDataController
/// </summary>
public class OverworldController : MonoBehaviour {

	public OverworldDataController m_OverworldDataController;
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

	void Awake(){
		m_LocationSelected = false;
	}



	void Start(){
		m_EventController = GameObject.FindGameObjectWithTag ("EventController").GetComponent<EventController> ();
		m_EventController.m_OverworldController = this;
		m_CurrentLocationButton = m_LocationButtons[m_OverworldDataController.GetPlayerLocation ()];
		UpdatePlayerSpriteLocation ();
		StartCoroutine (WaitForLocationSelection());
	}

	public IEnumerator WaitForLocationSelection(){
		while (!m_LocationSelected) {
			
			yield return null;

		}
		yield return null;

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
	/// Gets the index of the selected scene.
	/// </summary>
	/// <returns>The selected level scene index.</returns>
	public int GetSelectedLevelSceneIndex(){
		return m_SelectedLevelIndex;
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
