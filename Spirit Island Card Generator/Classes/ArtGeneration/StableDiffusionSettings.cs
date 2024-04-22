using Spirit_Island_Card_Generator.Classes.ArtGeneration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.ArtGeneration
{
    internal class StableDiffusionSettings
    {
        public bool enable_hr = false;
        public double denoising_strength = 0.7;
        public int firstphase_width = 0;
        public int firstphase_height = 0;
        public double upscale_by = 1;
        public string prompt = "";
        public List<string> styles = new List<string>();
        public double seed = 123456789;
        public double subseed = 123456789;
        public double subseed_strength = 0;
        public int seed_resize_from_h = -1;
        public int seed_resize_from_w = -1;
        public int batch_size = 1;
        public int n_iter = 1;
        public int steps = 20;
        public double cfg_scale = 9;
        public int width = 540;
        public int height = 370;
        public bool restore_faces = false;
        public bool tiling = false;
        public string negative_prompt = "bad anatomy, poor quality";
        public string sampler_index = "Euler a";
        public string model = new SIArt().GetModelString();

        public string saveFilename = "";
        public bool saveSettingsJSON = true;
        public bool testImage = false;

        public event EventHandler<SimpleEventArgs>? AfterImageProcesses;

        public static List<Model> Models = new List<Model>()
        {
            new SIArt()
        };

        public static List<string> NegativePrompts = new List<string>()
        {
            "bad anatomy, poor quality"
        };

        public static Model ChooseRandomModel()
        {
            Random random = new Random();
            //Model[] choices = Models.ToArray();
            Dictionary<Model, int> choices = new Dictionary<Model, int>();
            foreach(Model model in Models)
            {
                choices.Add(model, model.WEIGHT);
            }

            return Utils.ChooseWeightedOption(choices, random);
        }

        public static string[] GetModelNames()
        {
            List<string> models = new List<string>();
            foreach(Model model in Models)
            {
                models.Add(model.NAME);
            }
            return models.ToArray();
        }

        public static Model GetModelFromHash(string hash)
        {
            foreach(Model model in Models)
            {
                if (model.HASH.Equals(hash))
                    return model;
            }
            return null;
        }

        public static Model GetModelFromName(string name)
        {
            if (name.Equals("Random"))
            {
                return ChooseRandomModel();
            }
            else
            {
                foreach (Model model in Models)
                {
                    if (model.NAME.Equals(name))
                        return model;
                }
            }
            return null;
        }

        public static string ChooseRandomNegativePrompt()
        {
            string[] choices = NegativePrompts.ToArray();

            Random random = new Random();
            int choice = random.Next(choices.Length);
            return choices[choice];
        }

        public void ImageFinished(StableDiffusionResponse response)
        {
            AfterImageProcesses?.Invoke(this,new SimpleEventArgs(response));
        }
    }
}
