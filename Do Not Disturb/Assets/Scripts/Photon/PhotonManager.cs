using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0f";
    private string userID = "Bony";

    private GameObject player;
    private ResourceManager resourceManager;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userID;
        PhotonNetwork.ConnectUsingSettings();

        resourceManager = FindObjectOfType<ResourceManager>();

        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found in the scene.");
        }
        else
        {
            Debug.Log("ResourceManager found.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        ro.IsVisible = true;

        PhotonNetwork.CreateRoom("My Room", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}.{player.Value.ActorNumber}");
        }

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);

        player = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        Laboratory laboratory = FindObjectOfType<Laboratory>();
        if (laboratory != null)
        {
            laboratory.SetPlayer(player.GetComponent<PlayerMovement>());
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SetPlayerMovement(player.GetComponent<PlayerMovement>());
        }

        if (resourceManager != null && PhotonNetwork.IsMasterClient)
        {
            if (resourceManager.photonView == null)
            {
                Debug.LogError("PhotonView on ResourceManager is null.");
            }
            else
            {
                resourceManager.photonView.RPC("RPC_SpawnAllPrefabs", RpcTarget.AllBuffered);
            }
        }
    }


    void Update()
    {
    }
}
