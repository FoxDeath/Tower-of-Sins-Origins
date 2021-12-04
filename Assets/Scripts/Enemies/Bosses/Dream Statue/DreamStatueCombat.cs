using System.Collections;
using UnityEngine;

public class DreamStatueCombat : EnemyCombat
{
    #region Attributes
    [SerializeField] GameObject lightning;

    [SerializeField] int noOfLightningsInPhaseOne = 1;
    [SerializeField] int noOfLightningsInPhaseTwo = 4;

    private float? lightningTargetYPos;
    [SerializeField] float timeBetweenLightningsPhaseOne = 1f;
    [SerializeField] float timeBetweenLightningsPhaseTwo = 0.5f;
    //Set telegraph durations according to their lengths in the lightning attack anims.
    [SerializeField] float telegraphDuraionPhaseOne = 0.75f;
    [SerializeField] float telegraphDuraionPhaseTwo = 0.25f;
    #endregion

    #region Coroutines
    protected override IEnumerator AuxiliaryBehaviourBeforeCombat(Attack currentAttack)
    {
        if(astarAI.GetTarget() != null)
        {
            if(currentAttack.name.ToUpper().Contains("P2A2"))
            {
                StartCoroutine((astarAI as DreamStatueMovement).TeleportBehaviour());
            }
            else
            {
                FinishAuxiliaryBehaviour();
            }
        }

        yield return null;
    }

    protected override IEnumerator AuxiliaryBehaviourDuringCombat(Attack currentAttack)
    {
        if(astarAI.GetTarget() != null)
        {
            if(currentAttack.name.ToUpper().Contains("P1A2"))
            {
                StartCoroutine(LightningBehaviour(noOfLightningsInPhaseOne, timeBetweenLightningsPhaseOne, telegraphDuraionPhaseOne));
            }
            else if(currentAttack.name.ToUpper().Contains("P2A3"))
            {
                StartCoroutine(LightningBehaviour(noOfLightningsInPhaseTwo, timeBetweenLightningsPhaseTwo, telegraphDuraionPhaseTwo));
            }
        }

        FinishAuxiliaryBehaviour();
        
        yield return null;
    }

    //Spawns lightning attacks according to parameters. 
    private IEnumerator LightningBehaviour(int noOfLightnings, float timeInBetween, float telegraphDuraion)
    {
        yield return new WaitForSeconds(telegraphDuraion);

        for(int i = 0; i < noOfLightnings; i++)
        {
            //Records where ground level is. That's where it will spawn the lightnings.
            lightningTargetYPos = Physics2D.Raycast(astarAI.GetTarget().position, Vector2.down, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")).point.y;

            GameObject lightningPrefab = Instantiate(lightning, new Vector2(astarAI.GetTarget().position.x, lightningTargetYPos.Value), Quaternion.identity);

            lightningPrefab.GetComponent<EnemyWeaponController>().ProjectileSetUp(this);
            
            GameObject.Destroy(lightningPrefab, 0.6f);

            if((i + 1) < noOfLightnings)
            {
                yield return new WaitForSeconds(timeInBetween);
            }
        }
    }
    #endregion
}
