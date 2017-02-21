using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetViewerUIController : MonoBehaviour {

	public FleetViewerController m_FleetViewerController;

	private int m_ScreenWidth;
	private int m_ScreenHeight;

	private static Rect m_NextShipRect;
	private static Rect m_PreviousShipRect;
	private static Rect m_ExitButtonRect;
	private static Rect m_ExitConfirmRect;
	private static Rect m_ExitCancelRect;
	private static Rect m_StatsRect;

	public float m_NextPreviousButtonPosX;
	public float m_NextPreviousButtonPosY;
	public float m_NextPreviousButtonWidth;
	public float m_NextPreviousButtonHeight;
	public float m_NextPreviousButtonOffset;

	public float m_ExitButtonPosX;
	public float m_ExitButtonPosY;
	public float m_ExitButtonWidth;
	public float m_ExitButtonHeight;
	public float m_ExitOptionsOffset;

	public float m_StatsPosX;
	public float m_StatsPosY;
	public float m_StatsWidth;
	public float m_StatsHeight;


	private ShipAttributesData m_CurrentShipStats;
	private string m_CurrentShipStatsString;

	private bool m_DisplayingExitPrompt;

	void Awake(){
		m_ScreenWidth = Screen.width;
		m_ScreenHeight = Screen.height;


		m_DisplayingExitPrompt = false;

		m_NextShipRect = new Rect (m_ScreenWidth * m_NextPreviousButtonPosX, m_ScreenHeight * m_NextPreviousButtonPosY, m_ScreenWidth * m_NextPreviousButtonWidth, m_ScreenHeight * m_NextPreviousButtonHeight);
		m_PreviousShipRect = new Rect (m_ScreenWidth * (m_NextPreviousButtonPosX + m_NextPreviousButtonOffset), m_ScreenHeight * m_NextPreviousButtonPosY, m_ScreenWidth * m_NextPreviousButtonWidth, m_ScreenHeight * m_NextPreviousButtonHeight);

		m_ExitButtonRect = new Rect (m_ScreenWidth * m_ExitButtonPosX, m_ScreenHeight * m_ExitButtonPosY, m_ScreenWidth * m_ExitButtonWidth, m_ScreenHeight * m_ExitButtonHeight);

		m_ExitConfirmRect = m_ExitButtonRect;
		m_ExitConfirmRect.y += m_ExitOptionsOffset * m_ScreenHeight;

		m_ExitCancelRect = m_ExitConfirmRect;
		m_ExitCancelRect.y += m_ExitOptionsOffset * m_ScreenHeight;

		m_StatsRect = new Rect(m_ScreenWidth * m_StatsPosX, m_ScreenHeight * m_StatsPosY, m_ScreenWidth * m_StatsWidth,  m_ScreenHeight * m_StatsHeight);

	}

	void OnGUI(){
		GUI.Box (m_StatsRect, m_CurrentShipStatsString);

		if (GUI.Button (m_NextShipRect, "Next Ship")) {
			OnNextShipButton ();
		}

		if (GUI.Button (m_PreviousShipRect, "Previous Ship")) {
			OnPreviousShipButton ();
		}
		if (!m_DisplayingExitPrompt) {
			if (GUI.Button (m_ExitButtonRect, "Exit")) {
				m_DisplayingExitPrompt = true;
			}
		} else {
			GUI.Box (m_ExitButtonRect, "Return To OverWorld?");
			if (GUI.Button (m_ExitConfirmRect, "Yes")) {
				m_FleetViewerController.ExitFleetViewer ();
			}
			if (GUI.Button (m_ExitCancelRect, "No")){
				m_DisplayingExitPrompt = false;
			}
		}
	}

	private string ShipAttributesDataToString(ShipAttributesData shipStats){
		return "Ship Stats: " + "\n"
			+"Type: " + shipStats.shipName + "\n" 
			+ "Hull Strength: " + shipStats.currentHullStrength + " / " + shipStats.maxHullStrength + "\n"
			+ "Damage: " + shipStats.damage + "\n"
			+ "Range: " + shipStats.range + "\n"
			+ "Accuracy: " + shipStats.percentAccuracy +"%\n"
			+ "Reload Time: " + shipStats.reloadTime +"\n";
	}

	/// <summary>
	/// This function gets called when the next ship button is pressed
	/// It tells the fleet viewer controller to go to next ship and it displays that ships stats
	/// </summary>
	private void OnNextShipButton(){
		m_FleetViewerController.GoToNextShip ();
		m_CurrentShipStats = m_FleetViewerController.GetCurrentShipAttributes ();
		m_CurrentShipStatsString = ShipAttributesDataToString (m_CurrentShipStats);

	}

	/// <summary>
	/// This function gets called when the previous ship button is pressed
	/// It tells the fleet viewer controller to go to previous ship and it displays that ships stats
	/// </summary>
	private void OnPreviousShipButton(){
		m_FleetViewerController.GoToPreviousShip ();
		m_CurrentShipStats = m_FleetViewerController.GetCurrentShipAttributes ();
		m_CurrentShipStatsString = ShipAttributesDataToString (m_CurrentShipStats);

	}




}
