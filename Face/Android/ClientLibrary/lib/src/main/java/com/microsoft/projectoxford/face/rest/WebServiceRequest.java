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
package com.microsoft.projectoxford.face.rest;

import com.google.gson.Gson;
import com.microsoft.projectoxford.face.common.RequestMethod;
import com.microsoft.projectoxford.face.common.ServiceError;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.entity.ByteArrayEntity;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Map;

public class WebServiceRequest {
    private static final String HEADER_KEY = "ocp-apim-subscription-key";
    private static final String CONTENT_TYPE = "Content-Type";
    private static final String APPLICATION_JSON = "application/json";
    private static final String OCTET_STREAM = "octet-stream";
    private static final String DATA = "data";

    private HttpClient mClient = new DefaultHttpClient();
    private String mSubscriptionKey;
    private Gson mGson = new Gson();

    public WebServiceRequest(String key) {
        this.mSubscriptionKey = key;
    }

    public Object request(String url, RequestMethod method, Map<String, Object> data, String contentType) throws ClientException, IOException {
        switch (method) {
            case GET:
                return get(url);
            case HEAD:
                break;
            case POST:
                return post(url, data, contentType);
            case PATCH:
                return patch(url, data, contentType);
            case DELETE:
                return delete(url, data);
            case PUT:
                return put(url, data);
            case OPTIONS:
                break;
            case TRACE:
                break;
        }

        return null;
    }

    private Object get(String url) throws ClientException, IOException {
        HttpGet request = new HttpGet(url);
        request.setHeader(HEADER_KEY, mSubscriptionKey);

        HttpResponse response = this.mClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode == HttpStatus.SC_OK) {
            return readInput(response.getEntity().getContent());
        } else {
            String json = readInput(response.getEntity().getContent());
            if (json != null) {
                ServiceError error = mGson.fromJson(json, ServiceError.class);
                if (error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing GET request!", statusCode);
        }
    }


    private Object patch(String url, Map<String, Object> data, String contentType) throws ClientException, IOException {
        HttpPatch request = new HttpPatch(url);
        request.setHeader(HEADER_KEY, mSubscriptionKey);
        String json = mGson.toJson(data).toString();
        StringEntity entity = new StringEntity(json);
        request.setEntity(entity);
        request.setHeader(CONTENT_TYPE, APPLICATION_JSON);
        HttpResponse response = mClient.execute(request);

        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode == HttpStatus.SC_OK) {
            return readInput(response.getEntity().getContent());
        } else {
            json = readInput(response.getEntity().getContent());
            if (json != null) {
                ServiceError error = mGson.fromJson(json, ServiceError.class);
                if (error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing Patch request!", statusCode);
        }
    }


    private Object post(String url, Map<String, Object> data, String contentType) throws ClientException, IOException {
        HttpPost request = new HttpPost(url);
        boolean isStream = false;

        if (contentType != null && !(contentType.length() == 0)) {
            request.setHeader(CONTENT_TYPE, contentType);
            if (contentType.toLowerCase().contains(OCTET_STREAM)) {
                isStream = true;
            }
        } else {
            request.setHeader(CONTENT_TYPE, APPLICATION_JSON);
        }

        request.setHeader(HEADER_KEY, this.mSubscriptionKey);

        if (!isStream) {
            String json = mGson.toJson(data).toString();
            StringEntity entity = new StringEntity(json);
            request.setEntity(entity);
        } else {
            request.setEntity(new ByteArrayEntity((byte[]) data.get(DATA)));
        }

        HttpResponse response = mClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode == HttpStatus.SC_OK || statusCode == HttpStatus.SC_ACCEPTED) {
            return readInput(response.getEntity().getContent());
        } else {
            String json = readInput(response.getEntity().getContent());
            if (json != null) {
                ServiceError error = mGson.fromJson(json, ServiceError.class);
                if (error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing POST request!", statusCode);
        }
    }

    private Object put(String url, Map<String, Object> data) throws ClientException, IOException {
        HttpPut request = new HttpPut(url);

        request.setHeader(HEADER_KEY, mSubscriptionKey);
        String json = mGson.toJson(data).toString();
        StringEntity entity = new StringEntity(json);
        request.setEntity(entity);
        request.setHeader(CONTENT_TYPE, APPLICATION_JSON);
        HttpResponse response = mClient.execute(request);

        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode == HttpStatus.SC_OK) {
            return readInput(response.getEntity().getContent());
        } else {
            json = readInput(response.getEntity().getContent());
            if (json != null) {
                ServiceError error = mGson.fromJson(json, ServiceError.class);
                if (error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing PUT request!", statusCode);
        }
    }


    private Object delete(String url, Map<String, Object> data) throws ClientException, IOException {
        HttpResponse response;
        if (data == null || data.isEmpty()) {
            HttpDelete request = new HttpDelete(url);
            request.setHeader(HEADER_KEY, mSubscriptionKey);
            response = mClient.execute(request);
        } else {
            HttpDeleteWithBody request = new HttpDeleteWithBody(url);
            String json = mGson.toJson(data).toString();
            StringEntity entity = new StringEntity(json);
            request.setEntity(entity);
            request.setHeader(CONTENT_TYPE, APPLICATION_JSON);
            request.setHeader(HEADER_KEY, mSubscriptionKey);
            response = mClient.execute(request);
        }

        int statusCode = response.getStatusLine().getStatusCode();
        if (statusCode != HttpStatus.SC_OK) {
            String json = readInput(response.getEntity().getContent());
            if (json != null) {
                ServiceError error = mGson.fromJson(json, ServiceError.class);
                if (error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing DELETE request!", statusCode);
        }

        return readInput(response.getEntity().getContent());
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
                url.append(param.getKey());
                url.append("=");
                url.append(URLEncoder.encode(param.getValue().toString(), "UTF-8"));
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
