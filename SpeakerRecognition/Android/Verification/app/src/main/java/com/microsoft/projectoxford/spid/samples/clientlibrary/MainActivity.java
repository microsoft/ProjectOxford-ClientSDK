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

import com.microsoft.projectoxford.spid.samples.clientlibrary.response.EnrollmentResponse;
import com.microsoft.projectoxford.spid.samples.clientlibrary.response.ProfileResponse;
import com.microsoft.projectoxford.spid.samples.clientlibrary.response.VerificationResponse;
import com.microsoft.projectoxford.spid.samples.clientlibrary.simplesound.pcm.PcmAudioHelper;
import com.microsoft.projectoxford.spid.samples.clientlibrary.simplesound.pcm.WavAudioFormat;

import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.os.Bundle;
import android.os.Environment;
import android.support.v7.app.ActionBarActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.HttpURLConnection;
import java.nio.ByteOrder;

/**
 * Main activity - The entry point
 */
public class MainActivity extends ActionBarActivity {

    private ProgressBar mProgressBar;
    private TextView mStatus;
    private VerificationHttpHelper mHttpHelper;
    private String mVerificationSpeakerId;
    private TextView mPhraseText;
    /**
     * Enrollment and verification related fields
     */
    private boolean mIsEnrolling = false;
    private boolean mIsVerifying = false;
    private Thread mRecordingThread = null;
    private boolean mIsRecording = false;
    /**
     * Audio format related fields
     */
    private AudioRecord mRecorder = null;
    private static final int RECORDER_SAMPLERATE = 16000;
    private static final int RECORDER_CHANNELS = AudioFormat.CHANNEL_IN_MONO;
    private static final int RECORDER_AUDIO_ENCODING = AudioFormat.ENCODING_PCM_16BIT;
    private int mBufferElements2Rec = 1024;
    private int mBytesPerElement = 2; // 2 bytes in 16bit format
    private static final String TMP_FILE_NAME = "voice16K16bitmono.raw";
    private static final String WAV_FILE_NAME = "voice16K16bitmono.wav";
    private static boolean sIsLittleEndian;

    /**
     * On create method of the activity
     *
     * @param savedInstanceState A bundle object encapsulating the saved state from previous runs
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        //Initialize the progress bar and status label
        this.mProgressBar = (ProgressBar) findViewById(R.id.progressBar);
        this.mStatus = (TextView) findViewById(R.id.statusText);
        this.mPhraseText = (TextView) findViewById(R.id.phraseText);
        //creates a new instance of the helper
        String subscriptionKey = ConfigurationManager.getSubscriptionKey(this);
        this.mHttpHelper = VerificationHttpHelper.getInstance(subscriptionKey);
        //Initialize the speakerId
        initializeSpeakerId();
        //Getting the endianness of the underlying system
        ByteOrder order = ByteOrder.nativeOrder();
        if(order.equals(ByteOrder.BIG_ENDIAN)){
            sIsLittleEndian = false;
        }else{
            sIsLittleEndian = true;
        }
    }

    /**
     * Checks if there's a stored speaker Id, if not it creates a new profile
     */
    private void initializeSpeakerId() {
        String speakerId = ConfigurationManager.getSpeakerId(getBaseContext());
        if (speakerId == null) {
            showProgress();
            setStatus("Creating Profile....");
            mHttpHelper.createProfile("en-us", new ResponseCallback() {
                @Override
                public void callback(int statusCode, Object model, String msg) {
                    if (statusCode == HttpURLConnection.HTTP_OK) {
                        ProfileResponse response = (ProfileResponse) model;
                        mVerificationSpeakerId = response.verificationProfileId;
                        hideProgress();
                        setStatus("New Profile Created");
                        ConfigurationManager.setSpeakerId(mVerificationSpeakerId, getBaseContext());
                    } else {
                        setStatus(msg);
                    }
                }
            });
        } else
            mVerificationSpeakerId = speakerId;
    }

