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
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (Renderer mesh in meshToDisable)
                mesh.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            Debug.Log(root.transform.position);
            Debug.Log(VRRigPlayerReference.Singleton.root.transform.position);
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
