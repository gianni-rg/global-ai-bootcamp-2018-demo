using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.UX.ToolTips;
using HoloToolkitExtensions.Messaging;
using UnityEngine;

public class ObjectLabeler : MonoBehaviour, IInputClickHandler
{
    private List<GameObject> _createdObjects = new List<GameObject>();

    [SerializeField]
    private GameObject _labelObject;

    [SerializeField]
    private GameObject _labelContainer;

    [SerializeField]
    private GameObject _debugObject;

    private void Start()
    {
        Messenger.Instance.AddListener<ObjectRecognitionResultMessage>(p => lastMessage = p);
    }

    private ObjectRecognitionResultMessage lastMessage;

    private void Update()
    {
        if(lastMessage != null)
        {
            LabelObjects(lastMessage.Predictions, lastMessage.CameraWidth, lastMessage.CameraHeight, lastMessage.CameraTransform);
            lastMessage = null;
        }
    }

    public virtual void LabelObjects(IList<PredictionModel> predictions, int cameraWidth, int cameraHeight, Transform cameraTransform)
    {
        var heightFactor = cameraHeight / cameraWidth;
        var topCorner = cameraTransform.position + cameraTransform.forward - cameraTransform.right / 2f + cameraTransform.up * heightFactor / 2f;

        foreach (var prediction in predictions)
        {
            var center = prediction.GetCenter();
            var recognizedPos = topCorner + cameraTransform.right * center.x - cameraTransform.up * center.y * heightFactor;

            Debug.Log(string.Format("{0} ({1}) found", prediction.TagName, prediction.Probability));

#if UNITY_EDITOR
            _createdObjects.Add(CreateLabel(prediction.TagName, recognizedPos));
#endif
            var labelPos = DoRaycastOnSpatialMap(cameraTransform, recognizedPos);
            if (labelPos != null)
            {
                _createdObjects.Add(CreateLabel(prediction.TagName, labelPos.Value));
            }
            else
            {
                Debug.Log(string.Format("{0} - Unable to calculate real world position", prediction.TagName));
            }
        }

        if (_debugObject != null)
        {
             _debugObject.SetActive(false);
        }
    }

    private Vector3? DoRaycastOnSpatialMap(Transform cameraTransform, Vector3 recognitionCenterPos)
    {
        RaycastHit hitInfo;
        bool rayCastOk = false;
        if (SpatialMappingManager.Instance != null &&
            (rayCastOk = Physics.Raycast(cameraTransform.position, (recognitionCenterPos - cameraTransform.position), 
                out hitInfo, 10, SpatialMappingManager.Instance.LayerMask)))
        {
            return hitInfo.point;
        }
        else
        {
            if (SpatialMappingManager.Instance == null)
            {
                Debug.Log("SpatialMappingManager.Instance is null");
            }
            else if (!rayCastOk)
            {
                Debug.Log("Unable to calculate RayCast");
            }
        }
        return null;
    }

    private void ClearLabels()
    {
        foreach (var label in _createdObjects)
        {
            Destroy(label);
        }
        _createdObjects.Clear();
    }

    private GameObject CreateLabel(string text, Vector3 location)
    {
        var labelObject = Instantiate(_labelObject);
        var toolTip = labelObject.GetComponent<ToolTip>();
        toolTip.ShowOutline = false;
        toolTip.ShowBackground = true;
        toolTip.ToolTipText = text;
        toolTip.transform.position = location + Vector3.up * 0.2f;
        toolTip.transform.parent = _labelContainer.transform;
        toolTip.AttachPointPosition = location;
        toolTip.ContentParentTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        var connector = toolTip.GetComponent<ToolTipConnector>();
        connector.PivotDirectionOrient = ConnectorOrientType.OrientToCamera;
        connector.Target = labelObject;
        return labelObject;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        ClearLabels();
    }
}

