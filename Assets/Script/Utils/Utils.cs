using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Utils
{
    public static bool IsLocalPlayer(NetworkObject networkObject)
    {
        return networkObject.IsValid == networkObject.HasInputAuthority;
    }
    public static IEnumerator PlayAnimationSetStateFinish(GameObject parent, Animator animator, string clipName, bool active = true)
    {
        animator.Play(clipName);
        var animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        parent.SetActive(active);
    }
}
