using UnityEngine;

public class ObjectRecognitionMessageBase
{
    public int CameraWidth { get; protected set; }
    public int CameraHeight { get; protected set; }
    public Transform CameraTransform { get; protected set; }

    public ObjectRecognitionMessageBase(int cameraWidth, int cameraHeight, Transform cameraTransform)
    {
        CameraWidth = cameraWidth;
        CameraHeight = cameraHeight;
        CameraTransform = cameraTransform;
    }
}