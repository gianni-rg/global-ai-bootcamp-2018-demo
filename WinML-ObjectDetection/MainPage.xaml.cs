﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Media;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinMLObjectDetection.Yolo9000;

namespace WinMLObjectDetection
{
    public sealed partial class MainPage : Page
    {
        private TinyYoloV3 _model;
        private IList<YoloBoundingBox> _boxes = new List<YoloBoundingBox>();
        private readonly YoloWinMlParser _parser = new YoloWinMlParser();
        private readonly SolidColorBrush _lineBrushYellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly SolidColorBrush _lineBrushGreen = new SolidColorBrush(Windows.UI.Colors.Green);
        private readonly SolidColorBrush _fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        private readonly double _lineThickness = 2.0;
        private uint _yoloCanvasActualWidth;
        private uint _yoloCanvasActualHeight;
        private Stopwatch _stopwatch;

        public MainPage()
        {
            InitializeComponent();
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadYoloOnnxModel();
            GetCameraSize();

            Window.Current.SizeChanged += Current_SizeChanged;

            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            GetCameraSize();
        }

        private void GetCameraSize()
        {
            _yoloCanvasActualWidth = (uint)CameraPreview.ActualWidth;
            _yoloCanvasActualHeight = (uint)CameraPreview.ActualHeight;
        }

        private async void LoadYoloOnnxModel()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///TinyYoloV3.onnx"));
            _model = await TinyYoloV3.CreateTinyYoloV3(file); 

        }

        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e?.VideoFrame?.SoftwareBitmap == null) return;

            var input = new TinyYoloV3Input { Image = e.VideoFrame };
            _stopwatch = Stopwatch.StartNew();
            var output = await _model.EvaluateAsync(input);
            _stopwatch.Stop();
            _boxes = _parser.ParseOutputs(output.Grid.ToArray());

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextBlockInformation.Text = $"{1000f / _stopwatch.ElapsedMilliseconds,4:f1} FPS on Width {_yoloCanvasActualWidth} x Height {_yoloCanvasActualHeight}";
                DrawOverlays(e.VideoFrame);
            });
        }

        private void DrawOverlays(VideoFrame inputImage)
        {
            YoloCanvas.Children.Clear();
            if (_boxes.Count <= 0) return;
            var filteredBoxes = _parser.NonMaxSuppress(_boxes, 5, .5F);

            foreach (var box in filteredBoxes)
                DrawYoloBoundingBox(box, YoloCanvas);
        }

        private void DrawYoloBoundingBox(YoloBoundingBox box, Canvas overlayCanvas)
        {
            // process output boxes
            var x = (uint)Math.Max(box.X, 0);
            var y = (uint)Math.Max(box.Y, 0);
            var w = (uint)Math.Min(overlayCanvas.ActualWidth - x, box.Width);
            var h = (uint)Math.Min(overlayCanvas.ActualHeight - y, box.Height);

            // fit to current canvas and webcam size
            x = _yoloCanvasActualWidth * x / 416;
            y = _yoloCanvasActualHeight * y / 416;
            w = _yoloCanvasActualWidth * w / 416;
            h = _yoloCanvasActualHeight * h / 416;

            var rectStroke = box.Label == "person" ? _lineBrushGreen : _lineBrushYellow;

            var r = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Tag = box,
                Width = w,
                Height = h,
                Fill = _fillBrush,
                Stroke = rectStroke,
                StrokeThickness = _lineThickness,
                Margin = new Thickness(x, y, 0, 0)
            };

            var tb = new TextBlock
            {
                Margin = new Thickness(x + 4, y + 4, 0, 0),
                Text = $"{box.Label} ({Math.Round(box.Confidence, 4)})",
                FontWeight = FontWeights.Bold,
                Width = 126,
                Height = 21,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var textBack = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Width = 134,
                Height = 29,
                Fill = rectStroke,
                Margin = new Thickness(x, y, 0, 0)
            };

            overlayCanvas.Children.Add(textBack);
            overlayCanvas.Children.Add(tb);
            overlayCanvas.Children.Add(r);
        }
    }
}
