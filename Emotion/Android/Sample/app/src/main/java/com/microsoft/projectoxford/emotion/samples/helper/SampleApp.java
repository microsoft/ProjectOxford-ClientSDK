
package com.microsoft.projectoxford.emotion.samples.helper;

import android.app.Application;

import com.microsoft.emotion.R;
import com.microsoft.projectoxford.emotionlib.EmotionServiceRestClient;


public class SampleApp extends Application {
    private static EmotionServiceRestClient sFaceServiceClient;

    @Override
    public void onCreate() {
        super.onCreate();
        sFaceServiceClient = new EmotionServiceRestClient(getString(R.string.subscription_key));
    }

    public static EmotionServiceRestClient getKey() {
        return sFaceServiceClient;
    }

}
