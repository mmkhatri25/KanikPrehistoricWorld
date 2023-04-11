using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attacks {None, Disappear, ThrowStone, SpeedAttack, SuperAttack, FallingObjAttack, FlyingAttack, FlyingThrow, FlyingSpreadBullet, TornadoAttack, Boomerang }

[System.Serializable]
public class AttackOrder
{
    public float delayMin = 1;
    public float delayMax = 2;
    public Attacks[] attackRandomList;
}

public class Boss1AttackOrder : MonoBehaviour {
	public BOSS_1 BossTarget;
    
	public AttackOrder[] attackOrders;
    
    IEnumerator AttackCoWork;

    private void OnDrawGizmos()
    {
        if (BossTarget == null && GetComponent<BOSS_1>() != null)
            BossTarget = GetComponent<BOSS_1>();
    }

    private void OnEnable()
    {
        if (BossTarget == null && GetComponent<BOSS_1>() != null)
            BossTarget = GetComponent<BOSS_1>();
    }

    // Use this for initialization
    public void Play () {
        if (AttackCoWork != null)
            StopCoroutine(AttackCoWork);
        AttackCoWork = AttackCo();
        StartCoroutine(AttackCoWork);
        //firstEnable = false;
    }

	int current = 0;
    IEnumerator AttackCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(attackOrders[current].delayMin, attackOrders[current].delayMax));
           
            while (!BossTarget.isPlayerInRange || BossTarget.isMeleeAttacking || BossTarget.isKnockingBack)
            {
                yield return new WaitForEndOfFrame();
            }
           
            while (BossTarget.isDead || GameManager.Instance.State != GameManager.GameState.Playing)
            {
                yield return new WaitForEndOfFrame();
            }

            Attacks attackType = attackOrders[current].attackRandomList[Random.Range(0, attackOrders[current].attackRandomList.Length)];
            switch (attackType)
            {
                case Attacks.Disappear:
                    BossTarget.DisappearShowAction();
                    yield return null;
                    while (BossTarget.disapearing)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.ThrowStone:
                    BossTarget.ThrowStoneCoAction();
                    yield return new WaitForSeconds(1f);
                    break;
                case Attacks.SpeedAttack:
                    BossTarget.SpeedAttackCoAction();
                    yield return null;
                    while (BossTarget.isAttackSpeed)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.SuperAttack:
                    BossTarget.SuperAttackCoAction();
                    yield return null;
                    while (BossTarget.isSuperAttacking)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;

                case Attacks.FallingObjAttack:
                    BossTarget.FallingObjectAttackCoAction();
                    yield return null;
                    while (BossTarget.isFallingObjectAttack)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.FlyingAttack:
                    BossTarget.FlyingAttackCoAction();
                    yield return null;
                    while (BossTarget.isFlyingAttack)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.FlyingThrow:
                    BossTarget.FlyingAttackCoAction(true);
                    yield return null;
                    while (BossTarget.isFlyingAttack)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.FlyingSpreadBullet:
                    BossTarget.FlyingAttackCoAction(false, true);
                    yield return null;
                    while (BossTarget.isFlyingAttack)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.TornadoAttack:
                    BossTarget.TORNADOAttackCoAction();
                    yield return null;
                    while (BossTarget.isTornadoAttacking)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case Attacks.Boomerang:
                    BossTarget.BoomerangAttackCoAction();
                    yield return null;
                    while (BossTarget.isBoomerangeAttacking)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                default:
                    ;
                    break;
            }

            current++;
            if (current >= attackOrders.Length)
                current = 0;
        }
    }
}
