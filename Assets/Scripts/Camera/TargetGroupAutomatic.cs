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

        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerPrefab");

        for (int i = 0; i < players.Length; i++)
        {
            autoTargetGroup.AddMember(players[i].transform, 1, 0);
        }
    }
}
