# HoloLens Windows ML - Custom Vision Object Classification

Train your Custom Vision model in the Cloud and run it locally on the HoloLens.

## WinML-ObjectClassificationHL

This sample is based on the work by **Sebastien Bovo**, detailed in the post and sample, [HoloLens WinML and Custom Vision](https://github.com/Microsoft/Windows-AppConsult-Samples-MixedReality/tree/master/HoloLensWinML).

Here you can find the minimal sample code. The objectives are:

- Capture frames from the HoloLens camera
- Load the MachineLearning model
- Execute the model and get the predictions from each captured frame

> **Note:** Windows version 1809 i.e. build > 17738 is needed to be able to run the Windows.AI.MachineLearning APIs on HoloLens

## Software Versions

- Windows 10 (1809, October 2018 Update) for HoloLens (also on PC if you want to test it locally)
- Visual Studio 2017 version 15.9.4
- Windows SDK version 10.0.17763.0
- Unity 2017.4.16f1

## License

MIT License

Copyright (c) 2018 Gianni Rosa Gallina.

Based on the work by Sebastien Bovo.
Copyright (c) 2018 Microsoft.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
