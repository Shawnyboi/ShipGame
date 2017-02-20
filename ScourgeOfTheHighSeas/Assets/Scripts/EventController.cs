using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The event controller object will control the dialogue boxes that come up during the game;
/// </summary>
public class EventController : MonoBehaviour {

	private OverworldController m_OverworldController;
	private PlayerDataController m_PlayerDataController;

	public GUIStyle m_GUIStyle;

	public bool m_InOverworld;

	private bool m_DialogueOn;

	private string m_CurrentDialogueText;
	private string[] m_CurrentDialogueChoices;

	private float m_TimeScalePauseFactor = .00001f;

	private static int m_NotEnoughMoneyEventIndex = 6;
	private static int m_ThankYouEventIndex = 9;

	void Awake(){
		if (m_OverworldController != null) {
			m_OverworldController = null;
		}
	}

	void Start(){
		if (m_InOverworld) {
			m_OverworldController = GameObject.FindGameObjectWithTag ("OverworldController").GetComponent<OverworldController> ();
		}
		m_PlayerDataController = GameObject.FindGameObjectWithTag ("PlayerDataController").GetComponent<PlayerDataController> ();

		Dialoguer.events.onStarted += OnDialogueStarted;
		Dialoguer.events.onEnded += OnDialogueEnded;
		Dialoguer.events.onTextPhase += OnTextPhase;
		Dialoguer.events.onMessageEvent += ReadMessageEvent;

	}

	void OnDestroy(){
		Dialoguer.events.onStarted -= OnDialogueStarted;
		Dialoguer.events.onEnded -= OnDialogueEnded;
		Dialoguer.events.onTextPhase -= OnTextPhase;
		Dialoguer.events.onMessageEvent -= ReadMessageEvent;
	}

	public void StartEventDialogue(int dialoguerIndex){
		Dialoguer.StartDialogue (dialoguerIndex);
	}

	/// <summary>
	/// Execute code when the GUI is rendered
	/// Specifically in the event manager this renders the dialogue boxes
	/// </summary>
	void OnGUI(){
		if (m_DialogueOn == false) {
			return;
		} else {

			GUI.Box (new Rect (10, 10, 200, 150), m_CurrentDialogueText);

			if (m_CurrentDialogueChoices == null) {

				if (GUI.Button (new Rect (10, 220, 200, 30), "continue")) {
					Dialoguer.ContinueDialogue ();
				}
			} else if (m_CurrentDialogueChoices != null) {
				for (int i = 0; i < m_CurrentDialogueChoices.Length; i++) {
				
					if (GUI.Button (new Rect (10, 220 + (30 * i), 200, 30), m_CurrentDialogueChoices [i])) {
						Dialoguer.ContinueDialogue (i);
						break;
					}
				}
			}
		}

	}
	/// <summary>
	/// We store event commands in the dialoguer dialogues which will be activated based on player choices
	/// Here in the event controller those commands are parsed and the appropriate actions are carried out
	/// Not the message event is part of the dialoguer system itself
	/// </summary>
	private void ReadMessageEvent(string message, string metadata){
		
		Debug.Log ("Message Event: " + message);
		Debug.Log ("Metadata: " + metadata);

		if (message == "GoToLevel" && m_OverworldController != null) {//this tells us we are in the overworld and we want to begin a level
			
			int levelSceneIndex;
			int.TryParse (metadata, out levelSceneIndex);
			m_OverworldController.SetSelectedLevelSceneIndex (levelSceneIndex);

		} else if (message == "RepairShips" && m_OverworldController != null) {
			bool repaired = m_OverworldController.GoToRepair ();
			if (!repaired) {
				StartEventDialogue (m_NotEnoughMoneyEventIndex);
			}

		} else if (message == "GetReward" && m_PlayerDataController != null) {
			
			int rewardValue;
			int.TryParse (metadata, out rewardValue);

			if (m_OverworldController != null) {//if in overworld we call earn gold so the overworld ui is updated
				
				m_OverworldController.EarnGold (rewardValue);

			} else {//otherwise we go straight for the player data controller
				
				m_PlayerDataController.ChangePlayerGold (rewardValue);

			}
		} else if (message == "BuyShip" && m_OverworldController != null) {
			string[] metadataArray = SplitMetadataString (metadata);
			string shipType = metadataArray [0];
			int shipCost;
			int.TryParse (metadataArray [1], out shipCost);
			bool boughtShip = m_OverworldController.BuyShip (shipType, shipCost);
			if (boughtShip) {
				StartEventDialogue (m_ThankYouEventIndex);
			} else {
				StartEventDialogue (m_NotEnoughMoneyEventIndex);
			}

		} else if (message == "GoToDialogue") {
			int dialogueIndex;
			int.TryParse (metadata, out dialogueIndex);
			StartEventDialogue (dialogueIndex);
		}
	}

	/// <summary>
	/// execute code when a dialoguer dialogue begins
	/// </summary>
	private void OnDialogueStarted(){
		m_DialogueOn = true;
		PauseGame ();
	}

	/// <summary>
	/// Execute code when a dialoguer dialogue ends
	/// </summary>
	private void OnDialogueEnded(){
		m_DialogueOn = false;
		UnpauseGame ();
	}

	/// <summary>
	/// Execute code when the text is being created in the dialoguer dialogue
	/// </summary>
	/// <param name="textData">Text data.</param>
	private void OnTextPhase(DialoguerTextData textData){
		m_CurrentDialogueText = textData.text;
		m_CurrentDialogueChoices = textData.choices;
		Debug.Log (textData.choices);
	}

	private void PauseGame(){
		Time.timeScale = Time.timeScale * m_TimeScalePauseFactor;
	}

	private void UnpauseGame(){
		Time.timeScale = 1f;
	}

	private string[] SplitMetadataString(string metadata, char delimiter = ' ', int substringCount = 2){
		char[] delimeterArray = { delimiter };
		return metadata.Split (delimeterArray, substringCount);

	}

	/// <summary>
	/// Sets the overworld controller.
	/// </summary>
	public void SetOverworldController(OverworldController overWorldController){
		m_OverworldController = overWorldController;
	}

	/// <summary>
	/// A coroutine which will run as long as the dialogue box is on
	/// </summary>
	public IEnumerator WaitForDialogueToFinish(){
		while (m_DialogueOn) {
			yield return null;
		}
		yield return null;
	}
}
