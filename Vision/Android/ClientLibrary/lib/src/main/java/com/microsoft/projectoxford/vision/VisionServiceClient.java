//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
package com.microsoft.projectoxford.vision;

import com.microsoft.projectoxford.vision.contract.AnalysisInDomainResult;
import com.microsoft.projectoxford.vision.contract.AnalysisResult;
import com.microsoft.projectoxford.vision.contract.Model;
import com.microsoft.projectoxford.vision.contract.ModelResult;
import com.microsoft.projectoxford.vision.contract.OCR;
import com.microsoft.projectoxford.vision.rest.VisionServiceException;

import java.io.IOException;
import java.io.InputStream;

public interface VisionServiceClient {
    public AnalysisResult analyzeImage(String url, String[] visualFeatures, String[] details) throws VisionServiceException;

    public AnalysisResult analyzeImage(InputStream stream, String[] visualFeatures, String[] details) throws VisionServiceException, IOException;

    public AnalysisInDomainResult analyzeImageInDomain(String url, Model model) throws VisionServiceException;

    public AnalysisInDomainResult analyzeImageInDomain(String url, String model) throws VisionServiceException;

    public AnalysisInDomainResult analyzeImageInDomain(InputStream stream, Model model) throws VisionServiceException, IOException;

    public AnalysisInDomainResult analyzeImageInDomain(InputStream stream, String model) throws VisionServiceException, IOException;

    public AnalysisResult describe(String url, int maxCandidates) throws VisionServiceException;

    public AnalysisResult describe(InputStream stream, int maxCandidates) throws VisionServiceException, IOException;

    public ModelResult listModels() throws VisionServiceException;

    public OCR recognizeText(String url, String languageCode, boolean detectOrientation) throws VisionServiceException;

    public OCR recognizeText(InputStream stream, String languageCode, boolean detectOrientation) throws VisionServiceException, IOException;

    public byte[] getThumbnail(int width, int height, boolean smartCropping, String url) throws VisionServiceException, IOException;

    public byte[] getThumbnail(int width, int height, boolean smartCropping, InputStream stream) throws VisionServiceException, IOException;
}
