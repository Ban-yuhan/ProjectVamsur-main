using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string idleOrMoveParameter;

    [SerializeField]
    private string deathParameter;

    public void PlayIdleOrMove(float move)
    {
        if(animator != null)
        {
            bool isMove = move == 0.0f ? false : true;
            animator.SetBool(idleOrMoveParameter, isMove);
        }
    }

    public void PlayDeath()
    {
        if(animator != null)
        {
            animator.SetTrigger(deathParameter);
        }
    }
}
