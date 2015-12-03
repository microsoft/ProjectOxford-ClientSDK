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
package com.microsoft.projectoxford.spid.samples.clientlibrary;

import android.os.AsyncTask;
import android.util.Log;

import com.google.gson.Gson;

import com.microsoft.projectoxford.spid.samples.clientlibrary.response.EnrollmentResponse;
import com.microsoft.projectoxford.spid.samples.clientlibrary.response.ProfileResponse;
import com.microsoft.projectoxford.spid.samples.clientlibrary.response.VerificationResponse;

import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.List;

/**
 * A class that's used as a helper for verification http calls
 */
public class VerificationHttpHelper {

    /**
     * Verification endpoints for project oxford SPID service.
     */
    private static final String BASE_URI = "https://api.projectoxford.ai/spid/v1.0/verificationProfiles";
    private static final String VERIFY_ENDPOINT = "https://api.projectoxford.ai/spid/v1.0/verify";

    private String mSubscriptionKey;

    private static VerificationHttpHelper sInstance;
    private static final String OCP_SUBSCRIPTION_KEY_HEADER = "Ocp-Apim-Subscription-Key";
    private static final String AUDIO_PARAM = "audio";
    private static final String LOCALE_PARAM = "locale";

    private VerificationHttpHelper() {
        mSubscriptionKey = ConfigurationManager.getSubscriptionKey();
    }

    /**
     * Creates an instance of the verification helper
     *
     * @return A VerificationHttpHelper object
     */
    public static VerificationHttpHelper getInstance() {
        if (sInstance == null)
            sInstance = new VerificationHttpHelper();
        return sInstance;
    }

    /**
     * Creates a user profile
     *
     * @param locale      The user locale
     * @param resCallback The response callback
     */
    public void createProfile(String locale, ResponseCallback resCallback) {
        HttpPost request = new HttpPost(BASE_URI);
        request.addHeader(OCP_SUBSCRIPTION_KEY_HEADER, mSubscriptionKey);
        List<NameValuePair> paramsList = new ArrayList<>(2);
        paramsList.add(new BasicNameValuePair(LOCALE_PARAM, locale));
        try {
            request.setEntity(new UrlEncodedFormEntity(paramsList));
            new HttpAsyncTask(resCallback, ProfileResponse.class).execute(request);
        } catch (UnsupportedEncodingException e) {
            Log.e("ErrorOnProfileCreation", e.getLocalizedMessage());
            resCallback.callback(Constants.CALL_CREATION_FAIL, null, e.getLocalizedMessage());
        }
    }

    /**
     * Enrolls a given speaker
     *
     * @param speakerId   The speaker Id
     * @param audioFile   The audioFile
     * @param resCallback The response callback
     */
    public void enrollSpeaker(String speakerId, File audioFile, ResponseCallback resCallback) {
        String Url = null;
        try {
            Url = BASE_URI + "/" + URLEncoder.encode(speakerId, "UTF-8") + "/enroll";
            HttpPost request = new HttpPost(Url);
            request.addHeader(OCP_SUBSCRIPTION_KEY_HEADER, mSubscriptionKey);
            MultipartEntity entity = new MultipartEntity(HttpMultipartMode.STRICT);
            entity.addPart(AUDIO_PARAM, new FileBody(audioFile));
            request.setEntity(entity);
            new HttpAsyncTask(resCallback, EnrollmentResponse.class).execute(request);
        } catch (UnsupportedEncodingException e) {
            Log.e("Verify Speaker Error", e.getLocalizedMessage());
        }
    }

    /**
     * Verifies a given speaker against a given audio
     *
     * @param speakerId   The speaker Id
     * @param audioFile   The audiofile to match against
     * @param resCallback The response callback
     */
    public void verifySpeaker(String speakerId, File audioFile, ResponseCallback resCallback) {
        String Url = null;
        try {
            Url = VERIFY_ENDPOINT + "?verificationProfileId=" + URLEncoder.encode(speakerId, "UTF-8");
            HttpPost request = new HttpPost(Url);
            request.addHeader(OCP_SUBSCRIPTION_KEY_HEADER, mSubscriptionKey);
            MultipartEntity entity = new MultipartEntity(HttpMultipartMode.STRICT);
            entity.addPart("audio", new FileBody(audioFile));
            request.setEntity(entity);
            new HttpAsyncTask(resCallback, VerificationResponse.class).execute(request);
        } catch (UnsupportedEncodingException e) {
            Log.e("Verify Speaker Error", e.getLocalizedMessage());
        }
    }

    /**
     * A helper class that's used to run AsyncTasks for HTTPCalls
     */
    private class HttpAsyncTask extends AsyncTask<HttpUriRequest, String, HttpResponse> {
        private ResponseCallback resCallback;
        private Class resClass;

        /**
         * Creates an instance of the helper class
         *
         * @param resCallback A callback that's called once the request is done
         * @param resClass    The type of response object to be parsed
         */
        public HttpAsyncTask(ResponseCallback resCallback, Class resClass) {
            this.resCallback = resCallback;
            this.resClass = resClass;
        }

        @Override
        protected HttpResponse doInBackground(HttpUriRequest... params) {
            HttpClient client = new DefaultHttpClient();
            HttpUriRequest request = params[0];
            try {
                return client.execute(request);
            } catch (IOException e) {
                Log.e("HttpAsyncTask_Exception", e.getLocalizedMessage());
                return null;
            }
        }

        @Override
        protected void onPostExecute(HttpResponse httpResponse) {
            if (httpResponse == null)
                resCallback.callback(Constants.CALL_FAIL, null, "");
            else {
                int HttpCode = httpResponse.getStatusLine().getStatusCode();
                if (HttpCode == HttpURLConnection.HTTP_OK) {
                    try {
                        InputStream resStream = httpResponse.getEntity().getContent();
                        BufferedReader reader = new BufferedReader(new InputStreamReader(resStream));
                        String str;
                        StringBuilder stringResponse = new StringBuilder();
                        while ((str = reader.readLine()) != null)
                            stringResponse.append(str);
                        Object responseModel = new Gson().fromJson(stringResponse.toString(), this.resClass);
                        resCallback.callback(HttpURLConnection.HTTP_OK, responseModel, httpResponse.getStatusLine().getReasonPhrase());
                    } catch (IOException e) {
                        resCallback.callback(Constants.CALL_FAIL, null, e.getLocalizedMessage());
                    }
                } else {
                    resCallback.callback(HttpCode, null, httpResponse.getStatusLine().getReasonPhrase());
                }
            }
        }
    }
}
