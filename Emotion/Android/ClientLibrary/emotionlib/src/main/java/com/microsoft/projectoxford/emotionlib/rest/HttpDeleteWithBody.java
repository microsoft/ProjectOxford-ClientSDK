package com.microsoft.projectoxford.emotionlib.rest;

import org.apache.http.client.methods.HttpEntityEnclosingRequestBase;

import java.net.URI;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class HttpDeleteWithBody extends HttpEntityEnclosingRequestBase {
    private static final String METHOD_NAME = "DELETE";

    public String getMethod() {
        return "DELETE";
    }

    public HttpDeleteWithBody(String uri) {
        this(URI.create(uri));
    }

    public HttpDeleteWithBody(URI uri) {
        this.setURI(uri);
    }

    public HttpDeleteWithBody() {
    }
}
