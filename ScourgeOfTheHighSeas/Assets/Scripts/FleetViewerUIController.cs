using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetViewerUIController : MonoBehaviour {

	public FleetViewerController m_FleetViewerController;

	private int m_ScreenWidth;
	private int m_ScreenHeight;

	[SerializeField] private Rect m_NextShipRect;
	[SerializeField] private Rect m_PreviousShipRect;
	[SerializeField] private Rect m_ExitButtonRect;
	[SerializeField] private Rect m_ExitConfirmRect;
	[SerializeField] private Rect m_ExitCancelRect;
	[SerializeField] private Rect m_StatsRect;
	[SerializeField] private Rect m_UpgradeRect;
	[SerializeField] private Rect m_UpgradeButtonRect;

	public Vector2 m_UpgradeButtonOffset;

	private List<ShipUpgrade> m_ShipUpgradesList;
	private ShipAttributesData m_CurrentShipStats;
	private string m_CurrentShipStatsString;

	private bool m_DisplayingExitPrompt;

	void Awake(){


		m_ScreenWidth = Screen.width;
		m_ScreenHeight = Screen.height;


		m_DisplayingExitPrompt = false;

		m_NextShipRect = RectScreenFractionToPixels (m_NextShipRect);
		m_PreviousShipRect = RectScreenFractionToPixels (m_PreviousShipRect);
		m_ExitButtonRect = RectScreenFractionToPixels (m_ExitButtonRect);
		m_ExitConfirmRect = RectScreenFractionToPixels (m_ExitConfirmRect);
		m_ExitCancelRect = RectScreenFractionToPixels (m_ExitCancelRect);
		m_StatsRect = RectScreenFractionToPixels (m_StatsRect);
		m_UpgradeRect = RectScreenFractionToPixels (m_UpgradeRect);
		m_UpgradeButtonRect = RectScreenFractionToPixels (m_UpgradeButtonRect);

	}

	void Start(){
		m_CurrentShipStats = m_FleetViewerController.GetCurrentShipAttributes ();
		m_ShipUpgradesList = m_FleetViewerController.GetUpgradeList ();

		m_CurrentShipStatsString = ShipAttributesDataToString (m_CurrentShipStats);


	}

	void OnGUI(){
		
		GUI.Box (m_StatsRect, m_CurrentShipStatsString);

		for (int i = 0; i < m_ShipUpgradesList.Count; i++) {
			
			if (GUI.Button (OffsetRect(m_UpgradeButtonRect, m_UpgradeButtonOffset * i), m_ShipUpgradesList[i].name)) {
				
				m_FleetViewerController.ApplyUpgradeToCurrentShip (m_ShipUpgradesList [i]);
				m_ShipUpgradesList.RemoveAt (i);
				m_CurrentShipStats = m_FleetViewerController.GetCurrentShipAttributes ();
				m_CurrentShipStatsString = ShipAttributesDataToString (m_CurrentShipStats);
				break;

			}
		}

		GUI.Box (m_UpgradeRect, "Available Upgrades");

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

	/// <summary>
	/// Converts a ship attribute data object to an easily understandable string form
	/// </summary>
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

	/// <summary>
	/// Takes an input rect(in pixel units) and an offset(in units of fraction of screen space) and returns the new offset rect
	/// </summary>
	/// <returns>The rect.</returns>
	/// <param name="inputRect">Input rect.</param>
	/// <param name="offset">Offset (in units of fraction of screen space).</param>
	private Rect OffsetRect(Rect inputRect, Vector2 offset){	
		
		Vector2 actualOffset = new Vector2 (offset.x * m_ScreenWidth, offset.y * m_ScreenHeight);
		return new Rect (inputRect.x + actualOffset.x, inputRect.y + actualOffset.y, inputRect.width, inputRect.height);

	}

	/// <summary>
	/// Converts a Rect from units of a fraction of screen space to pixel coordinates
	/// </summary>
	/// <returns>The rect in pixel space.</returns>
	/// <param name="inputRect">Input rect.</param>
	private Rect RectScreenFractionToPixels(Rect inputRect){
		
		return new Rect (inputRect.x * m_ScreenWidth, inputRect.y * m_ScreenHeight, inputRect.width * m_ScreenWidth, inputRect.height * m_ScreenHeight);

	}



}
