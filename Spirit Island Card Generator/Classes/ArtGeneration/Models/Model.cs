using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.ArtGeneration.Models
{
    public abstract class Model
    {
        public abstract string NAME { get; }
        public abstract string FILENAME { get; }
        public abstract string HASH { get; }
        public abstract int WEIGHT { get; }
        public abstract List<string> ALLOWED_EMBEDDINGS { get; }
        public abstract List<string> EXTRA_NEGATIVE_TAGS { get; }

        public string GetModelString()
        {
            return FILENAME + $" [{HASH}]";
        }
    }
}
