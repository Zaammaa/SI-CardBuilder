using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Spirit_Island_Card_Generator.Classes.CardGenerator
{
    public class EffectGeneratorSettings
    {
        public List<Attribute> attributes = new List<Attribute>();
        public List<Attribute> bannedAttributes = new List<Attribute>();
        public List<Type> whitelist = new List<Type>();
        public List<Type> blacklist = new List<Type>();

        public Context context;
        public EffectGeneratorSettings(Context context)
        {
            this.context = context;
        }

        public bool Matches(Type type)
        {
            if (
                !bannedAttributes.Any((bannedAtt) => { return type.GetCustomAttribute(bannedAtt.GetType()) != null; }) &&
                !context.card.effects.Any((effect) => { return effect.GetType() == type; }) &&
                !context.GetSiblings().Any((effect) => { return effect.GetType() == type; }) &&
                !blacklist.Contains(type) &&
                (whitelist.Count == 0 || whitelist.Contains(type))
            )
            {
                return true;
            }
            return false;
        }

        public static EffectGeneratorSettings GetStandardEffectSettings(Context context)
        {
            EffectGeneratorSettings effectSettings = new EffectGeneratorSettings(context);
            List<Attribute> attributes = new List<Attribute>();

            if (context.target.SpiritTarget)
            {
                attributes.Add(new SpiritEffectAttribute());
            }
            else
            {
                attributes.Add(new LandEffectAttribute());
            }

            if (context.chain.Count > 0)
            {
                attributes.Add(new ConditionalEffectAttribute());
            }
            effectSettings.attributes.AddRange(attributes);

            return effectSettings;
        }
    }
}
