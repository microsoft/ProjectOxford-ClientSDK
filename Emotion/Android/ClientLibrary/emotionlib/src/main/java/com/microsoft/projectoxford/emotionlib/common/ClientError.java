package com.microsoft.projectoxford.emotionlib.common;

import java.util.UUID;

/**
 * Created by rcrespoy on 1/6/2016.
 */
public class ClientError {
    public String code;
    public String message;
    public UUID requestId;

    public ClientError() {
    }
}
