using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupAutomatic : MonoBehaviour
{
    private CinemachineTargetGroup autoTargetGroup;

    public void UpdateTarget()
    {
        autoTargetGroup = GetComponent<CinemachineTargetGroup>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) 
        {
            autoTargetGroup.AddMember(player.transform, 1, 0);
        }
    }

}
