using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Animator animctr;
    [SerializeField] private float magnitude;
    private float animStart;
    [SerializeField] private float duration;
    private float elapsedTime;
    [SerializeField] private float percentageComplete;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private PlayerMovement PlayerMovement;

    private bool idle = true;
    private bool run = true;
    private bool sprint = true;
    private bool crouch = true;
    private bool cWalk = true;
    private bool jumpStart = true;
    private bool jumpEnd = true;
    private bool air = true;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        percentageComplete = elapsedTime / duration;
    }

    public void AnimState(PlayerMovement.MovementState state)
    {
        animctr.SetBool("IsStandard", true);
        if (state == PlayerMovement.MovementState.idle)
        {
            if(idle == true)
            {
                animStart = magnitude;
                percentageComplete = 0;
                elapsedTime = 0;
                idle = false;
                run = true;
                sprint = true;
                crouch = true;
                cWalk = true;
                jumpStart = true;
                jumpEnd = true;
                air = true;
            }
            magnitude = Mathf.Lerp(animStart, 0, curve.Evaluate(percentageComplete));
            animctr.SetBool("Standard Movement", true);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Air", false);
            animctr.SetFloat("Input Magnitude", magnitude);
        }
        else if (state == PlayerMovement.MovementState.running)
        {
            if (run == true)
            {
                animStart = magnitude;
                percentageComplete = 0;
                elapsedTime = 0;
                idle = true;
                run = false;
                sprint = true;
                crouch = true;
                cWalk = true;
                jumpStart = true;
                jumpEnd = true;
                air = true;
            }
            magnitude = Mathf.Lerp(animStart, 0.5f, curve.Evaluate(percentageComplete));
            animctr.SetBool("Standard Movement", true);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Air", false);
            animctr.SetFloat("Input Magnitude", magnitude);
        }
        else if (state == PlayerMovement.MovementState.sprinting)
        {
            if (sprint == true)
            {
                animStart = magnitude;
                percentageComplete = 0;
                elapsedTime = 0;
                idle = true;
                run = true;
                sprint = false;
                crouch = true;
                cWalk = true;
                jumpStart = true;
                jumpEnd = true;
                air = true;
            }
            magnitude = Mathf.Lerp(animStart, 1, curve.Evaluate(percentageComplete));
            animctr.SetBool("Standard Movement", true);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Air", false);
            animctr.SetFloat("Input Magnitude", magnitude);
        }
        else if (state == PlayerMovement.MovementState.crouching_idle)
        {
            if (crouch == true)
            {
                animStart = magnitude;
                percentageComplete = 0;
                elapsedTime = 0;
                idle = true;
                run = true;
                sprint = true;
                crouch = false;
                cWalk = true;
                jumpStart = true;
                jumpEnd = true;
                air = true;
            }
            magnitude = Mathf.Lerp(animStart, 0, curve.Evaluate(percentageComplete));
            animctr.SetBool("Standard Movement", false);
            animctr.SetBool("Crouch", true);
            animctr.SetBool("Air", false);
            animctr.SetFloat("Crouch Magnitude", magnitude);
        }
        else if (state == PlayerMovement.MovementState.crouching_walking)
        {
            if (cWalk == true)
            {
                animStart = magnitude;
                percentageComplete = 0;
                elapsedTime = 0;
                idle = true;
                run = true;
                sprint = true;
                crouch = true;
                cWalk = false;
                jumpStart = true;
                jumpEnd = true;
                air = true;
            }
            magnitude = Mathf.Lerp(animStart, 1, curve.Evaluate(percentageComplete));
            animctr.SetBool("Standard Movement", false);
            animctr.SetBool("Crouch", true);
            animctr.SetBool("Air", false);
            animctr.SetFloat("Crouch Magnitude", magnitude);
        }
        else if (state == PlayerMovement.MovementState.jump_start)
        {
            if (jumpStart == true)
            {
                idle = true;
                run = true;
                sprint = true;
                crouch = true;
                cWalk = true;
                jumpStart = false;
                jumpEnd = true;
                air = true;
            }
            
            animctr.SetBool("Standard Movement", false);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Jump", true);
            animctr.SetBool("Air", false);
            animctr.SetBool("JumpState", true);
        }
        else if (state == PlayerMovement.MovementState.jump_end)
        {
            if (jumpEnd == true)
            {
                idle = true;
                run = true;
                sprint = true;
                crouch = true;
                cWalk = true;
                jumpStart = true;
                jumpEnd = false;
                air = true;
            }
            animctr.SetBool("Standard Movement", false);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Air", false);
            animctr.SetBool("JumpState", false);
        }
        else if (state == PlayerMovement.MovementState.air)
        {
            if (air == true)
            {
                idle = true;
                run = true;
                sprint = true;
                crouch = true;
                cWalk = true;
                jumpStart = true;
                jumpEnd = true;
                air = false;
            }
            animctr.SetBool("Standard Movement", false);
            animctr.SetBool("Crouch", false);
            animctr.SetBool("Air", true);
            animctr.SetBool("Jump", false);
            animctr.SetBool("JumpState", true);
        }

        Mathf.Clamp01(percentageComplete);
    }
}
