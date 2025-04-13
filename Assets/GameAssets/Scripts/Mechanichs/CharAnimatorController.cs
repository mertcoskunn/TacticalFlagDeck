using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimatorController : StateMachineBehaviour
{
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameCharacter gameChar = animator.GetComponentInParent<GameCharacter>();
        if (gameChar != null)
        {
            gameChar.AnimationEnd(); 
        }
    }
}
