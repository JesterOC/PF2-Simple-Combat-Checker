using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFInder_Damage_Test
{
    enum eResult { critFail, Fail, Suc, CritSuc }

    public abstract class Dude
    {
        private int _HP = 0;
        private int _ToHitMod = 0;
        private DiceExpression attackExpression;
        private int _AC = 0;
        protected StreamWriter logFile;
        private bool _reactionAvailable = true;

        protected int InitialHP;

        private static readonly Random DudeRoller = new Random();

        public DiceExpression AttackExpression { get => attackExpression; set => attackExpression = value; }
        public int ToHitMod { get => _ToHitMod; set => _ToHitMod = value; }
        public int HP { get => _HP; set => _HP = value; }
        public int AC { get => _AC; set => _AC = value; }
        public bool ReactionAvailable { get => _reactionAvailable; set => _reactionAvailable = value; }

        public void Attack(Dude dudeToAttack, int mod)
        {
            // Only allow attack if they are still up
            if (dudeToAttack.HP > 0)
            {
                dudeToAttack.HandleAttack(AttackToHitRoll(), ToHitMod + mod, AttackDamageValue(), AttackAvergateDamageValue());
            }
        }

        public abstract void HandleAttack(int AttackingValue, int AttackMod, int AttackingDamage, decimal AverageDamage);

        public virtual void Reset()
        {
            HP = InitialHP;
            ReactionAvailable = true;
        }

        public void RenewReaction()
        {
            ReactionAvailable = true;
        }

        private int AttackToHitRoll()
        {
            int result = DudeRoller.Next(1, 21);
            if( result > 20)
            {
                return result;
            }
            return result;
        }

        private int AttackDamageValue()
        {
            return attackExpression.Evaluate();
        }
        private decimal AttackAvergateDamageValue()
        {
            return attackExpression.GetCalculatedAverage();
        }
    }
}
