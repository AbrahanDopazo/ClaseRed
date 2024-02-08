using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerPrefab : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public UnityEngine.UI.Slider healthSlider;
    public const float baseHealth = 100f;
    public NetworkVariable<float> health = new NetworkVariable<float>(baseHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (Renderer mesh in meshToDisable)
                mesh.enabled = false;
        }

        health.OnValueChanged += HealthUpdated;
    }

    private void HealthUpdated(float previousvalue, float newvalue)
    {
        Debug.Log(newvalue);
        healthSlider.value = newvalue / baseHealth;
    }
        
    public void TakeDamage(float amount){
        if (health.Value < 0f)
        {
            health.Value = 0;
            //Fución de muerte de personaje
        }
        else
            health.Value -= amount;
    }
    public void Healing(float amount){
        if (health.Value + amount > baseHealth)
            health.Value = baseHealth;
        else
            health.Value += amount;
    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            root.transform.position = VRRigPlayerReference.Singleton.root.transform.position;
            root.transform.rotation = VRRigPlayerReference.Singleton.root.transform.rotation;
            head.transform.position = VRRigPlayerReference.Singleton.head.transform.position;
            head.transform.rotation = VRRigPlayerReference.Singleton.head.transform.rotation;
            leftHand.transform.position = VRRigPlayerReference.Singleton.leftHand.transform.position;
            leftHand.transform.rotation = VRRigPlayerReference.Singleton.leftHand.transform.rotation;
            rightHand.transform.position = VRRigPlayerReference.Singleton.rightHand.transform.position;
            rightHand.transform.rotation = VRRigPlayerReference.Singleton.rightHand.transform.rotation;
        }
    }
}
