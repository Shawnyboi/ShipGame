using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// The game manager class is the foundational code that ties varioous elements of the game (like the individual levels, the overworld, and the data) together
/// It will contain information necesarry to Load Scenes, instantiate level managers, load data and exit the game
/// </summary>
public class GameManager : MonoBehaviour {

	//These objects are everything the Gamemanager interacts with
	private LevelManager m_CurrentLevelManager;
	private OverworldController m_CurrentOverworldController;
	private OverworldDataController m_OverworldDataController;
	private PlayerDataController m_PlayerDataController;

	private string m_OverworldControllerTag;

	public LevelManager m_GenericLevelManagerPrefab;
	//public OverworldController m_OverworldControllerPrefab;

	//we need to know the overworld scene index as we will return to it many times
	private int m_OverworldSceneIndex;

	void Awake (){
		DontDestroyOnLoad (gameObject);// we want the game manager to stay around all the time
		Dialoguer.Initialize();

	}


	/// <summary>
	/// This will get called after the game manager is instantiated to set some receive certain member variable references
	/// </summary>
	public void InitializeGameManager(OverworldDataController overworldDataController, PlayerDataController playerDataController, bool newGame){
		
		m_OverworldDataController = overworldDataController;
		m_PlayerDataController = playerDataController;
		if (!newGame) {
			GameDataController gameDataController = new GameDataController ();
			gameDataController.Load (m_OverworldDataController, m_PlayerDataController);
		}
		m_OverworldSceneIndex = 1;
		m_OverworldControllerTag = "OverworldController";
		GoToOverworld ();

	}

	/// <summary>
	/// Call this function to bring the game into the overworld map
	/// it will also create and initialize the overworld controller
	/// </summary>
	private void GoToOverworld(){
		
		SceneManager.LoadScene (m_OverworldSceneIndex);
		//See OnSceneFinishedLoading for additional logic that this causes

	}
	/// <summary>
	/// Call this function to go to a certain level
	/// This will indirectly create the level manager
	/// </summary>
	/// <param name="levelSceneIndex">index for the scene manager.</param>
	private void GoToLevel(int levelSceneIndex){
		
		SceneManager.LoadScene (levelSceneIndex);
		//See OnSceneFinishedLoading for additional logic that this causes

	}


	/// <summary>
	/// This function will be called in the game manager and it will fill the current level manager with ships based on the scene
	/// the level manager being populated it passed in as a 
	/// </summary>
	public void PopulateLevelManager(int sceneIndex, LevelManager levelManager){

		GameObject[] comShips = m_OverworldDataController.GetEnemyShipsForLevel (sceneIndex);
		Dictionary<string, int> eventDictionary = m_OverworldDataController.GetEventDictionaryForLevel (sceneIndex);
		levelManager.InitializeEventDictionary (eventDictionary);

		int playerShipsToSpawn = Mathf.Min (m_CurrentLevelManager.m_PlayerShips.Length, m_PlayerDataController.GetFleetSize()); //determine how many player ships to take from fleet

		for (int i = 0; i < playerShipsToSpawn; i++) { //loop through player fleet for the first n ships and give their prefab references to the level manager

			levelManager.m_PlayerShips [i] = m_PlayerDataController.GetShipFromFleet(i); //fill the player ships in the level manager with the first ships in the player fleet
			levelManager.m_PlayerShipAttributesData[i] = m_PlayerDataController.GetShipAttributesDataFromFleet(i);
		}

		m_PlayerDataController.RemoveShipsfromFleet (0, playerShipsToSpawn); //remove those ships so they can be replaced with the updated versions later

		for (int i = 0; i < levelManager.m_ComShipsFirstWave.Length; i++) {

			levelManager.m_ComShipsFirstWave [i] = comShips[Random.Range (0, comShips .Length)];

		}

		for (int i = 0; i < levelManager.m_ComShipsSecondWave.Length; i++) {

			levelManager.m_ComShipsSecondWave [i] = comShips [Random.Range (0, comShips .Length)];

		}

		for (int i = 0; i < levelManager.m_ComShipsThirdWave.Length; i++) {

			levelManager.m_ComShipsThirdWave [i] = comShips [Random.Range (0, comShips .Length)]; //fill the com ship arrays with random ships from the array given by the OwDC

		}
	}

	/// <summary>
	/// this coroutine starts the battleloop coroutine and thereby keeps track if the level is over
	/// When the level ends it takes the game back to the overworld;
	/// </summary>
	/// <returns>The loop.</returns>
	public IEnumerator LevelLoop(){
		
		yield return StartCoroutine(m_CurrentLevelManager.BattleLoop());
		LevelCleanup ();
		GoToOverworld ();

	}


	/// <summary>
	/// This coroutine starts the loop that waits for the player to select a location (level)
	/// When selected it calls the go to level funcion for that level;
	/// </summary>
	/// <returns>The loop.</returns>
	public IEnumerator OverworldLoop(){
		
		m_CurrentOverworldController = GameObject.FindWithTag (m_OverworldControllerTag).GetComponent<OverworldController>();
		yield return StartCoroutine (m_CurrentOverworldController.WaitForLocationSelection ());
		GoToLevel (m_CurrentOverworldController.GetSelectedLevelSceneIndex ());

	}

	/// <summary>
	/// Call this function at the end of a level to persist the player fleet and auto save
	/// </summary>
	private void LevelCleanup(){
		List<GameObject> survivingShips = m_CurrentLevelManager.GetLivePlayerShips ();
		foreach (GameObject ship in survivingShips) {
			m_PlayerDataController.AddShipToFleet (ship);
		}
		GameDataController gameDataController = new GameDataController ();
		gameDataController.Save (m_OverworldDataController, m_PlayerDataController);

	}


	void OnEnable(){

		SceneManager.sceneLoaded += OnSceneFinishedLoading;

	}

	void OnDisable(){

		SceneManager.sceneLoaded -= OnSceneFinishedLoading;

	}

	/// <summary>
	/// It is necesarry to delay certain functions until after a scene is finsihed loading
	/// This function is added to the delegate function called on sceneLoaded
	/// </summary>
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
		
		if (scene.buildIndex == m_OverworldSceneIndex) {//This means we went to the overworld
			m_CurrentOverworldController = GameObject.FindGameObjectWithTag(m_OverworldControllerTag).GetComponent<OverworldController>();
			m_CurrentOverworldController.m_OverworldDataController = m_OverworldDataController;
			m_CurrentOverworldController.m_PlayerDataController = m_PlayerDataController;
			StartCoroutine (OverworldLoop ());

		} else {//This means we went to some other level
			
			m_CurrentLevelManager = Instantiate (m_GenericLevelManagerPrefab).GetComponent<LevelManager> ();
			PopulateLevelManager (scene.buildIndex, m_CurrentLevelManager);
			StartCoroutine (LevelLoop ());

		}
	}

}
