using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    public List<MinionController> minions = new List<MinionController>();
    public void JumpMinions()
    {
        AudioManager.Instance.PlayMinionJump();
        foreach (MinionController minion in minions)
        {
            minion.Jump();
        }
    }
}
