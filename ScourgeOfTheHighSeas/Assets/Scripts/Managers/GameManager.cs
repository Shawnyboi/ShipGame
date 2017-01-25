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
	public OverworldController m_CurrentOverworldController;
	private OverworldDataController m_OverworldDataController;
	private PlayerDataController m_PlayerDataController;

	private string m_OverworldControllerTag;

	public LevelManager m_GenericLevelManagerPrefab;
	//public OverworldController m_OverworldControllerPrefab;

	//we need to know the overworld scene index as we will return to it many times
	private int m_OverworldSceneIndex;

	void Awake (){
		DontDestroyOnLoad (gameObject);// we want the game manager to stay around all the time

	}



	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
	
	}



	/// <summary>
	/// This will get called after the game manager is instantiated to set some receive certain member variable references
	/// </summary>
	public void InitializeGameManager(OverworldDataController overworldDataController, PlayerDataController playerDataController){
		m_OverworldDataController = overworldDataController;
		m_PlayerDataController = playerDataController;
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

	private void GoToLevel(int levelSceneIndex){
		SceneManager.LoadScene (levelSceneIndex);
		//See OnSceneFinishedLoading for additional logic that this causes
	}


	/// <summary>
	/// This function will be called in the game manager and it will fill the current level manager with ships based on the scene
	/// the level manager being populated it passed in as a 
	/// </summary>
	public void PopulateLevelManagerShips(int sceneIndex, LevelManager levelManager){

		GameObject[] comShips = m_OverworldDataController.GetEnemyShipsForLevel (sceneIndex);

		int playerShipsToSpawn = Mathf.Min (m_CurrentLevelManager.m_PlayerShips.Length, m_PlayerDataController.m_PlayerFleet.Count);

		for (int i = 0; i < playerShipsToSpawn; i++) {

			levelManager.m_PlayerShips [i] = m_PlayerDataController.m_PlayerFleet[i]; //fill the player ships in the level manager with the first ships in the player fleet
		}

		for (int i = 0; i < levelManager.m_ComShipsFirstWave.Length; i++) {

			levelManager.m_ComShipsFirstWave [i] = comShips[Random.Range (0, comShips .Length)];

		}

		for (int i = 0; i < levelManager.m_ComShipsSecondWave.Length; i++) {

			levelManager.m_ComShipsSecondWave [i] = comShips [Random.Range (0, comShips .Length)];

		}

		for (int i = 0; i < levelManager.m_ComShipsThirdWave.Length; i++) {

			levelManager.m_ComShipsThirdWave [i] = comShips [Random.Range (0, comShips .Length)]; //fill the com ship arrays with random ships

		}
	}

	/// <summary>
	/// this coroutine starts the battleloop coroutine and thereby keeps track if the level is over
	/// When the level ends it takes the game back to the overworld;
	/// </summary>
	/// <returns>The loop.</returns>
	public IEnumerator LevelLoop(){
		
		yield return StartCoroutine(m_CurrentLevelManager.BattleLoop());
		List<GameObject> survivingShips = m_CurrentLevelManager.GetLivePlayerShips ();
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


	void OnEnable(){

		SceneManager.sceneLoaded += OnSceneFinishedLoading;

	}

	void OnDisable(){

		SceneManager.sceneLoaded -= OnSceneFinishedLoading;

	}

	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
		
		if (scene.buildIndex == m_OverworldSceneIndex) {//This means we went to the overworld
			
			StartCoroutine (OverworldLoop ());

		} else {//This means we went to some other level
			
			m_CurrentLevelManager = Instantiate (m_GenericLevelManagerPrefab).GetComponent<LevelManager> ();
			PopulateLevelManagerShips (scene.buildIndex, m_CurrentLevelManager);
			StartCoroutine (LevelLoop ());

		}
	}

}
