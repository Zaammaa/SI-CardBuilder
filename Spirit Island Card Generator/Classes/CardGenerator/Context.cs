using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class Context
    {
        public Settings settings;
        public Card card;
        public Random rng;
        public EffectGenerator effectGenerator;

        //Whether we are targeting a spirit or a land. This is different from the card target in some cases. Either a spirit targeting card with a term like "in one of target spirit's lands (do land effect)", or a land targeting card with a clause like "A spirit in target land may (do spirit effect)"
        public Target target;
        //This keeps track of any nested effects.
        public List<IGeneratorOption> chain = new List<IGeneratorOption>();
        //This keeps track of any conditions that apply to effects at this level.
        //So if the card targets wetland and a condition says no blight, new effects can just check the validity based on this rather than going through the whole chain.
        public List<Condition> conditions = new List<Condition>();
    }
}
