using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PathFInder_Damage_Test
{
    class SwordBoardDude : Dude
    {
        private const int CritOvershootValue = 10;
        private const int Nat20 = 20;
        private int _ShieldHardness;
        private int _ShieldHP;
        private int _CurrentShieldHP;
        private int _ShieldBT;
        private int _DespShieldUse;

        private bool _BrokenShield = false;
        private int shieldBonus = 2;
        private enum eShieldStrat { PROTECT, ALWAYS, NEVER };
        private eShieldStrat shieldStrat;

        public int ShieldHP
        {
            get => _ShieldHP;
            set
            {
                _ShieldHP = value;
                CurrentShieldHP = value;
                _ShieldBT = value / 2;
            }
        }

        public SwordBoardDude( int inHP,int inToHitMod,DiceExpression inAttackExpression,int inAC,int ShieldACValue, int inShieldHP,int ShieldHard, ComboBox shieldTactics, StreamWriter inLogFile)
        {
            logFile = inLogFile;
            HP = inHP;
            InitialHP = inHP;
            ToHitMod = inToHitMod;
            AttackExpression = inAttackExpression;
            AC = inAC;
            ShieldHP = inShieldHP;
            ShieldHardness = ShieldHard;
            shieldBonus = ShieldACValue;
            switch(shieldTactics.SelectedIndex)
            {
                case 0:
                    shieldStrat = eShieldStrat.PROTECT;
                    break;
                case 1:
                    shieldStrat = eShieldStrat.ALWAYS;
                    break;
                case 2:
                    shieldStrat = eShieldStrat.NEVER;
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();
            CurrentShieldHP = _ShieldHP;
            BrokenShield = false;
            DespShieldUse = 0;
        }

        public int ShieldHardness { get => _ShieldHardness; set => _ShieldHardness = value; }
        public bool BrokenShield { get => _BrokenShield; set => _BrokenShield = value; }
        public int CurrentShieldHP { get => _CurrentShieldHP; set => _CurrentShieldHP = value; }
        public int DespShieldUse { get => _DespShieldUse; set => _DespShieldUse = value; }

        /// <summary>Handles the attack.</summary>
        /// <param name="DieRoll">The die roll.</param>
        /// <param name="AttackMod">The attack mod. This already has the Multi attack penalty baked in</param>
        /// <param name="AttackingDamage">The attacking damage. The damage that will be applied.</param>
        /// <param name="AverageDamage">The average damage. This is what the player being attacked would know about the potential damage</param>
        public override void HandleAttack(int DieRoll, int AttackMod, int AttackingDamage, decimal AverageDamage)
        {
            eResult attackResult = eResult.Fail;
            bool useShield = !BrokenShield && shieldStrat != eShieldStrat.NEVER && ReactionAvailable;

            int effectiveAC = useShield ? AC+ shieldBonus : AC;

            if (DieRoll + AttackMod >= effectiveAC)
            {
                attackResult = eResult.Suc;
                if(DieRoll + AttackMod >= effectiveAC + CritOvershootValue)
                {
                    attackResult++;
                }
            }
            if (DieRoll == Nat20 && attackResult != eResult.CritSuc)
            {
                attackResult++;
            }
            if(attackResult == eResult.CritSuc)
            {
                AttackingDamage *= 2;
                // The player will know if the hit they are blocking is a crit
                AverageDamage *= 2;
            }

            if (attackResult >= eResult.Suc)
            {
                if (useShield )
                {
                    // Use shield if we are protecting it if it won't get broken
                    // Or if we have so few hp that we have a 50% chance of dieing
                    // Or use it if we are not protecting it
                    if ( (shieldStrat == eShieldStrat.PROTECT && (CurrentShieldHP - Math.Max(AverageDamage - ShieldHardness, 0) > _ShieldBT) )||
                         (shieldStrat == eShieldStrat.PROTECT && (HP - AverageDamage <= 0)) ||
                          shieldStrat == eShieldStrat.ALWAYS )
                    {
                        ReactionAvailable = false;
                        MainWindow.Log("TH Attack Hit! " + DieRoll + "+" + AttackMod + " For " + AttackingDamage + " points of damage", logFile);
                        int reducedDamage = Math.Max(AttackingDamage - ShieldHardness, 0);
                        CurrentShieldHP -= reducedDamage;
                        if(CurrentShieldHP <= _ShieldBT)
                        {
                            BrokenShield = true;
                        }
                        HP -= reducedDamage;
                        // Bookkeeping just keep track of how often this happened
                        if(shieldStrat == eShieldStrat.PROTECT && (HP - AverageDamage <= 0))
                        {
                            DespShieldUse++;
                        }

                        MainWindow.Log("Shield Block reduced damage to " + reducedDamage + " points of damage", logFile);
                        if (BrokenShield)
                        {
                            if (CurrentShieldHP <= 0)
                            {
                                MainWindow.Log("Shield was destroyed with this hit!", logFile);
                            }
                            else
                            {
                                MainWindow.Log("Shield was broken with this hit", logFile);
                            }
                        }
                    }
                    else
                    {
                        MainWindow.Log("TH Attack Hit! " + DieRoll + "+" + AttackMod + " For " + AttackingDamage + " points of damage", logFile);
                        MainWindow.Log("Shield block not used!", logFile);
                        HP -= AttackingDamage;
                    }
                }
                else
                {
                    MainWindow.Log("TH Attack Hit! " + DieRoll + "+" + AttackMod + " For " + AttackingDamage + " points of damage", logFile);
                    if(BrokenShield)
                    {
                        MainWindow.Log("Shield was previously broken!", logFile);
                    }
                    if (!ReactionAvailable)
                    {
                        MainWindow.Log("reaction not available!", logFile);
                    }
                    HP -= AttackingDamage;
                }
            }
            else
            {
                MainWindow.Log("TH Attack Missed! " + DieRoll + "+" + AttackMod, logFile);
            }
        }
    }
}
