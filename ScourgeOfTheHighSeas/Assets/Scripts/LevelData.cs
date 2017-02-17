using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is meant to contain important information about a specific scene for example the enemies that will spawn or the events and dialogue
//the information will generaly be parsed out of an external file and held temporarily in the scene data class
public class LevelData  {

	//the index of this scene in the build editor
	public int m_SceneIndex;

	//the list of enemy ships that will be spawned in this scene
	public List<string> m_Enemies;

	public string m_Boss;

	//The mapping of events in the level to their index in the dialoguer system
	public Dictionary<string,int> m_LevelEventToEventIndex;


	//generic constructor
	public LevelData(){
		m_Enemies = new List<string> ();
		m_LevelEventToEventIndex = new Dictionary<string, int> ();
	}


}
