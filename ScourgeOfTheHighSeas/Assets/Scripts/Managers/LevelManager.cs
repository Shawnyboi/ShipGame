using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class handles the information pertaining to the setup and execution of a single level
public class LevelManager : MonoBehaviour {

	private SpawnPointContainer m_SpawnPointContainer;

	private EventController m_EventController;

	//There can be up to three waves of computer ships for a standard level
	public GameObject[] m_PlayerShips;
	public ShipAttributesData[] m_PlayerShipAttributesData;
	public GameObject[] m_ComShipsFirstWave;
	public GameObject[] m_ComShipsSecondWave;
	public GameObject[] m_ComShipsThirdWave;
	private int m_CurrentWave;

	//Here we keep references to every cureently operating ship in the game
	private List<GameObject> m_LivePlayerShips;
	private List<GameObject> m_LiveComShips;

	//the level manager instantiates the player and the AI
	//then holds onto a reference so it can give them further information
	public GameObject m_PlayerPrefab;
	public PlayerController m_PlayerController;
	public GameObject m_AIPrefab;
	private AIController m_AIController;

	private int m_NumberOfComShips;
	//private int m_NumberOfPlayerShips;

	public LevelUIController m_LevelUIController;

	private bool m_BattleOver;
	private bool m_LevelStarted;


	private void Awake(){

		m_CurrentWave = 1;
		m_BattleOver = false;
		m_LevelStarted = false;
		m_LiveComShips = new List<GameObject> ();
		m_LivePlayerShips = new List<GameObject> ();

		m_LevelUIController = this.gameObject.GetComponent<LevelUIController> ();

		m_EventController = GameObject.FindGameObjectWithTag ("EventController").GetComponent<EventController> ();

		//The number of ships to be generated are determined by the spawn point container object which is in each level
		m_SpawnPointContainer = GameObject.FindGameObjectWithTag ("SpawnPointContainer").GetComponent<SpawnPointContainer>();
		m_PlayerShips = new GameObject[m_SpawnPointContainer.m_PlayerSpawnPoints.Length];
		m_PlayerShipAttributesData = new ShipAttributesData[m_SpawnPointContainer.m_PlayerSpawnPoints.Length];
		m_ComShipsFirstWave = new GameObject[m_SpawnPointContainer.m_ComSpawnPointsFirstWave.Length];
		m_ComShipsSecondWave = new GameObject[m_SpawnPointContainer.m_ComSpawnPointsSecondWave.Length];
		m_ComShipsThirdWave = new  GameObject[m_SpawnPointContainer.m_ComSpawnPointsThirdWave.Length];


	}

	//When the level starts the level manager will create all the ships in the level at the designated 
	private void Start(){
		Debug.Log ("calling start");
		 
		GameObject player = Instantiate (m_PlayerPrefab) as GameObject;
		m_PlayerController = player.GetComponent<PlayerController> ();
		m_LevelUIController.m_PlayerController = m_PlayerController;
		m_PlayerController.GetComponent<PlayerController> ().m_LevelManager = this;
		m_PlayerController.GetComponent<PlayerController> ().SetMaxNumShips (m_PlayerShips.Length);

		//GameObject[] playerShips = new GameObject[m_NumberOfPlayerShips];

		for (int i = 0; i < m_PlayerShips.Length; i++) {
			if (m_PlayerShips [i] != null) {
				Debug.Log ("Instantiating Player ship " + i);
				GameObject playerShip = Instantiate (m_PlayerShips [i], m_SpawnPointContainer.m_PlayerSpawnPoints [i].position, m_SpawnPointContainer.m_PlayerSpawnPoints [i].rotation) as GameObject;
				playerShip.GetComponent<ShipStatusController> ().m_PlayerTeam = true;
				playerShip.gameObject.layer = LayerMask.NameToLayer ("PlayerShips");
				playerShip.GetComponent<ShipStatusController> ().m_LevelManager = this;
				playerShip.GetComponent<ShipAttributes> ().CopyShipAttributesFromData (m_PlayerShipAttributesData [i]);
				playerShip.name = "PlayerShip" + i;
				m_LivePlayerShips.Add (playerShip);
			}
																																										
		}

		SpawnNextWave ();
		m_LevelStarted = true;



	}

	private void Update(){
		

	}

	//This coroutine keeps track of if the battle is over or the next wave should be spawned
	//When this loop ends the level should end and return to the overworld
	public IEnumerator BattleLoop(){
		while (!m_BattleOver) {
			if (m_LevelStarted) {
				//if team destroyed{end level}
				if (m_LiveComShips.Count == 0) {

					if (m_CurrentWave >= 3) {

						yield return StartCoroutine (m_LevelUIController.DisplayWinMessage ());
						m_BattleOver = true;

					} else {

						SpawnNextWave ();

					}
				}

				if (m_LivePlayerShips.Count == 0) {

					yield return StartCoroutine (m_LevelUIController.DisplayLoseMessage ());
					m_BattleOver = true;

				}
			}


			yield return null;

		}
		yield return null;
	}

	//This function will be called by the ships when they die to let the level manager know
	//This is so the level manager can check how many ships are left in the level at any given time
	//The parameter tells the level manager whether it was a player ship or a com ship
	public void ShipDestroyed(bool playerShip){
		m_AIController.RemoveDeadShip ();
		if (playerShip) {
			
			m_PlayerController.DeselectDeadShips ();
			m_LivePlayerShips.RemoveAll (ship => !ship.activeSelf); // remove dead player ships

			//m_NumberOfPlayerShips -= 1;

		} else {

			m_LiveComShips.RemoveAll (ship => !ship.activeSelf); // remove dead com ships
			//m_NumberOfComShips -= 1;

		}
	}

	private void SpawnNextWave(){

		GameObject[] ships;
		Transform[] spawns;

		if (m_CurrentWave == 1) {
			
			ships = m_ComShipsFirstWave;
			spawns = m_SpawnPointContainer.m_ComSpawnPointsFirstWave;

		} else if (m_CurrentWave == 2) {

			ships = m_ComShipsSecondWave;
			spawns =m_SpawnPointContainer. m_ComSpawnPointsSecondWave;

		} else if (m_CurrentWave == 3) {

			ships = m_ComShipsThirdWave;
			spawns = m_SpawnPointContainer.m_ComSpawnPointsThirdWave;

		} else {
			ships = new GameObject[0];
			spawns = new Transform[0];
		}

		for (int i = 0; i < ships.Length; i++) {

			GameObject comShip = Instantiate (ships[i], spawns[i].position, spawns[i].rotation) as GameObject;
			comShip.GetComponent<ShipStatusController>().m_PlayerTeam = false;
			comShip.gameObject.layer = LayerMask.NameToLayer("ComShips");
			comShip.GetComponent<ShipStatusController> ().m_LevelManager = this;
			comShip.name = "ComShip" + i;
			m_LiveComShips.Add(comShip);

		}

		GameObject AI = Instantiate (m_AIPrefab) as GameObject;
		m_AIController = AI.GetComponent<AIController> ();
		m_AIController.SetUpAI (m_LiveComShips, m_LivePlayerShips);

		m_CurrentWave += 1;

	}

	/// <summary>
	/// This will return the list of live player ships in the level;
	/// Use this to keep the damage of ships continous throughout levels
	/// </summary>
	/// <returns>The live player ships.</returns>
	public List<GameObject> GetLivePlayerShips(){
		return m_LivePlayerShips;
	}


}
