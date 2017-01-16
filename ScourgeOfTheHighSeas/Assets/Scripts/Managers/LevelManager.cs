using UnityEngine;
using System.Collections;

//This class handles the information pertaining to the setup and execution of a single level
public class LevelManager : MonoBehaviour {

	public Transform[] m_PlayerSpawnPoints;
	public Transform[] m_ComSpawnPoints;

	public GameObject[] m_PlayerShips;
	public GameObject[] m_ComShips;

	public GameObject m_PlayerPrefab;
	private PlayerController m_PlayerController;

	private int m_NumberOfComShips;
	private int m_NumberOfPlayerShips;

	public LevelUIController m_LevelUIController;


	private void Awake(){

		m_LevelUIController = this.gameObject.GetComponent<LevelUIController> ();

		if (m_PlayerShips != null) {
			m_NumberOfPlayerShips = m_PlayerShips.Length;
		} else {
			m_NumberOfPlayerShips = 0;
		}

		if (m_ComShips != null) {
			m_NumberOfComShips = m_ComShips.Length;
		} else {
			m_NumberOfComShips = 0;
		}

	}

	//When the level starts the level manager will create all the ships in the level at the designated 
	private void Start(){
		GameObject player = Instantiate (m_PlayerPrefab) as GameObject;
		m_PlayerController = player.GetComponent<PlayerController> ();
		m_LevelUIController.m_PlayerController = m_PlayerController;
		m_PlayerController.GetComponent<PlayerController> ().m_LevelManager = this;
		m_PlayerController.GetComponent<PlayerController> ().SetMaxNumShips (m_NumberOfPlayerShips);


		for (int i = 0; i < m_PlayerSpawnPoints.Length; i++) {
			GameObject playerShip = Instantiate (m_PlayerShips [i], m_PlayerSpawnPoints [i].position, m_PlayerSpawnPoints [i].rotation) as GameObject;
			playerShip.GetComponent<ShipStatusController>().m_PlayerTeam = true;
			playerShip.gameObject.layer = LayerMask.NameToLayer("PlayerShips");
			playerShip.GetComponent<ShipStatusController> ().m_LevelManager = this;
			playerShip.name = "PlayerShip" + i;
																																							
		}
		for (int i = 0; i < m_ComSpawnPoints.Length; i++) {
			GameObject comShip = Instantiate (m_ComShips [i], m_ComSpawnPoints [i].position, m_ComSpawnPoints [i].rotation) as GameObject;
			comShip.GetComponent<ShipStatusController>().m_PlayerTeam = false;
			comShip.gameObject.layer = LayerMask.NameToLayer("ComShips");
			comShip.GetComponent<ShipStatusController> ().m_LevelManager = this;
			comShip.name = "ComShip" + i;
		}
		//Here ^^ we create all the ships in the level
	}

	private void Update(){
		
		//if team destroyed{end level}
		if (m_NumberOfComShips <= 0) {
			
			StartCoroutine (m_LevelUIController.DisplayWinMessage ());
		}

		if (m_NumberOfPlayerShips <= 0) {
			
			StartCoroutine (m_LevelUIController.DisplayLoseMessage ());

		}
	}

	//This function will be called by the ships when they die to let the level manager know
	//This is so the level manager can check how many ships are left in the level at any given time
	//The parameter tells the level manager whether it was a player ship or a com ship
	public void ShipDestroyed(bool playerShip){
		
		if (playerShip) {
			
			m_PlayerController.DeselectDeadShips ();

			m_NumberOfPlayerShips -= 1;

		} else {
			
			m_NumberOfComShips -= 1;

		}
	}



}
