using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.ArtGeneration
{
    public class StableDiffusionResponse
    {
        public List<string> images;
        public StableDiffusionSettings parameters;
        public string info;
    }
}
