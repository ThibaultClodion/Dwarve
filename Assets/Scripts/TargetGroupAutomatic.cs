using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupAutomatic : MonoBehaviour
{
    public GameObject playerParentGO;
    private CinemachineTargetGroup autoTargetGroup;

    public void UpdateTarget()
    {
        autoTargetGroup = GetComponent<CinemachineTargetGroup>();

        Transform[] players = playerParentGO.GetComponentsInChildren<Transform>();

        foreach (Transform player in players) 
        {
            autoTargetGroup.AddMember(player, 1, 0); 
        }
    }

}
