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
package com.microsoft.projectoxford.emotion.rest;

import com.google.gson.Gson;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Map;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.entity.ByteArrayEntity;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;

public class WebServiceRequest {
    private static final String headerKey = "ocp-apim-subscription-key";
    private HttpClient client = new DefaultHttpClient();
    private String subscriptionKey;
    private Gson gson = new Gson();

    public WebServiceRequest(String key) {
        this.subscriptionKey = key;
    }

    public Object post(String url, Map<String, Object> data, String contentType, boolean responseInputStream) throws EmotionServiceException {
        return webInvoke("POST", url, data, contentType, responseInputStream);
    }

    private Object webInvoke(String method, String url, Map<String, Object> data, String contentType, boolean responseInputStream) throws EmotionServiceException {
        HttpPost request = null;

        request = new HttpPost(url);

        boolean isStream = false;

        /*Set header*/
        if (contentType != null && !contentType.isEmpty()) {
            request.setHeader("Content-Type", contentType);
            if (contentType.toLowerCase().contains("octet-stream")) {
                isStream = true;
            }
        } else {
            request.setHeader("Content-Type", "application/json");
        }

        request.setHeader(headerKey, this.subscriptionKey);

        try {
            if (!isStream) {
                String json = this.gson.toJson(data).toString();
                StringEntity entity = new StringEntity(json);
                request.setEntity(entity);
            } else {
                request.setEntity(new ByteArrayEntity((byte[]) data.get("data")));
            }

            HttpResponse response = this.client.execute(request);
            int statusCode = response.getStatusLine().getStatusCode();
            if (statusCode == 200) {
                if(!responseInputStream) {
                    return readInput(response.getEntity().getContent());
                }else {
                    return response.getEntity().getContent();
                }
            } else {
                throw new Exception("Error executing POST request! Received error code: " + response.getStatusLine().getStatusCode());
            }
        } catch (Exception e) {
            throw new EmotionServiceException(e.getMessage());
        }
    }

    public static String getUrl(String path, Map<String, Object> params) {
        StringBuffer url = new StringBuffer(path);

        boolean start = true;
        for (Map.Entry<String, Object> param : params.entrySet()) {
            if (start) {
                url.append("?");
                start = false;
            } else {
                url.append("&");
            }

            try {
                url.append(param.getKey() + "=" + URLEncoder.encode(param.getValue().toString(), "UTF-8"));
            } catch (UnsupportedEncodingException e) {
                e.printStackTrace();
            }
        }

        return url.toString();
    }

    private String readInput(InputStream is) throws IOException {
        BufferedReader br = new BufferedReader(new InputStreamReader(is));
        StringBuffer json = new StringBuffer();
        String line;
        while ((line = br.readLine()) != null) {
            json.append(line);
        }

        return json.toString();
    }
}
