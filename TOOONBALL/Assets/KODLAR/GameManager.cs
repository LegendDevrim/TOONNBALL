using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    void Start()
    {
        if (!PhotonNetwork.InRoom) return;

        // Takım alınır
        string teamStr = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        Team playerTeam = (Team)System.Enum.Parse(typeof(Team), teamStr);

        // Prefab seçimi
        string prefabName = playerTeam == Team.Red ? "RedPlayer" : "BluePlayer";
        Transform spawnPoint = GetSpawnPoint(playerTeam);

        // Instantiate
        PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);
    }

    Transform GetSpawnPoint(Team team)
    {
        Transform[] spawnArray = team == Team.Red ? redSpawnPoints : blueSpawnPoints;
        return spawnArray[Random.Range(0, spawnArray.Length)];
    }
}
