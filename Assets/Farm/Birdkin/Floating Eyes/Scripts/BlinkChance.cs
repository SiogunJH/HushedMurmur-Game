using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkChance : StateMachineBehaviour
{
    const string ANIMATOR_TRIGGER_BLINK = "Blink";
    float currentAnimTime = 0;
    [SerializeField] int blinkChanceInPercent = 25;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AttemptToBlink(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime % 1 < currentAnimTime) AttemptToBlink(animator);
        currentAnimTime = stateInfo.normalizedTime % 1;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(ANIMATOR_TRIGGER_BLINK);
    }

    void AttemptToBlink(Animator animator)
    {
        if (Random.Range(0, 100) < blinkChanceInPercent)
            animator.SetTrigger(ANIMATOR_TRIGGER_BLINK);
    }
}
