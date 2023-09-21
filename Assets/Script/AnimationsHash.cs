using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


internal class AnimationsHash
{
    internal static int isMovingHash = Animator.StringToHash("isMoving");
    internal static int isJumpingHash = Animator.StringToHash("Jump");
    internal static int isAttackingHash = Animator.StringToHash("Attack");
    internal static int isGroundedHash = Animator.StringToHash("isGrounded");
    internal static int isOnWallHash = Animator.StringToHash("isOnWall");
    internal static int isOnCellingHash = Animator.StringToHash("isOnCelling");
    internal static int YvelocityHash = Animator.StringToHash("Yvelocity");
    internal static int canMoveHash = Animator.StringToHash("canMove");
    internal static int closeTargetHash = Animator.StringToHash("closeTarget");
    internal static int isAliveHash = Animator.StringToHash("isAlive");
    internal static int hitTriggerHash = Animator.StringToHash("hit");
    internal static int lockVelocityHash = Animator.StringToHash("lockVelocity");
    internal static int attackCoolDownHash = Animator.StringToHash("attackCooldown");
    internal static int slideTriggerHash = Animator.StringToHash("Slide");
    internal static int shootTriggerHash = Animator.StringToHash("Shoot");
    internal static int AirAttackTrigger = Animator.StringToHash("AirAttack");
    internal static int farTargetHash = Animator.StringToHash("farTarget");
    internal static int hasTargetHash = Animator.StringToHash("hasTarget");
    internal static int isSomersaultHash = Animator.StringToHash("isSomersault");
}

