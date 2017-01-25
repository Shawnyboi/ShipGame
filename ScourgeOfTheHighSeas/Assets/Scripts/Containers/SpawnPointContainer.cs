using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will exist in the scen when it is loaded and it will be found by the level manager upon the level managers instantiation
/// The class will contain all the spawn point positions of the level
/// </summary>
public class SpawnPointContainer : MonoBehaviour{
	public Transform[] m_PlayerSpawnPoints;
	public Transform[] m_ComSpawnPointsFirstWave;
	public Transform[] m_ComSpawnPointsSecondWave;
	public Transform[] m_ComSpawnPointsThirdWave;


}