package com.microsoft.projectoxford.emotionlib;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.microsoft.projectoxford.emotionlib.common.RequestMethod;
import com.microsoft.projectoxford.emotionlib.contract.EmotionFace;
import com.microsoft.projectoxford.emotionlib.rest.ClientException;
import com.microsoft.projectoxford.emotionlib.rest.WebServiceRequest;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Type;
import java.util.HashMap;
import java.util.List;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class EmotionServiceRestClient {

    private final WebServiceRequest mRestCall;
    private Gson mGson = new Gson();
    private String mServiceHost = "https://api.projectoxford.ai/emotion/v1.0";
    public EmotionServiceRestClient(String subscriptionKey) {
        this.mRestCall = new WebServiceRequest(subscriptionKey);
    }

    public EmotionServiceRestClient(String serviceHost, String subscriptionKey) {
        this.mServiceHost = serviceHost;
        this.mRestCall = new WebServiceRequest(subscriptionKey);
    }

    public EmotionFace[] detect(String url) throws ClientException, IOException {
        HashMap params = new HashMap();


        String var12 = String.format("%s/%s", new Object[]{this.mServiceHost, "recognize"});
        //String var13 = WebServiceRequest.getUrl(var12, params);
        params.clear();
        params.put("url", url);
        String var14 = (String)this.mRestCall.request(var12, RequestMethod.POST, params, (String) null);


        Type var15 = (new TypeToken() {
        }).getType();
        List var16 = (List)this.mGson.fromJson(var14, var15);
        return (EmotionFace[])var16.toArray(new EmotionFace[var16.size()]);

       // return var14;

    }

    public EmotionFace[] detect(InputStream imageStream) throws ClientException, IOException {
        HashMap params = new HashMap();

        int bytesRead;


        String var15 = String.format("%s/%s", new Object[]{this.mServiceHost, "recognize"});
        //String var16 = WebServiceRequest.getUrl(var15, params);
        ByteArrayOutputStream var17 = new ByteArrayOutputStream();
        byte[] var19 = new byte[1024];

        while((bytesRead = imageStream.read(var19)) > 0) {
            var17.write(var19, 0, bytesRead);
        }

        byte[] var18 = var17.toByteArray();
        params.clear();
        params.put("data", var18);
        String json = (String)this.mRestCall.request(var15, RequestMethod.POST, params, "application/octet-stream");
        //Log.d("JSON",json);
        EmotionFace[] emotionFaces =  this.mGson.fromJson(json, EmotionFace[].class);
        return emotionFaces;


    }
}
