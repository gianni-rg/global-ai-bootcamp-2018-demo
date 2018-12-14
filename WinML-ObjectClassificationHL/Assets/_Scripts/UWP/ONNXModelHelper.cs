
using System;
using System.Diagnostics;
using System.Threading.Tasks;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Media;
using Windows.Storage;
#endif


public class ONNXModelHelper
{
    private float accuracy = 0.5f;
    private string ModelFilename = "ONNXModel.onnx";
    private Stopwatch TimeRecorder = new Stopwatch();
    private IUnityScanScene UnityApp;

    public ONNXModelHelper()
    {
        UnityApp = null;
    }

    public ONNXModelHelper(IUnityScanScene unityApp)
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
    private ONNXModel Model = null;

#endif
    public async Task LoadModelAsync(string modelName = "", float accuracy = 0.5f)
    {
        if (accuracy > 0)
            this.accuracy = accuracy;

        if (!String.IsNullOrEmpty(modelName))
        {
            ModelFilename = modelName;
        }
        ModifyText($"Loading {ModelFilename}... Patience");

#if UNITY_WSA && !UNITY_EDITOR
        try
        {
            TimeRecorder = Stopwatch.StartNew();


            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(
                new Uri($"ms-appx:///Data/StreamingAssets/{ModelFilename}"));
            Model = await ONNXModel.CreateOnnxModel(modelFile);
            TimeRecorder.Stop();
            ModifyText($"Loaded {ModelFilename}: Elapsed time: {TimeRecorder.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            ModifyText($"error: {ex.Message}");
            Model = null;
        }
#endif
    }

#if UNITY_WSA && !UNITY_EDITOR
    public async Task EvaluateVideoFrameAsync(VideoFrame frame)
    {
        if (frame != null)
        {
            try
            {
                TimeRecorder.Restart();
                ONNXModelInput inputData = new ONNXModelInput();
                inputData.Data = frame;
                var output = await Model.EvaluateAsync(inputData).ConfigureAwait(false);

                var product = output.ClassLabel.GetAsVectorView()[0];
                var loss = output.Loss[0][product];
                TimeRecorder.Stop();

                var lossStr = $"{(loss * 100.0f).ToString("#0.00")}%");
                string timing = $"[{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00}] Eval took {TimeRecorder.ElapsedMilliseconds}ms";
                string message = string.Empty;
                if (loss >= accuracy)
                {
                    message = $"{product} ({lossStr})\n";
                }

                message += timing;
                message = message.Replace("\\n", "\n");

                ModifyText(message);
            }
            catch (Exception ex)
            {
                var err_message = $"error: {ex.Message}";
                ModifyText(err_message);
            }
        }
    }
#endif




}
