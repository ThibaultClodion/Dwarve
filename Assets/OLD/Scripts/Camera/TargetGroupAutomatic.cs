using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGroupAutomatic : MonoBehaviour
{
    private CinemachineTargetGroup autoTargetGroup;
    private Transform[] oldTargets;

    public void UpdateTarget()
    {
        autoTargetGroup = GetComponent<CinemachineTargetGroup>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if(oldTargets != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                autoTargetGroup.RemoveMember(oldTargets[i]);
                oldTargets[i] = players[i].transform;
                autoTargetGroup.AddMember(oldTargets[i], 1, 0);
            }
        }
        else
        {
            oldTargets = new Transform[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                oldTargets[i] = players[i].transform;
                autoTargetGroup.AddMember(oldTargets[i], 1, 0);
            }
        }
    }

}
