using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.ArtGeneration.Models
{
    internal class SIArt : Model
    {
        public override string NAME { get { return "SIArt"; } }
        public override string FILENAME { get { return "v1-5-pruned-emaonly.ckpt"; } }
        public override string HASH { get { return "cc6cb27103"; } }
        public override int WEIGHT { get { return 10; } }
        public override List<string> ALLOWED_EMBEDDINGS
        {
            get
            {
                return new List<string>()
                {

                };
            }
        }
        public override List<string> EXTRA_NEGATIVE_TAGS
        {
            get
            {
                return new List<string>()
                {

                };
            }
        }
    }
}
