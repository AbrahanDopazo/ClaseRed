using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigPlayerReference : MonoBehaviour
{
    public static VRRigPlayerReference Singleton;
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Awake()
    {
        Singleton = this;
    }
}


