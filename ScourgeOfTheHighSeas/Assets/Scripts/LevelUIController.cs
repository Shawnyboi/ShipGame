using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// this class contains all the necesarry information and functions associated with the in-level UI
/// </summary>
public class LevelUIController : MonoBehaviour {


	public LevelManager m_LevelManger;
	public PlayerController m_PlayerController;

	public Text m_OutcomeMessage;
	public Button m_StrategicPauseButton;
	public bool m_LevelEnded;

	void Awake(){
		
		m_LevelEnded = false;
		m_LevelManger = this.gameObject.GetComponent<LevelManager> ();
		m_OutcomeMessage.enabled = false;

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


}
