package com.microsoft.projectoxford.emotionlib.rest;


import android.util.Log;

import com.google.gson.Gson;
import com.microsoft.projectoxford.emotionlib.common.RequestMethod;
import com.microsoft.projectoxford.emotionlib.common.ServiceError;

import org.apache.http.HttpResponse;
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
import java.util.Iterator;
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
        Log.d("REQUEST", String.valueOf(method.ordinal()));
        switch(method.ordinal()) {
            case 1:
                return this.get(url);
            case 6:
            case 7:
            case 8:
            default:
                return null;
            case 2:
                return this.post(url, data, contentType);
            case 4:
                return this.patch(url, data, contentType);
            case 5:
                return this.delete(url, data);
            case 3:
                return this.put(url, data);
        }
        //return this.post(url, data, contentType);
    }

    private Object get(String url) throws ClientException, IOException {
        HttpGet request = new HttpGet(url);
        request.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
        HttpResponse response = this.mClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        if(statusCode == 200) {
            return this.readInput(response.getEntity().getContent());
        } else {
            String json = this.readInput(response.getEntity().getContent());
            if(json != null) {
                ServiceError error = (ServiceError)this.mGson.fromJson(json, ServiceError.class);
                if(error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing GET request!", statusCode);
        }
    }

    private Object patch(String url, Map<String, Object> data, String contentType) throws ClientException, IOException {
        HttpPatch request = new HttpPatch(url);
        request.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
        String json = this.mGson.toJson(data).toString();
        StringEntity entity = new StringEntity(json);
        request.setEntity(entity);
        request.setHeader("Content-Type", "application/json");
        HttpResponse response = this.mClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        if(statusCode == 200) {
            return this.readInput(response.getEntity().getContent());
        } else {
            json = this.readInput(response.getEntity().getContent());
            if(json != null) {
                ServiceError error = (ServiceError)this.mGson.fromJson(json, ServiceError.class);
                if(error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing Patch request!", statusCode);
        }
    }

    private Object post(String url, Map<String, Object> data, String contentType) throws ClientException, IOException {
        HttpPost request = new HttpPost(url);
        boolean isStream = false;
        if(contentType != null && contentType.length() != 0) {
            request.setHeader("Content-Type", contentType);
            if(contentType.toLowerCase().contains("octet-stream")) {
                isStream = true;
            }
        } else {
            request.setHeader("Content-Type", "application/json");
        }

        request.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
        if(!isStream) {
            String response = this.mGson.toJson(data).toString();
            StringEntity statusCode = new StringEntity(response);
            request.setEntity(statusCode);
        } else {
            request.setEntity(new ByteArrayEntity((byte[])((byte[])data.get("data"))));
        }

        HttpResponse response1 = this.mClient.execute(request);
        int statusCode1 = response1.getStatusLine().getStatusCode();
        if(statusCode1 != 200 && statusCode1 != 202) {
            String json = this.readInput(response1.getEntity().getContent());
            if(json != null) {
                ServiceError error = (ServiceError)this.mGson.fromJson(json, ServiceError.class);
                if(error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing POST request!", statusCode1);
        } else {
            return this.readInput(response1.getEntity().getContent());
        }
    }

    private Object put(String url, Map<String, Object> data) throws ClientException, IOException {
        HttpPut request = new HttpPut(url);
        request.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
        String json = this.mGson.toJson(data).toString();
        StringEntity entity = new StringEntity(json);
        request.setEntity(entity);
        request.setHeader("Content-Type", "application/json");
        HttpResponse response = this.mClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        if(statusCode == 200) {
            return this.readInput(response.getEntity().getContent());
        } else {
            json = this.readInput(response.getEntity().getContent());
            if(json != null) {
                ServiceError error = (ServiceError)this.mGson.fromJson(json, ServiceError.class);
                if(error != null) {
                    throw new ClientException(error.error);
                }
            }

            throw new ClientException("Error executing PUT request!", statusCode);
        }
    }

    private Object delete(String url, Map<String, Object> data) throws ClientException, IOException {
        HttpResponse response;
        String json;
        if(data != null && !data.isEmpty()) {
            HttpDeleteWithBody statusCode1 = new HttpDeleteWithBody(url);
            json = this.mGson.toJson(data).toString();
            StringEntity error = new StringEntity(json);
            statusCode1.setEntity(error);
            statusCode1.setHeader("Content-Type", "application/json");
            statusCode1.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
            response = this.mClient.execute(statusCode1);
        } else {
            HttpDelete statusCode = new HttpDelete(url);
            statusCode.setHeader("ocp-apim-subscription-key", this.mSubscriptionKey);
            response = this.mClient.execute(statusCode);
        }

        int statusCode2 = response.getStatusLine().getStatusCode();
        if(statusCode2 != 200) {
            json = this.readInput(response.getEntity().getContent());
            if(json != null) {
                ServiceError error1 = (ServiceError)this.mGson.fromJson(json, ServiceError.class);
                if(error1 != null) {
                    throw new ClientException(error1.error);
                }
            }

            throw new ClientException("Error executing DELETE request!", statusCode2);
        } else {
            return this.readInput(response.getEntity().getContent());
        }
    }

    public static String getUrl(String path, Map<String, Object> params) {
        StringBuffer url = new StringBuffer(path);
        boolean start = true;
        Iterator var4 = params.entrySet().iterator();

        while(var4.hasNext()) {
            Map.Entry param = (Map.Entry)var4.next();
            if(start) {
                url.append("?");
                start = false;
            } else {
                url.append("&");
            }

            try {
                url.append((String)param.getKey());
                url.append("=");
                url.append(URLEncoder.encode(param.getValue().toString(), "UTF-8"));
            } catch (UnsupportedEncodingException var7) {
                var7.printStackTrace();
            }
        }

        return url.toString();
    }

    private String readInput(InputStream is) throws IOException {
        BufferedReader br = new BufferedReader(new InputStreamReader(is));
        StringBuffer json = new StringBuffer();

        String line;
        while((line = br.readLine()) != null) {
            json.append(line);
        }

        return json.toString();
    }
}
