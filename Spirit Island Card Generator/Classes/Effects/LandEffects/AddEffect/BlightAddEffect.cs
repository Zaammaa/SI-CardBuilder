using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    internal class BlightAddEffect : AddEffect
    {
        public override string Name => "Add Blight";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Fire, Element.Earth }; } }
        public override double BaseProbability { get { return .04; } }
        public override int Complexity { get { return 3; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Blight;

        public override List<Type> IncompatibleEffects {  get { return new List<Type>() { typeof(VitalityAddEffect), typeof(RemoveBlightEffect)}; } }

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 }
        };

        public override double effectStrength => -3.0;

        public override IPowerLevel Duplicate()
        {
            BlightAddEffect effect = new BlightAddEffect();
            effect.Context = Context.Duplicate();
            effect.addAmount = addAmount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            addAmount = 1;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Blighted) || context.card.Range.range == 0)
                return false;
            return true;
        }
    }
}
