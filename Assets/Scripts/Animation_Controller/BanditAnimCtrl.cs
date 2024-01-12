using System;
using UnityEngine;

public class BanditAnimCtrl : MonoBehaviour
{
    private Animator banditAnimator;

    private enum State
    {
        Idle, CombatIdle, Run, Attack, Hurt, Death, Passive
    }
    private State state;
    private void Start()
    {
        banditAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
    }

    public void SetAnimIdle()
    {
        state = State.Idle;
        banditAnimator.SetInteger("AnimState", (int)state);
    }

    public void SetAnimCombatIdle()
    {
        state = State.CombatIdle;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
    public void SetAnimRun()
    {
        state = State.Run;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
    public void SetAnimAttack()
    {
        state = State.Attack;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
    public void SetAnimHurt()
    {
        state = State.Hurt;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
    public void SetAnimDeath()
    {
        state = State.Death;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
    public void SetAnimPassive()
    {
        state = State.Passive;
        banditAnimator.SetInteger("AnimState", (int)state);
    }
}
