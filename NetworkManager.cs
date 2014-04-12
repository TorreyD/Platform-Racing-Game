using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	public enum PlayerPosition 
	{
		Player1,
		Player2,
		None
	}
	
	
	public static PlayerPosition _playerPos;


	private const string typeName = "RunForIt";
	private const string gameName = "Room1";

	private HostData[] hostList;

	public GameObject Player1;
	public GameObject Player2;

	private bool gameStarted = false;

	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		_playerPos = PlayerPosition.Player1;
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initialization Successful!");
		SpawnPlayer1();
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
		_playerPos = PlayerPosition.Player2;
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Successfully Joined Server!");
		SpawnPlayer2();
	}

	private void SpawnPlayer1()
	{
		gameStarted = true;
		Network.Instantiate(Player1, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
		if (_playerPos == PlayerPosition.Player1)
		{
			//CamMovement._target = Player1;
		}
	}

	private void SpawnPlayer2()
	{
		gameStarted = true;
		Network.Instantiate(Player2, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
		if (_playerPos == PlayerPosition.Player2)
		{
			CamMovement._target = Player2;
		}
	}

	// Use this for initialization
	void Start () 
	{
		MasterServer.ipAddress = "127.0.0.1";
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (gameStarted) 
		{
			if (GameObject.FindGameObjectWithTag ("Player1").transform.position.x > GameObject.FindGameObjectWithTag ("finishLine").transform.position.x) {
					if (_playerPos == PlayerPosition.Player1) {
							Application.LoadLevel (1);
					} else {
							Application.LoadLevel (2);
					}
			}
			if (GameObject.FindGameObjectWithTag ("Player2").transform.position.x > GameObject.FindGameObjectWithTag ("finishLine").transform.position.x) {
					if (_playerPos == PlayerPosition.Player2) {
							Application.LoadLevel (1);
					} else {
							Application.LoadLevel (2);
					}
			}
		}
	}
}