    /**
     * Initialize the main menu of the app
     *
     * @param menu The menu object passed by the launcher
     * @return A boolean whether the current method should be the last observer or not
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        return super.onOptionsItemSelected(item);
    }

    private void showProgress() {
        this.mProgressBar.setVisibility(View.VISIBLE);
    }

    private void hideProgress() {
        this.mProgressBar.setVisibility(View.GONE);
    }

    private void setStatus(String msg) {
        this.mStatus.setText(msg);
    }

    private void toast(String msg) {
        Toast.makeText(getBaseContext(), msg, Toast.LENGTH_LONG).show();
    }

    private void startRecording() {
        mRecorder = new AudioRecord(MediaRecorder.AudioSource.MIC,
                RECORDER_SAMPLERATE, RECORDER_CHANNELS,
                RECORDER_AUDIO_ENCODING, mBufferElements2Rec * mBytesPerElement);

        mRecorder.startRecording();
        mIsRecording = true;
        mRecordingThread = new Thread(new Runnable() {

            public void run() {
                writeAudioDataToFile();
            }
        }, "AudioRecorder Thread");
        mRecordingThread.start();
    }

    private String getTempFilename() {
        return Environment.getExternalStorageDirectory().getAbsolutePath() + File.separator + TMP_FILE_NAME;
    }

    private String getFileName() {
        return Environment.getExternalStorageDirectory().getAbsolutePath() + File.separator + WAV_FILE_NAME;
    }

    private void writeAudioDataToFile() {
        short sData[] = new short[mBufferElements2Rec];
        FileOutputStream os = null;
        try {
            os = new FileOutputStream(getTempFilename());
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        }
        while (mIsRecording) {
            // gets the voice output from microphone to byte format
            mRecorder.read(sData, 0, mBufferElements2Rec);
            try {
                // // writes the data to file from buffer
                // // stores the voice buffer
                byte bData[] = short2byte(sData, sIsLittleEndian);
                os.write(bData, 0, mBufferElements2Rec * mBytesPerElement);
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        try {
            os.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private byte[] short2byte(short[] sData, boolean isLittleEndian) {
        int shortArrsize = sData.length;
        byte[] bytes = new byte[shortArrsize * 2];
        for (int i = 0; i < shortArrsize; i++) {
            if (isLittleEndian) {
                bytes[i * 2] = (byte) (sData[i] & 0x00FF);
                bytes[(i * 2) + 1] = (byte) (sData[i] >> 8);
                sData[i] = 0;
            } else {
                bytes[(i * 2)] = (byte) (sData[i] >> 8);
                bytes[(i * 2) + 1] = (byte) (sData[i] & 0x00FF);
            }
        }
        return bytes;
    }

    private void stopRecording() {
        mIsRecording = false;
        mRecordingThread = null;
        mRecorder.stop();
        mRecorder.release();
        mRecorder = null;
        File inFile = new File(getTempFilename());
        File outFile = new File(getFileName());
        try {
            PcmAudioHelper.convertRawToWav(WavAudioFormat.mono16Bit(16000), inFile, outFile);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    /**
     * Click listener for enroll button
     *
     * @param view The event trigger
     */
    public void enroll(View view) {
        if (mIsEnrolling) {
            setStatus("Recording done!");
            stopRecording();
            setStatus("Enrolling..");
            showProgress();
            mHttpHelper.enrollSpeaker(mVerificationSpeakerId, new File(getFileName()), new ResponseCallback() {
                @Override
                public void callback(int statusCode, Object model, String msg) {
                    if (statusCode == HttpURLConnection.HTTP_OK) {
                        EnrollmentResponse response = (EnrollmentResponse) model;
                        hideProgress();
                        setStatus("Remaining Enrollments: " + response.remainingEnrollments);
                        setPhraseText(response.phrase);
                    } else {
                        setStatus(msg);
                    }
                }
            });
        } else {
            setStatus("Recording..");
            startRecording();
        }
        mIsEnrolling = !mIsEnrolling;
    }

    /**
     * Click listener for the verify button
     *
     * @param view The view that triggered the event
     */
    public void verify(View view) {
        if (mIsVerifying) {
            setStatus("Recording done!");
            stopRecording();
            setStatus("Verifying..");
            showProgress();
            mHttpHelper.verifySpeaker(mVerificationSpeakerId, new File(getFileName()), new ResponseCallback() {
                @Override
                public void callback(int statusCode, Object model, String msg) {
                    if (statusCode == HttpURLConnection.HTTP_OK) {
                        VerificationResponse response = (VerificationResponse) model;
                        hideProgress();
                        setStatus("Verification Result: " + response.result + " with Confidence: " + response.confidence);
                    } else {
                        setStatus(msg);
                    }
                }
            });
        } else {
            setStatus("Recording..");
            startRecording();
        }
        mIsVerifying = !mIsVerifying;

    }

    @Override
    public void onPause() {
        super.onPause();
        if (mRecorder != null) {
            mRecorder.release();
            mRecorder = null;
        }
    }

    private void setPhraseText(String s) {
        mPhraseText.setText("Phrase: " + s);
    }
}
