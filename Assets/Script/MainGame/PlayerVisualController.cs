using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pivotGunTransform;
    [SerializeField] private Transform canvasTransform;
    private readonly int isMovingHash = Animator.StringToHash("IsWalking");
    private bool isFacingRight = true;
    private Vector3 originalPlayerScale = Vector3.one;
    private Vector3 originalCanvasScale = Vector3.one;
    private Vector3 originalGunPivotScale = Vector3.one;
    private void Start()
    {
        originalPlayerScale = this.transform.localScale;
        originalCanvasScale = canvasTransform.localScale;
        originalGunPivotScale = pivotGunTransform.localScale;
    }
    public void RenderVisuals(Vector2 velocity)
    {
        var isMoving = velocity.x > 0.1f || velocity.x < -0.1f;

        animator.SetBool(isMovingHash, isMoving);
    }

    public void UpdateScaleTransforms(Vector2 velocity)
    {
        if (velocity.x > 0.1f)
        {
            isFacingRight = true;
        }
        else if (velocity.x < -0.1f)
        {
            isFacingRight = false;
        }

        SetLocalScaleBasedonDir(gameObject, originalPlayerScale);
        SetLocalScaleBasedonDir(canvasTransform.gameObject, originalCanvasScale);
        SetLocalScaleBasedonDir(pivotGunTransform.gameObject, originalGunPivotScale);
    }

    private void SetLocalScaleBasedonDir(GameObject obj, Vector3 originalScale)
    {
        var yValue = originalScale.y;
        var zValue = originalScale.z;
        var xValue = isFacingRight ? originalPlayerScale.x : -originalPlayerScale.x;
        obj.transform.localScale = new Vector3(xValue, yValue, zValue);
    }
}
