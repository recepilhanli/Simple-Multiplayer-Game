using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using static Unity.VisualScripting.Member;

public class Ball : NetworkBehaviour
{

    NetworkVariable<int> Player1Score = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<int> Player2Score = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public TextMeshProUGUI Player1ScoreText;
    public TextMeshProUGUI Player2ScoreText;

    public float BounceForce = 100f;
    public Rigidbody body;

    public Collider HitBox;

    public override void OnNetworkSpawn()
    {  
        ScoresServerRPC();
        body.WakeUp();


       
    }

  

    [ServerRpc(RequireOwnership = false)]
    public void ScoresServerRPC()
    {
        UpdateScoreClientRPC(Player1Score.Value, Player2Score.Value);

    }



    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("player")) return;

        if (collision.gameObject.CompareTag("goal"))
        {
            Debug.Log("Goal1");
            if (collision.gameObject.name == "Goal0" && IsServer) GiveScore(2);
            else if (collision.gameObject.name == "Goal1" && IsServer) GiveScore(1);

            UpdateScoreClientRPC(Player1Score.Value, Player2Score.Value);
            if (IsServer) ResetBallPos();
            UpdateScoreForHost();
            body.velocity = Vector3.zero;

        }
        else
        {

            short Force = 1; //reducing rpc size




            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsClient) BallForceServerRPC(Force, contact.normal);
            }



        }

    }


   




    [ServerRpc(RequireOwnership = false)]
    public void BallForceServerRPC(short Force, Vector3 Pos)
    {
        body.AddForce(25 * BounceForce * Force * Pos);

        if (body.velocity.magnitude > 50)
        {
            body.velocity = body.velocity.normalized * 50;
        }
        Debug.Log("test");
    }




    public void GiveScore(short team)
    {
        if(team == 1) Player1Score.Value++;
        else Player2Score.Value++;

    }



    public void ResetBallPos()
    {
  
        gameObject.transform.position = new Vector3(0, 0.266f,0);




            GameObject[] players = GameObject.FindGameObjectsWithTag("player");

            foreach (GameObject player in players)
            {
                PlayerState state = player.GetComponent<PlayerState>();
                if (state == null) continue;

                if(IsHost) state.ResetMyPos();

                state.ResetMyPosClientRPC();

            }

        




    }



    [ClientRpc]
    private void UpdateScoreClientRPC(int team1score, int team2score, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        Debug.Log("Score updated.");
        Player1ScoreText.text = team1score.ToString();
        Player2ScoreText.text = team2score.ToString();

    }


 
    private void UpdateScoreForHost()
    {
        if (!IsHost) return;

        Player1ScoreText.text = Player1Score.Value.ToString();
        Player2ScoreText.text = Player2Score.Value.ToString();

    }



    void Update()
    {

       
        
    }
}
