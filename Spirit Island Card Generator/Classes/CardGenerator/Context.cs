using Spirit_Island_Card_Generator.Classes.Effects;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class Context
    {
        public Settings settings;
        public Card card;
        public Random rng;
        public EffectGenerator effectGenerator;

        public enum CardTargets
        {
            NONE,
            You,
            TargetSpirit,
            Land,
            AdjacentLand,
            OriginLand
        }

        //Whether we are targeting a spirit or a land. This is different from the card target in some cases. Either a spirit targeting card with a term like "in one of target spirit's lands (do land effect)", or a land targeting card with a clause like "A spirit in target land may (do spirit effect)"
        public Target target;
        //Whether the context has mentioned a target already
        public bool targetMentioned = false;
        //This keeps track of any nested effects.
        public List<IParentEffect> chain = new List<IParentEffect>();
        //This keeps track of any conditions that apply to effects at this level.
        //So if the card targets wetland and a condition says no blight, new effects can just check the validity based on this rather than going through the whole chain.
        public List<Condition> conditions = new List<Condition>();

        public IParentEffect? Parent
        {
            get { return chain.LastOrDefault() as IParentEffect; }
        }

        public bool HasEffectAbove(Type effectType)
        {
            foreach(IParentEffect effect in chain)
            {
                if (effect.GetType() == effectType)
                    return true;
            }
            return false;
        }

        //Checks if the effect is in the chain.
        public bool IsParent(Effect parentEffect, Effect childEffect)
        {
            if (!parentEffect.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
            {
                return false;
            }
            else
            {
                return GetAllChildren(parentEffect).Contains(childEffect);
            }
        }

        private List<Effect> GetAllChildren(Effect effect)
        {
            if (!effect.GetType().GetInterfaces().Contains(typeof(IParentEffect)))
            {
                return new List<Effect>() { effect };
            }
            else
            {
                IParentEffect parent = (IParentEffect)effect;
                List<Effect> children = new List<Effect>();
                foreach(Effect childEffect in parent.GetChildren())
                {
                    children.AddRange(GetAllChildren(childEffect));
                }
                return children;
            }
        }

        //Gets all children from the last parent on the chain
        public List<Effect> GetSiblings()
        {
            IParentEffect? parent = chain.LastOrDefault();
            if (parent == null)
                return new List<Effect>();

            IEnumerable<Effect> children = parent.GetChildren();
            List<Effect> validChildren = new List<Effect>();
            foreach(Effect child in children)
            {
                if (child != null)
                    validChildren.Add(child);
            }
            return validChildren;
        }

        //Not a true duplicate since some of the properties are intentionally deep copies. It still works for anything we'd like to change though
        public Context Duplicate()
        {
            Context newContext = new Context();
            newContext.rng = this.rng;
            newContext.card = this.card;
            newContext.settings = this.settings;
            newContext.effectGenerator = this.effectGenerator;
            newContext.conditions = new List<Effects.Conditions.Condition>(this.conditions);
            newContext.target = this.target.CreateShallowCopy();
            newContext.targetMentioned = this.targetMentioned;

            newContext.chain = new List<IParentEffect>(this.chain);

            return newContext;
        }
    }
}
