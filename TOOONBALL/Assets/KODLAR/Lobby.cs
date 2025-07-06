using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button saveNicknameButton;

    private const string DefaultRoomName = "DefaultRoom"; // Varsayılan oda adı

    private void Start()
    {
        joinButton.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        saveNicknameButton.onClick.AddListener(OnSaveNicknameClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        joinButton.interactable = false; // Nickname kaydedilmeden katılmayı engelle
    }

    private void OnSaveNicknameClicked()
    {
        string nickname = nicknameInputField.text;
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning("Nickname is empty.");
            return;
        }

        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString("PlayerNickname", nickname); // Kaydetme
        PlayerPrefs.Save();

        Debug.Log($"Nickname saved: {nickname}");

        joinButton.interactable = true; // Nickname kaydedildikten sonra katılmayı etkinleştir
    }

    public void OnJoinButtonClicked()
    {
        JoinOrCreateDefaultRoom();
    }

    private void JoinOrCreateDefaultRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(DefaultRoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel("Game"); // Load the gameplay scene
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
        CreateDefaultRoom(); // Oda yoksa oluştur
    }

    private void CreateDefaultRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(DefaultRoomName, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room {DefaultRoomName} created successfully.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to create room: {message}");
    }
}