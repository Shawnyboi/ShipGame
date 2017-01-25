using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// this class contains all the necesarry information and functions associated with the in-level UI
/// </summary>
public class LevelUIController : MonoBehaviour {


	public LevelManager m_LevelManager;
	public PlayerController m_PlayerController;

	public Canvas m_LevelUICanvas;
	public Text m_OutcomeMessage;
	public Button m_StrategicPauseButton;
	public bool m_LevelEnded;

	void Awake(){
		
		m_LevelEnded = false;
		m_LevelManager = this.gameObject.GetComponent<LevelManager> ();
		m_PlayerController = m_LevelManager.m_PlayerController;
		m_OutcomeMessage.enabled = false; 
		//m_StrategicPauseButton.

	}

	public IEnumerator DisplayWinMessage(){
		
		if (!m_LevelEnded) {
			
			m_LevelEnded = true;
			m_OutcomeMessage.text = "Congratulations Captain, You've Won!";
			m_OutcomeMessage.enabled = true;
			yield return new WaitForSeconds (5);
			m_OutcomeMessage.enabled = false;

		}
	}

	public IEnumerator DisplayLoseMessage(){
		
		if (!m_LevelEnded) {
			
			m_LevelEnded = true;
			m_OutcomeMessage.text = "Sorry Captain, You've Lost.";
			m_OutcomeMessage.enabled = true;
			yield return new WaitForSeconds (5);
			m_OutcomeMessage.enabled = false;

		}
	}

	/// <summary>
	/// This function will be attached to the pause button and can used to activate virtual pause
	/// </summary>
	public void TogglePauseFromButton(){
		Debug.Log ("Calling virtual pause in level ui controller");

		m_PlayerController.VirtualPause ();
	}


}
