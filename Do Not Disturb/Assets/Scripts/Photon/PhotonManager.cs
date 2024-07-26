using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전 입력
    private readonly string version = "1.0f";
    // 사용자 아이디 입력
    private string userID = "Bony";



    // Start is called before the first frame update
    void Start()
    {
        // 같은 움의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;
        // 유저 아이디 할당
        PhotonNetwork.NickName = userID;
        // 포톤 서버와 통신 횟수 설정. 초당 30회
        Debug.Log(PhotonNetwork.SendRate);
        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
               
    }

    //  포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("Connected To Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();

    }

    //  로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();         // 랜덤 매치메이킹 기능
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Filed {returnCode}:{message}");

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 2;
        ro.IsOpen = true;
        ro.IsVisible = true;

        // 룸 생성
        PhotonNetwork.CreateRoom("My Room", ro);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnConnected()
    {
        Debug.Log("Create Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetWork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
