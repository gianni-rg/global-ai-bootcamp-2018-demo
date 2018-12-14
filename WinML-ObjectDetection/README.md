# Windows UWP and WinML Demo - YOLOv3 Object Detection

Perform object detection in a Windows 10 UWP app, using a pre-trained ONNX model.

## WinML-ObjectClassificationHL

This sample is based on the work by **Bruno Capuano**, detailed in the post and sample [How to convert Tiny-YoloV3 model in CoreML format to Onnx and use it in a Windows 10 App](https://elbruno.com/2018/07/10/winml-how-to-convert-tiny-yolov3-model-in-coreml-format-to-onnx-and-use-it-in-a-windows10-app/).

Here you can find the minimal sample code. The objectives are:

- Capture frames from the webcam
- Load the Machine Learning model (ONNX)
- Execute the model and get the predictions from each captured frame
- Show predictions (classes/bounding boxes)

## Software Versions

- Windows 10 (1803, April 2018 Update)
- Visual Studio 2017 version 15.9.4
- Windows SDK version 10.0.17134.0

## License

Copyright (c) 2018 Gianni Rosa Gallina.

Based on the work by Bruno Capuano.
Copyright (c) 2018 Bruno Capuano.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.