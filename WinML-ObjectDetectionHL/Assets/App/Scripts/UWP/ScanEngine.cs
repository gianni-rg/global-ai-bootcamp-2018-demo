using HoloToolkitExtensions.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_WSA && !UNITY_EDITOR
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
#endif

public class ScanEngine
{
    private TimeSpan PredictionFrequency;

    private Int64 FramesCaptured;
    private int CaptureWidth;
    private int CaptureHeight;
    public Transform CameraTransform;

    ObjectDetectionHelper ModelHelper;
    IUnityScanScene UnityApp;

    public ScanEngine(int predictionFrequency = 400)
    {
        PredictionFrequency = TimeSpan.FromMilliseconds(predictionFrequency);
    }

    public async Task Inititalize(IUnityScanScene unityApp, string modelName = "", int maxDetection = 20, float probabilityThreshold = 0.1f, float iouThreshold = 0.45f)
    {
        UnityApp = unityApp;
        ModelHelper = new ObjectDetectionHelper(UnityApp);
        await ModelHelper.LoadModelAsync(modelName, maxDetection, probabilityThreshold, iouThreshold);

#if UNITY_WSA && !UNITY_EDITOR
        await InitializeCameraCapture();
        await InitializeCameraFrameReader();
#endif
    }

#if UNITY_WSA && !UNITY_EDITOR

    private MediaCapture CameraCapture;
    private MediaFrameReader CameraFrameReader;
    private async Task InitializeCameraCapture()
    {
        CameraCapture = new MediaCapture();
        MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
        settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        await CameraCapture.InitializeAsync(settings);
    }

    private async Task InitializeCameraFrameReader()
    {
        var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
        MediaFrameSourceGroup selectedGroup = null;
        MediaFrameSourceInfo colorSourceInfo = null;

        foreach (var sourceGroup in frameSourceGroups)
        {
            foreach (var sourceInfo in sourceGroup.SourceInfos)
            {
                if (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview
                    && sourceInfo.SourceKind == MediaFrameSourceKind.Color)
                {
                    colorSourceInfo = sourceInfo;
                    break;
                }
            }
            if (colorSourceInfo != null)
            {
                selectedGroup = sourceGroup;
                break;
            }
        }

        var colorFrameSource = CameraCapture.FrameSources[colorSourceInfo.Id];

        CaptureWidth = (int)colorFrameSource.CurrentFormat.VideoFormat.Width;
        CaptureHeight = (int)colorFrameSource.CurrentFormat.VideoFormat.Height;
        CameraFrameReader = await CameraCapture.CreateFrameReaderAsync(colorFrameSource);
        await CameraFrameReader.StartAsync();
    }

    public void StartPullCameraFrames()
    {
        Task.Run(async () =>
        {
            for (; ; ) // Forever = While the app runs
            {
                FramesCaptured++;
                await Task.Delay(PredictionFrequency);
                using (var frameReference = CameraFrameReader.TryAcquireLatestFrame())
                using (var videoFrame = frameReference?.VideoMediaFrame?.GetVideoFrame())
                {

                    if (videoFrame == null)
                    {
                        continue; //ignoring frame
                    }

                    if (videoFrame.Direct3DSurface == null)
                    {
                        videoFrame.Dispose();
                        continue; //ignoring frame
                    }

                    try
                    {
                        var result = await ModelHelper.EvaluateVideoFrameAsync(videoFrame).ConfigureAwait(false);
                        if (result != null)
                        {
                            Messenger.Instance.Broadcast(new ObjectRecognitionResultMessage(result, CaptureWidth, CaptureHeight, CameraTransform));
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                    }
                }
            }
        });
    }
#endif
}
