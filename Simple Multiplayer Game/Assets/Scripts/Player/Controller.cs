using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Controller : NetworkBehaviour
{
 
    public float Speed = 5f;
    public Rigidbody rb;

    private bool texting = false;

    private Slider DodgeSlider;
    private Image DodgeEffect;
    private TMP_InputField ChatInput;

    private ServerManagment server;
    private PlayerState state;

    private Ball ball;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            this.enabled = false;
            return;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("ball")) return;
            short Force = 1; //reducing rpc size
       
            Force += (short)(rb.velocity.magnitude / 5);



        foreach (ContactPoint contact in collision.contacts)
        {
            ball.BallForceServerRPC(Force, -contact.normal);
        }


    }

    private void Start()
    {
        if(!IsOwner) return;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        DodgeSlider = GameObject.Find("DodgeSlider").GetComponent<Slider>();
        DodgeEffect = GameObject.Find("DodgeEffect").GetComponent<Image>();
        ChatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();
        server = GameObject.Find("Server").GetComponent<ServerManagment>();
        ball = GameObject.Find("Ball").GetComponent<Ball>();

        state = gameObject.GetComponent<PlayerState>();
    }



    private void DodgeEffectUpdate()
    {
        if(DodgeEffect.color.a > 0)
        {
            Color c = DodgeEffect.color;
            DodgeEffect.color = new Color(c.r, c.g, c.b, c.a-Time.deltaTime);
        }

    }
    

    public void Dodge(float h, float v)
    {
        DodgeEffectUpdate();

        if (Input.GetKeyDown(KeyCode.Space) && DodgeSlider.value <= 0 && (h != 0 || v != 0)) //dodge
        {
            rb.AddForce(h * 350, 0, v * 350);
            DodgeSlider.value = 1f;
            Color c = DodgeEffect.color;
            DodgeEffect.color = new Color(c.r, c.g, c.b, 0.5f);
        }
        else if (DodgeSlider.value > 0)
        {
            DodgeSlider.value -= Time.deltaTime / 4f;
        }
        else if (DodgeSlider.value < 0) DodgeSlider.value = 0;
    }


    private void Chatting()
    {
        if(Input.GetKeyUp(KeyCode.Return) && texting == false)
        {
            ChatInput.ActivateInputField();
            texting = true;
        }
        else if (Input.GetKeyUp(KeyCode.Return) && texting == true)
        {
            string text = state.playerName.Value.ToString() + ": " + ChatInput.text + System.Environment.NewLine;
            if(ChatInput.text.Length > 2) server.SendTextServerRPC(text);
            ChatInput.DeactivateInputField();
            texting = false;
            ChatInput.text = string.Empty;
        }
        else if(texting == false) ChatInput.DeactivateInputField();
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;


        Chatting();
        if (texting == true) return;

        if (transform.position.y == 0f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 0.65f, transform.position.z);
        }

        float h = Input.GetAxis("Horizontal") * Time.deltaTime * Speed;
        float v = Input.GetAxis("Vertical") * Time.deltaTime * Speed;
     

        rb.AddForce(h, 0, v);

        Dodge(h,v);


        if (Input.GetMouseButton(0)) //rotate
        {

            float x = Input.GetAxis("Mouse X") * Time.deltaTime * Speed*3;

            transform.Rotate(0, x, 0);


        }


    }

}
