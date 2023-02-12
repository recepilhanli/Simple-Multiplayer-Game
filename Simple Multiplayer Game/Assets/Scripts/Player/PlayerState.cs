using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{

    private ServerManagment server;
    public TextMeshPro nickText;

    public static Dictionary<ulong, PlayerData> playerData;


    public NetworkVariable<short> playerClient = new NetworkVariable<short>(default,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

 
    public override void OnNetworkSpawn()
    {

       

        if (IsServer) playerData = new Dictionary<ulong, PlayerData>();

        if (gameObject.name == "Server" && ( IsHost ||  IsClient )) this.enabled = false;


        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }
           

        ConnectionServerRPC(NetworkManager.LocalClientId,ServerManagment.PlayerConnectionName);
        playerClient.Value = Convert.ToInt16(NetworkManager.LocalClientId);
        playerName.Value = ServerManagment.PlayerConnectionName;
        Invoke("UpdateHostServerRPC", 0.25f); //waiting


        ResetMyPos();

        server = GameObject.Find("Server").GetComponent<ServerManagment>();
        

    }

    [ServerRpc(RequireOwnership = false)]

    void ConnectionServerRPC(ulong client, string playerName)
    {
        playerData.Add(client, new PlayerData(playerName, -1));

        Debug.LogFormat("Connection: {0} (id: {1})", playerName, client);


        UpdateAllPlayersClientRPC();
    }



    [ServerRpc(RequireOwnership = false)]

    

    void UpdateHostServerRPC()
    {
        if (!IsHost) return;

        UpdatePlayers();

    }



    [ClientRpc]

    void UpdateAllPlayersClientRPC(ClientRpcParams clientRpcParams  = default)
    {

        Invoke("UpdatePlayers", 0.1f); //waiting for networkvariable

    }






    [ClientRpc]

    public void ResetMyPosClientRPC(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        ResetMyPos();

    }




    public void UpdatePlayers()
    {
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("player");

        foreach (GameObject player in players)
        {
            PlayerState state = player.GetComponent<PlayerState>();
            if (state == null) continue;

            if (state.playerClient.Value % 2 == 0) player.GetComponent<Renderer>().material.color = Color.red;
            else player.GetComponent<Renderer>().material.color = Color.blue;


            state.nickText.text = state.playerName.Value.ToString();
        }
    }

    public void ResetMyPos()
    {
        if (!IsOwner) return;
        short team;

        if (playerClient.Value % 2 == 0) team = 1;
        else team = 2;

        GameObject pos = GameObject.Find("teamSpawn_" + team);
        transform.position = new Vector3(pos.transform.position.x, 0.625f, pos.transform.position.z);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }



   

}
