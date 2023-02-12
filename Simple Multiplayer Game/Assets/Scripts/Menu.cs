using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Globalization;

public class Menu : MonoBehaviour
{

    public TextMeshProUGUI nickField;
    public TextMeshProUGUI adressField;
    public NetworkManager networkManager;



    

    private void Start()
    {

        if(Application.platform != RuntimePlatform.WindowsServer) Time.timeScale = 0f;
  
        else Server();

            
    }





    public void Connect()
    {
        ServerManagment.PlayerConnectionName = nickField.text;
        Time.timeScale = 1f;

        string adress = adressField.text;

        adress = adress.Remove(adress.Length - 1); //Unity bug?

        if (adressField.text.Length > 1) networkManager.GetComponent<UnityTransport>().ConnectionData.Address = adress;
        else networkManager.GetComponent<UnityTransport>().ConnectionData.Address = "127.0.0.1";


        NetworkManager.Singleton.StartClient();

        Destroy(gameObject);
    }

    
    public void Host()
    {
        ServerManagment.PlayerConnectionName = nickField.text;
        Time.timeScale = 1f;
        NetworkManager.Singleton.StartHost();
        Destroy(gameObject);
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Server started.");
    }
}
