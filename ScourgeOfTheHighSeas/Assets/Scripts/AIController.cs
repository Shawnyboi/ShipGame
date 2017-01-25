using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class wil be in charge of controlling the enemy ships
/// </summary>
public class AIController : MonoBehaviour {


	//The AI needs to have a refernce to all the ships in the level so it can do things with them, 
	//this is because it has no eyes or hands with which to play the game
	private List<GameObject> m_CurrentComShips;
	private List<GameObject> m_CurrentPlayerShips;

	//this flag iss used to make sure run assault doesn't get called while it's running
	private bool m_RunningAssault;

	//this variable wil serve to keep track of the AI s overall behavior
	private string m_AIDirective;


	void Update(){
		if (m_AIDirective == "on attack" && m_RunningAssault == false) {
			StartCoroutine (RunAssault ());
		}
	}

	/// <summary>
	/// this function should be called by the level manager when the level starts in order to set certain variables for the AI
	/// </summary>
	public void SetUpAI(List<GameObject> comShips, List<GameObject> playerShips, string directive = "on attack"){
		
		m_AIDirective = directive; 
		m_CurrentComShips = comShips;
		m_CurrentPlayerShips = playerShips;

	}

	//This gets called when a ship is destroyed to make sure inactive ships don't get targetted
	public void RemoveDeadShip(){
		m_CurrentComShips.RemoveAll (ship => ship.activeSelf == false);
		m_CurrentPlayerShips.RemoveAll (ship => ship.activeSelf == false);
	}

	/// <summary>
	/// This coroutine gives all the com ships without targets a random target from the list of PlayerShips
	/// </summary>
	/// <returns>The assault.</returns>
	private IEnumerator RunAssault(){
		
		if (!m_RunningAssault) {
			
			m_RunningAssault = true;

			while (m_CurrentPlayerShips.Count > 0) { //while there are player ships

				foreach (GameObject ship in m_CurrentComShips) {

					if (ship.GetComponent<ShipFireController> ().CurrentlyHasTarget () == false) { // check if a ship has no target
						GameObject randomPlayerShip = m_CurrentPlayerShips [Random.Range (0, m_CurrentPlayerShips.Count)]; //if not, give them a random one
						ship.GetComponent<ShipFireController> ().SetNewTarget (randomPlayerShip);
						ship.GetComponent<ShipMovementController> ().SetDestination (ship.transform.position); //cancel current destination
						StartCoroutine (ship.GetComponent<ShipMovementController> ().TrackTarget ());//start tracking it

					}
				}

				yield return null;

			}
			yield return null;
			m_RunningAssault = false;

		} else {
			
			yield return null;

		}
	}
		

}
