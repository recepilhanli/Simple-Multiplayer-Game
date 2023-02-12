using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;
public class ServerManagment : NetworkBehaviour
{

    public static string PlayerConnectionName;

    public TextMeshProUGUI ChatTMP;

    NetworkVariable<FixedString512Bytes> ChatText = new NetworkVariable<FixedString512Bytes>();


    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        Invoke("UpdateText", 0.2f);
    }


    [ClientRpc]
    public void UpdateTextClientRPC(ClientRpcParams clientRpcParams = default)
    {
        Invoke("UpdateText", 0.1f);
    }

    private void UpdateText()
    {
        ChatTMP.text = ChatText.Value.ToString();
    }


    [ServerRpc(RequireOwnership =false)]

    public void SendTextServerRPC(string text)
    {
        Debug.Log(text);
        string newtext = ChatText.Value.ToString();

        newtext = newtext.Insert(0, text);

        ChatText.Value = newtext;

        UpdateTextClientRPC();
    }






}




