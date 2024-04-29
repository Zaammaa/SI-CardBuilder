using OpenQA.Selenium.DevTools;
using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.TransformingEffects
{
    [LandEffect]
    [CustomEffect(5)]
    internal class AnyToAny : TransformationEffect
    {
        protected Piece _fromPiece;
        protected Piece _toPiece;
        public override Piece FromPiece => _fromPiece;

        public override Piece ToPiece => _toPiece;

        public override double BaseProbability => 0.1;

        public override int Complexity => 3;

        public override IPowerLevel Duplicate()
        {
            AnyToAny effect = new AnyToAny();
            effect.Context = Context.Duplicate();
            effect._fromPiece = _fromPiece;
            effect._toPiece = _toPiece;
            return effect;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            return true;
        }

        protected override void InitializeEffect()
        {
            List<Piece> pieceOptions = Enum.GetValues<Piece>().ToList();

            pieceOptions.Remove(Piece.NoPiece);
            pieceOptions.Remove(Piece.Strife);
            pieceOptions.Remove(Piece.SacredSite);
            pieceOptions.Remove(Piece.Invader);
            pieceOptions.Remove(Piece.Presence);
            _toPiece = Utils.ChooseRandomListElement(pieceOptions, Context.rng);

            pieceOptions.Add(Piece.Presence);
            pieceOptions.Remove(Piece.DestroyedPresence);
            pieceOptions.Remove(_toPiece);

            if (Context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoInvaders))
            {
                pieceOptions.Remove(Piece.City);
                pieceOptions.Remove(Piece.Town);
                pieceOptions.Remove(Piece.Explorer);
            }

            if (Context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoBuildings))
            {
                pieceOptions.Remove(Piece.City);
                pieceOptions.Remove(Piece.Town);
            }

            if (Context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoCity))
            {
                pieceOptions.Remove(Piece.City);
            }

            if (Context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.NoDahan))
            {
                pieceOptions.Remove(Piece.Dahan);
            }

            if (Context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Noblight))
            {
                pieceOptions.Remove(Piece.Blight);
            }

            _fromPiece = Utils.ChooseRandomListElement(pieceOptions, Context.rng);
        }
    }
}
