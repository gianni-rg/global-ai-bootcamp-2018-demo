using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Media;
using Windows.Storage;
#endif

public class ObjectDetectionHelper
{
    private float maxDetections = 20;
    private float probabilityThreshold = 0.1f;
    private float iouThreshold = 0.45f;
    private string ModelFilename = "ONNXModel.onnx";

    private Stopwatch TimeRecorder = new Stopwatch();
    private IUnityScanScene UnityApp;

    public ObjectDetectionHelper()
    {
        UnityApp = null;
    }

    public ObjectDetectionHelper(IUnityScanScene unityApp)
    {
        UnityApp = unityApp;
    }

#if UNITY_WSA && !UNITY_EDITOR
    private void ModifyText(string text)
    {
        System.Diagnostics.Debug.WriteLine(text);
        if (UnityApp != null)
        {
            UnityApp.ModifyOutputText(text);
        }
    }
#else
    private void ModifyText(string text)
    {
        System.Diagnostics.Debug.WriteLine(text);
    }
#endif

#if UNITY_WSA && !UNITY_EDITOR
    private ObjectDetection Model = null;

#endif
    public async Task LoadModelAsync(string modelName = "", int maxDetections = 20, float probabilityThreshold = 0.1f, float iouThreshold = 0.45f)
    {
        if (maxDetections > 0 && maxDetections <= 20)
        {
            this.maxDetections = maxDetections;
        }

        if (probabilityThreshold > 0 && probabilityThreshold <= 1)
        {
            this.probabilityThreshold = probabilityThreshold;
        }

        if (iouThreshold > 0 && iouThreshold <= 1)
        {
            this.iouThreshold = iouThreshold;
        }

        if (!string.IsNullOrWhiteSpace(modelName))
        {
            ModelFilename = modelName;
        }

        ModifyText($"Loading {ModelFilename}...");

#if UNITY_WSA && !UNITY_EDITOR
        try
        {
            TimeRecorder = Stopwatch.StartNew();

            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Data/StreamingAssets/{ModelFilename}"));
            var labelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Data/StreamingAssets/labels.txt"));

            var labels = await FileIO.ReadLinesAsync(labelFile);
            Model = new ObjectDetection(labels, maxDetections, probabilityThreshold, iouThreshold);
            await Model.InitAsync(modelFile);

            TimeRecorder.Stop();

            ModifyText($"Loaded {ModelFilename} in {TimeRecorder.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            ModifyText($"Error: {ex.Message}");
            Model = null;
        }
#endif
    }

#if UNITY_WSA && !UNITY_EDITOR
    public async Task<IList<PredictionModel>> EvaluateVideoFrameAsync(VideoFrame frame)
    {
        ModifyText(string.Empty);

        if (frame != null)
        {
            try
            {
                TimeRecorder.Restart();

                var predictions = await Model.PredictImageAsync(frame).ConfigureAwait(false);

                TimeRecorder.Stop();

                return predictions;
            }
            catch (Exception ex)
            {
                var err_message = $"Error during prediction: {ex.Message}";
                ModifyText(err_message);
            }
        }

        return null;
    }
#endif
}
