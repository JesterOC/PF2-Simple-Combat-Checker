using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFInder_Damage_Test
{
    class TwoHDude : Dude
    {

        public TwoHDude(int inHP, int inToHitMod, DiceExpression inAttackExpression, int inAC, StreamWriter inLogFile)
        {
            logFile = inLogFile;
            HP = inHP;
            InitialHP = inHP;
            ToHitMod = inToHitMod;
            AttackExpression = inAttackExpression;
            AC = inAC;
        }

        public override void HandleAttack(int AttackingValue, int AttackMod, int AttackingDamage, decimal AverageDamage)
        {
            eResult attackResult = eResult.Fail;

            int effectiveAC = AC;

            if (AttackingValue + AttackMod >= effectiveAC)
            {
                attackResult = eResult.Suc;
                if (AttackingValue + AttackMod >= effectiveAC + 10)
                {
                    attackResult++;
                }
            }
            if (AttackingValue == 20 && attackResult != eResult.CritSuc)
            {
                attackResult++;
            }
            if (attackResult == eResult.CritSuc)
            {
                AttackingDamage *= 2;
            }
            if (attackResult == eResult.Fail || attackResult == eResult.critFail)
            {
                MainWindow.Log("SB Attack Missed! " + AttackingValue + "+" + AttackMod, logFile);
            }else
            {
                MainWindow.Log("SB Attack Hit! " + AttackingValue + "+" + AttackMod + " For " + AttackingDamage + " points of damage", logFile);
            }

            if (attackResult >= eResult.Suc)
            {
                HP -= AttackingDamage;
            }
            
        }
    }
}
