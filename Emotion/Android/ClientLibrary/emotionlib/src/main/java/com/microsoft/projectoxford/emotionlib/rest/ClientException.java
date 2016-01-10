package com.microsoft.projectoxford.emotionlib.rest;

import com.microsoft.projectoxford.emotionlib.common.ClientError;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class ClientException extends Exception {
    public ClientError error = new ClientError();

    public ClientException(ClientError clientError) {
        super(clientError.message);
        this.error.code = clientError.code;
        this.error.message = clientError.message;
    }

    public ClientException(String message, int statusCode) {
        super(message);
        Integer code = Integer.valueOf(statusCode);
        this.error.code = code.toString();
        this.error.message = message;
    }

    public ClientException(String message) {
        super(message);
    }
}
