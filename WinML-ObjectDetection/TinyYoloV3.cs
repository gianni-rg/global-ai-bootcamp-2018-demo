using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

namespace WinMLObjectDetection
{
    public sealed class TinyYoloV3Input
    {
        public VideoFrame Image { get; set; }
    }

    public sealed class TinyYoloV3Output
    {
        public List<float> Grid { get; set; }
        public TinyYoloV3Output()
        {
            Grid = new List<float>();
            Grid.AddRange(new float[21125]);  // Total size of TinyYOLO output
        }
    }

    public sealed class TinyYoloV3
    {
        private LearningModelPreview _learningModel;
        public static async Task<TinyYoloV3> CreateTinyYoloV3(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            TinyYoloV3 model = new TinyYoloV3();
            model._learningModel = learningModel;
            return model;
        }
        public async Task<TinyYoloV3Output> EvaluateAsync(TinyYoloV3Input input) {
            TinyYoloV3Output output = new TinyYoloV3Output();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(_learningModel);
            binding.Bind("image", input.Image);
            binding.Bind("grid", output.Grid);
            LearningModelEvaluationResultPreview evalResult = await _learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
