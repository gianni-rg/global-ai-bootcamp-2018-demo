
using System.Collections.Generic;
using UnityEngine;

public class ObjectRecognitionResultMessage : ObjectRecognitionMessageBase
{
    public IList<PredictionModel> Predictions { get; protected set; }


    public ObjectRecognitionResultMessage(IList<PredictionModel> predictions, int frameWidth, int frameHeight, Transform cameraTransform) :  base(frameWidth, frameHeight, cameraTransform)
    {
        Predictions = predictions;
    }
}
