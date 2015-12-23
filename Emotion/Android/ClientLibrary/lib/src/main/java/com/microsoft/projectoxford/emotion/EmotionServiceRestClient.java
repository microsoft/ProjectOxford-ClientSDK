//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK Github:
// https://github.com/Microsoft/ProjectOxfordSDK-Windows
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
package com.microsoft.projectoxford.emotion;

import com.google.gson.Gson;
import com.microsoft.projectoxford.emotion.contract.FaceRectangle;
import com.microsoft.projectoxford.emotion.contract.RecognizeResult;
import com.microsoft.projectoxford.emotion.rest.EmotionServiceException;
import com.microsoft.projectoxford.emotion.rest.WebServiceRequest;

import org.apache.commons.io.IOUtils;

import java.io.IOException;
import java.io.InputStream;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class EmotionServiceRestClient implements EmotionServiceClient {
    private static final String serviceHost = "https://api.projectoxford.ai/emotion/v1.0";
    private static final String FACE_RECTANGLES = "faceRectangles";
    private WebServiceRequest restCall = null;
    private Gson gson = new Gson();

    public EmotionServiceRestClient(String subscriptionKey) {
        this.restCall = new WebServiceRequest(subscriptionKey);
    }

    @Override
    public List<RecognizeResult> recognizeImage(String url) throws EmotionServiceException {
        return recognizeImage(url, null);
    }

    @Override
    public List<RecognizeResult> recognizeImage(String url, FaceRectangle[] faceRectangles) throws EmotionServiceException {
        Map<String, Object> params = new HashMap<>();
        String path = serviceHost + "/recognize";
        if (faceRectangles != null && faceRectangles.length > 0) {
            params.put(FACE_RECTANGLES, getFaceRectangleStrings(faceRectangles));
        }
        String uri = WebServiceRequest.getUrl(path, params);

        params.clear();
        params.put("url", url);

        String json = (String) this.restCall.post(uri, params, null, false);
        RecognizeResult[] recognizeResult = this.gson.fromJson(json, RecognizeResult[].class);

        return Arrays.asList(recognizeResult);
    }

    @Override
    public List<RecognizeResult> recognizeImage(InputStream stream) throws EmotionServiceException, IOException {
        return recognizeImage(stream, null);
    }

    @Override
    public List<RecognizeResult> recognizeImage(InputStream stream, FaceRectangle[] faceRectangles) throws EmotionServiceException, IOException {
        Map<String, Object> params = new HashMap<>();
        String path = serviceHost + "/recognize";
        if (faceRectangles != null && faceRectangles.length > 0) {
            params.put(FACE_RECTANGLES, getFaceRectangleStrings(faceRectangles));
        }

        String uri = WebServiceRequest.getUrl(path, params);

        params.clear();
        byte[] data = IOUtils.toByteArray(stream);
        params.put("data", data);

        String json = (String) this.restCall.post(uri, params, "application/octet-stream", false);
        RecognizeResult[] recognizeResult = this.gson.fromJson(json, RecognizeResult[].class);

        return Arrays.asList(recognizeResult);
    }

    private String getFaceRectangleStrings(FaceRectangle[] faceRectangles) {
        StringBuffer sb = new StringBuffer();

        boolean firstRectangle = true;
        for (FaceRectangle faceRectangle : faceRectangles) {
            if (firstRectangle) {
                firstRectangle = false;
            } else {
                sb.append(';');
            }
            sb.append(String.format("%d,%d,%d,%d", faceRectangle.left, faceRectangle.top, faceRectangle.width, faceRectangle.height));
        }
        return sb.toString();
    }
}
