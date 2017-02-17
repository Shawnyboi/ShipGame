﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The event controller object will control the dialogue boxes that come up during the game;
/// </summary>
public class EventController : MonoBehaviour {

	public OverworldController m_OverworldController;

	private bool m_DialogueOn;

	private string m_CurrentDialogueText;
	private string[] m_CurrentDialogueChoices;

	private float m_TimeScalePauseFactor = .00001f;



	void Start(){
		
		Dialoguer.events.onStarted += OnDialogueStarted;
		Dialoguer.events.onEnded += OnDialogueEnded;
		Dialoguer.events.onTextPhase += OnTextPhase;
		Dialoguer.events.onMessageEvent += ReadMessageEvent;

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

		if (message == "GoToLevel" && m_OverworldController != null){//this tells us we are in the overworld and we want to begin a level
			
			int levelSceneIndex;
			int.TryParse (metadata, out levelSceneIndex);
			m_OverworldController.SetSelectedLevelSceneIndex (levelSceneIndex);

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
}