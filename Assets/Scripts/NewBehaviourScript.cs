using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using FishNet.Connection;

public class NewBehaviourScript : NetworkBehaviour
{
    public GameObject target;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {

        }
        else
        {
            GetComponent<Actions>().BangAction(target);
        }
    }

}
