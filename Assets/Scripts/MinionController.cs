using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class MinionController : MonoBehaviour
{
    public Animator animator;
    public string jumpTrigger = "jump";
    public string runState = "running";
    public string jumpState = "jump";
    public SplineAnimate splineAnimator;

    private bool isJumping = false;

    public void Jump()
    {
        if (isJumping) return;
        StartCoroutine(JumpSequence());
    }

    private IEnumerator JumpSequence()
    {
        isJumping = true;

        // Pausa lo spline
        if (splineAnimator != null)
            splineAnimator.Pause();

        // Triggera il jump
        if (animator != null)
            animator.SetTrigger(jumpTrigger);

        // Aspetta che l'animator entri nello stato di jump
        yield return null;
        yield return null;

        // Legge la durata dell'animazione di jump
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float jumpDuration = stateInfo.length;

        yield return new WaitForSeconds(jumpDuration);

        // Riprende lo spline
        if (splineAnimator != null)
            splineAnimator.Play();

        isJumping = false;
    }
}