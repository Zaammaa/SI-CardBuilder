using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.GameConcepts
{
    public class GamePieces
    {
        public enum Piece
        {
            //Invaders
            Explorer,
            Town,
            City,
            Invader,
            //Tokens
            Beast,
            Wilds,
            Disease,
            Strife,
            Badland,
            Vitality,
            //Spirit
            Presence,
            SacredSite,
            DestroyedPresence,
            //Other
            Blight,
            Dahan,
            NoPiece
        }

        public static string ToSIBuilderString(Piece piece)
        {
            switch (piece)
            {
                case Piece.Explorer:
                    return "{explorer}";
                case Piece.Town:
                    return "{town}";
                case Piece.City:
                    return "{city}";
                case Piece.Invader:
                    return "Invader";
                case Piece.Beast:
                    return "{beast}";
                case Piece.Wilds:
                    return "{wilds}";
                case Piece.Disease:
                    return "{disease}";
                case Piece.Strife:
                    return "{strife}";
                case Piece.Badland:
                    return "{badland}";
                case Piece.Vitality:
                    return "{vitality}";
                case Piece.Presence:
                    return "{presence}";
                case Piece.SacredSite:
                    return "{sacred-site}";
                case Piece.DestroyedPresence:
                    return "{destroyed-presence}";
                case Piece.Blight:
                    return "{blight}";
                case Piece.Dahan:
                    return "{dahan}";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
