package com.microsoft.projectoxford.emotionlib.rest;

import org.apache.http.client.methods.HttpPost;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class HttpPatch extends HttpPost {
    private static final String METHOD_PATCH = "PATCH";

    public HttpPatch(String url) {
        super(url);
    }

    public String getMethod() {
        return "PATCH";
    }
}
