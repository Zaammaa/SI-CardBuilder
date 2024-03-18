using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions
{
    [CostCondition]
    internal class ForgetThisCardCondition : Condition
    {
        public override double BaseProbability => 0.08;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 4;

        public override double DifficultyMultiplier
        {
            get
            {
                return 0.35;
            }
        }
        protected bool isFast = false;
        public override string ConditionText {
            get
            {
                if (isFast)
                {
                    return "You may forget this card, gaining its elements. If you do";
                } else
                {
                    return "You may forget this card. If you do";
                }
            }
        }

        public override bool ChooseEasierCondition(Context context)
        {
            return false;
        }

        public override bool ChooseHarderCondition(Context context)
        {
            return false;
        }

        public override IPowerLevel Duplicate()
        {
            ForgetThisCardCondition condition = new ForgetThisCardCondition();
            return condition;
        }

        public override void Initialize(Context context)
        {
            isFast = context.card.Fast;
        }

        public override bool IsValid(Context context)
        {
            return true;
        }
    }
}
