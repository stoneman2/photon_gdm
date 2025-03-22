using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static IEnumerator PlayAnimationSetStateFinish(GameObject parent, Animator animator, string clipName, bool active = true)
    {
        animator.Play(clipName);
        var animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);
        parent.SetActive(active);
    }
}
