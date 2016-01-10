package com.microsoft.projectoxford.emotionlib.rest;

import com.google.gson.Gson;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class ServiceException extends Exception {
    public ServiceException(String message) {
        super(message);
    }

    public ServiceException(Gson errorObject) {
        super(errorObject.toString());
    }
}
