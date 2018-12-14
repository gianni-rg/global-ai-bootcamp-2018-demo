using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// The Interface gives one method to implement. This method modifies the text to display in Unity
/// Any code can then call this method outside the Unity MonoBehavior object
/// </summary>
public interface IUnityScanScene
{
    void ModifyOutputText(string newText);

    Transform GetCameraTransForm();
}

public class UnityScanScene : MonoBehaviour, IUnityScanScene
{
    // Unity 3D Text object that contains the displayed TextMesh in the FOV
    public GameObject OutputText;

    // ONNX Model Name
    public string ModelName;

    // Max Predictions
    public int MaxDetections = 20;

    // Probability Threshold 
    public float ProbabilityThreshold = 0.1f;

    // IOU Threshold 
    public float IouThreshold = 0.45f;

    // Prediction Frequency (in ms)
    public int PredictionFrequency = 100;

    // TextMesh object provided by the OutputText game object
    private TextMesh OutputTextMesh;

    // String to be affected to the TextMesh object
    private string OutputTextString = string.Empty;

    // Indicate if we have to Update the text displayed
    private bool OutputTextChanged = false;

    private ScanEngine CameraScanEngine;
    private GameObject g;

    // Use this for initialization
    async void Start()
    {
        OutputTextMesh = OutputText.GetComponent<TextMesh>();
        OutputTextMesh.text = string.Empty;
        g = new GameObject();
        CameraScanEngine = new ScanEngine(PredictionFrequency);
        await CameraScanEngine.Inititalize(this, ModelName, MaxDetections, ProbabilityThreshold, IouThreshold);

#if UNITY_WSA && !UNITY_EDITOR // RUNNING ON WINDOWS
        CameraScanEngine.StartPullCameraFrames();
#else                          // RUNNING IN UNITY
        ModifyOutputText("Sorry ;-( The app is not supported in the Unity player.");
#endif
    }

    /// <summary>
    /// Modify the text to be displayed in the FOV
    /// or/and in the debug traces
    /// + Indicate that we have to update the text to display
    /// </summary>
    /// <param name="newText">new string value to display</param>
    public void ModifyOutputText(string newText)
    {
        OutputTextString = newText;
        OutputTextChanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        CameraScanEngine.CameraTransform = GetCameraTransForm();
        if (OutputTextChanged)
        {
            OutputTextMesh.text = OutputTextString;
            OutputTextChanged = false;
        }
    }

    public Transform GetCameraTransForm()
    {
        g.transform.position = CameraCache.Main.transform.position;
        g.transform.rotation = CameraCache.Main.transform.rotation;
        g.transform.localScale = CameraCache.Main.transform.localScale;
        return g.transform;
    }
}
