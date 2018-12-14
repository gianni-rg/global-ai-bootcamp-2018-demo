using System.Collections.Generic;
using UnityEngine;

public class VideoFrameMessage : ObjectRecognitionMessageBase
{
    public IList<byte> Image { get; protected set; }

    public VideoFrameMessage(IList<byte> image, int cameraWidth, int cameraHeight, Transform cameraTransform) : base(cameraWidth, cameraHeight, cameraTransform)
    {
        Image = image;
    }
}
