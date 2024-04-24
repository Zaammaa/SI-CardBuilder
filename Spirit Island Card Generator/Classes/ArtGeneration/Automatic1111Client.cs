using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.ArtGeneration
{
    internal class Automatic1111Client
    {
        public static readonly Automatic1111Client stableDiffusionClient = new Automatic1111Client();
        private readonly HttpClient client = new HttpClient();
        //public static string IMAGE_SAVE_DIR = MetaManager.OUTPUTS_FOLDER;
        public static int MAX_QUEUE_SIZE = 200;
        public event EventHandler<SimpleEventArgs> FinishImageProcessing;

        private Queue<StableDiffusionSettings> prompts = new Queue<StableDiffusionSettings>();
        private bool running = false;

        public Automatic1111Client()
        {
            client.Timeout = TimeSpan.FromMinutes(30);
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
        }

        public void AddPromptToQueue(StableDiffusionSettings settings)
        {
            if (prompts.Count < MAX_QUEUE_SIZE)
            {
                prompts.Enqueue(settings);
                if (!running)
                    Run();
            }
        }

        public bool QueueFull()
        {
            return prompts.Count >= MAX_QUEUE_SIZE;
        }

        public async void Run()
        {
            running = true;
            while (prompts.Count > 0 && running)
            {
                try
                {
                    await Task.Run(() => GeneratePrompt(prompts.Dequeue()));
                }
                catch (Exception e)
                {

                }

            }
            running = false;
        }

        public async Task GeneratePrompt(StableDiffusionSettings settings)
        {
            string prompt = settings.prompt;
            Dictionary<string, string> options = new Dictionary<string, string>();

            await SwitchModel(settings);

            string json = JsonConvert.SerializeObject(settings);

            var response = await client.PostAsync(@"http://127.0.0.1:7860/sdapi/v1/txt2img", new StringContent(json, Encoding.UTF8, "application/json"));

            var responseString = await response.Content.ReadAsStringAsync();

            StableDiffusionResponse responseJson = JsonConvert.DeserializeObject<StableDiffusionResponse>(responseString);
            StableDiffusionSettings responseSettings = responseJson.parameters as StableDiffusionSettings;

            int batch_count = 0;
            //foreach (string imageString in responseJson.images)
            //{
            string imageString = responseJson.images.First();
            byte[] imageBytes = Convert.FromBase64String(imageString);
            string filepath = settings.saveFilename;
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            File.WriteAllBytes(filepath, imageBytes); //TODO: fix batch files. After we changed the output path to be set in the settings file, the filename will be same if there's more than one file in the batch
            if (settings.saveSettingsJSON)
                File.WriteAllText(filepath.Replace(".png", ".json"), responseJson.info);

            batch_count++;
            //}
            if (!settings.testImage)
            {
                FinishImageProcessing?.Invoke(this, new SimpleEventArgs(responseSettings));
                settings.ImageFinished(responseJson);
            }
                
        }

        private async Task SwitchModel(StableDiffusionSettings settings)
        {

            string url = "http://127.0.0.1:7860";

            string option_payload = "{\"sd_model_checkpoint\": \"" + settings.model + "\"}";

            var response = await client.PostAsync($"{url}/sdapi/v1/options", new StringContent(option_payload, Encoding.UTF8, "application/json"));

        }

        public async Task RefreshSDCheckpoints()
        {
            var response = await client.PostAsync(@"http://127.0.0.1:7860/sdapi/v1/refresh-checkpoints", new StringContent("", Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> IsStableDiffusionRunning()
        {
            var response = client.GetAsync(@"http://127.0.0.1:7860/internal/ping").Result;
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
