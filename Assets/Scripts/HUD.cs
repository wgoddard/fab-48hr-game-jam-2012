using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {
	
	public GUISkin guiSkin;
	public Player p1;
	public Player p2;
	public Player p3;
	public Player p4;
	
	private Player[] players;
	private Player m_winningPlayer;
	
	public Texture red;
	public Texture blue;
	public Texture yellow;
	public Texture green;
	
	public GUITexture winnerTex;
	
	private float m_winTimeout = 0.0f;
	
	private bool m_victory = false;
	private bool m_waitingForPlayers = true;

	// Use this for initialization
	void Start () 
	{
		players = new Player[4];
		players[0] = p1;
		players[1] = p2;
		players[2] = p3;
		players[3] = p4;
	}
	
	public bool IsReady()
	{
		foreach (Player p in players)
		{
			if (!p.IsReady())
			{
				return false;
			}
			if (p.IsOverrideStart())
			{
				foreach (Player p2 in players)
				{
					p2.ForceReady();
				}
				m_waitingForPlayers = false;
				return true;
			}
		}
		
		foreach (Player p in players)
		{
			p.HideTut();
		}
		m_waitingForPlayers = false;
		return true;
	}
	
	public bool IsVictory()
	{
		return m_victory;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check for victory
		if (!m_victory)
		{
			int numMonsters = 0;
			
			foreach (Player p in players)
			{
				if (p.IsMonster())
				{
					numMonsters++;
				}
				else
				{
					m_winningPlayer = p;
				}
			}
			
			if (numMonsters >= 3)
			{
				m_victory = true;
				m_winTimeout = 5.0f;
				AudioSource.PlayClipAtPoint(m_winningPlayer.PlayerWonClip, Camera.mainCamera.transform.position,0.5f);
			}
		}
		
		if (m_winTimeout > 0.0f)
		{
			m_winTimeout -= 1.0f / 60.0f;
			if (m_winTimeout < 0.0f)
			{				
				Application.LoadLevel (0);		
			}
		}
		
		if (p1.Health <= 20)
		{
			guiSkin.customStyles[0].normal.textColor = new Color(1, 0, 0, 1);
		}
		if (p2.Health <= 20)
		{
			guiSkin.customStyles[1].normal.textColor = new Color(1, 0, 0, 1);
		}
		if (p3.Health <= 20)
		{
			guiSkin.customStyles[2].normal.textColor = new Color(1, 0, 0, 1);
		}
		if (p4.Health <= 20)
		{
			guiSkin.customStyles[3].normal.textColor = new Color(1, 0, 0, 1);
		}
	}
	
	private string GUIString(Player player)
	{
		if (player.IsReady())
		{
			return "" + player.Health;
		}
		else
		{
			return "" + player.Health;
		}
	}
	
	void OnGUI()
	{
		GUI.skin = guiSkin;

		GUI.Box(new Rect(0,0, 150, 75), GUIString(p1), guiSkin.customStyles[0]);
		GUI.Box (new Rect (0,75,150,75), GUIString(p2),guiSkin.customStyles[1]);
		GUI.Box (new Rect (0,150,150,75), GUIString(p3),guiSkin.customStyles[2]);
		GUI.Box (new Rect (0,225,150,75), GUIString(p4),guiSkin.customStyles[3]);
		
		//if (m_waitingForPlayers)
		//{
		//	GUI.TextField(new Rect(220, 350, 150, 75), (p1.IsReady()) ? "Ready" : "Press A");
		//	GUI.TextField(new Rect(380, 350, 150, 75), (p2.IsReady()) ? "Ready" : "Press A");
		//	GUI.TextField(new Rect(540, 350, 150, 75), (p3.IsReady()) ? "Ready" : "Press A");
		//	GUI.TextField(new Rect(700, 350, 150, 75), (p4.IsReady()) ? "Ready" : "Press A");
		//}
		
		if (m_victory)
		{
			//GUI.Label(new Rect(Screen.width / 3, Screen.height / 3, 300, 100), m_winningPlayer.ColourName + " Player Wins!");
			winnerTex.guiTexture.enabled = true;
			switch (m_winningPlayer.ColourName)
			{
			case "Red":
				winnerTex.guiTexture.texture = red;
				break;
			case "Blue":
				winnerTex.guiTexture.texture = blue;
				break;
			case "Green":
				winnerTex.guiTexture.texture = green;
				break;
			case "Orange":
				winnerTex.guiTexture.texture = yellow;
				break;
			default:
				break;
			}
		}
	}
}
