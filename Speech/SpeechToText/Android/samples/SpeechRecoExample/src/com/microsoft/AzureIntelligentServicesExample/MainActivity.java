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
package com.microsoft.AzureIntelligentServicesExample;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

import com.microsoft.projectoxford.speechrecognition.Contract;
import com.microsoft.projectoxford.speechrecognition.DataRecognitionClient;
import com.microsoft.projectoxford.speechrecognition.DataRecognitionClientWithIntent;
import com.microsoft.projectoxford.speechrecognition.ISpeechRecognitionServerEvents;
import com.microsoft.projectoxford.speechrecognition.MicrophoneRecognitionClient;
import com.microsoft.projectoxford.speechrecognition.MicrophoneRecognitionClientWithIntent;
import com.microsoft.projectoxford.speechrecognition.RecognitionResult;
import com.microsoft.projectoxford.speechrecognition.RecognitionStatus;
import com.microsoft.projectoxford.speechrecognition.SpeechRecognitionMode;
import com.microsoft.projectoxford.speechrecognition.SpeechRecognitionServiceFactory;

import java.io.IOException;
import java.io.InputStream;
import java.util.concurrent.TimeUnit;

public class MainActivity extends Activity implements ISpeechRecognitionServerEvents
{
    int m_waitSeconds = 0;
    DataRecognitionClient m_dataClient = null;
    MicrophoneRecognitionClient m_micClient = null;
    boolean m_isMicrophoneReco;
    SpeechRecognitionMode m_recoMode;
    boolean m_isIntent;
    FinalResponseStatus isReceivedResponse = FinalResponseStatus.NotReceived;
    
    public enum FinalResponseStatus { NotReceived, OK, Timeout }
    		
    public void onPartialResponseReceived(final String response)
    {
        EditText myEditText = (EditText) findViewById(R.id.editText1);
        myEditText.append("********* Partial Result *********\n");
        myEditText.append(response + "\n");	
    }

    public void onFinalResponseReceived(final RecognitionResult response)
    {
        boolean isFinalDicationMessage = m_recoMode == SpeechRecognitionMode.LongDictation && 
                                                       (response.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                                                        response.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout);
        if (m_isMicrophoneReco && ((m_recoMode == SpeechRecognitionMode.ShortPhrase) || isFinalDicationMessage)) {
            // we got the final result, so it we can end the mic reco.  No need to do this
            // for dataReco, since we already called endAudio() on it as soon as we were done
            // sending all the data.
            m_micClient.endMicAndRecognition();
        }

        if ((m_recoMode == SpeechRecognitionMode.ShortPhrase) || isFinalDicationMessage) {
            Button startButton = (Button) findViewById(R.id.button1);
            startButton.setEnabled(true);
            this.isReceivedResponse = FinalResponseStatus.OK;
        }

        if (!isFinalDicationMessage) {
            EditText myEditText = (EditText) findViewById(R.id.editText1);
            myEditText.append("***** Final NBEST Results *****\n");
            for (int i = 0; i < response.Results.length; i++) {
                myEditText.append(i + " Confidence=" + response.Results[i].Confidence + 
                                  " Text=\"" + response.Results[i].DisplayText + "\"\n");
            }
    	}
    } 

    /**
    * Called when a final response is received and its intent is parsed 
    */
    public void onIntentReceived(final String payload)
    {
        EditText myEditText = (EditText) findViewById(R.id.editText1);
        myEditText.append("********* Final Intent *********\n");
        myEditText.append(payload + "\n");
    }
    
    public void onError(final int errorCode, final String response) 
    {
        Button startButton = (Button) findViewById(R.id.button1);
        startButton.setEnabled(true);
        
        EditText myEditText = (EditText) findViewById(R.id.editText1);
        myEditText.append("********* Error Detected *********\n");
        myEditText.append(errorCode + " " + response + "\n");
    }

    /**
     * Invoked when the audio recording state has changed.
     *
     * @param recording The current recording state
     */
    public void onAudioEvent(boolean recording)
    {
    	if (!recording) {
    		m_micClient.endMicAndRecognition();
            Button startButton = (Button) findViewById(R.id.button1);
            startButton.setEnabled(true);
    	}
    	
        EditText myEditText = (EditText) findViewById(R.id.editText1);
        myEditText.append("********* Microphone status: " + recording + " *********\n");
    }

    /**
	    * Speech recognition with data (for example from a file or audio source).  
	    * The data is broken up into buffers and each buffer is sent to the Speech Recognition Service.
	    * No modification is done to the buffers, so the user can apply their
	    * own VAD (Voice Activation Detection) or Silence Detection
	    * 
	    * @param dataClient
	    * @param recoMode
	    * @param filename
	*/	
    private class RecognitionTask extends AsyncTask<Void, Void, Void> {
    	DataRecognitionClient dataClient;
    	SpeechRecognitionMode recoMode;
    	String filename;
    	
    	RecognitionTask(DataRecognitionClient dataClient, SpeechRecognitionMode recoMode, String filename) {
    		this.dataClient = dataClient;
    		this.recoMode = recoMode;
    		this.filename = filename;
    	}
    	
    	@Override
    	protected Void doInBackground(Void... params) {
            try {           
                // Note for wave files, we can just send data from the file right to the server.
                // In the case you are not an audio file in wave format, and instead you have just
                // raw data (for example audio coming over bluetooth), then before sending up any 
                // audio data, you must first send up an SpeechAudioFormat descriptor to describe 
                // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.
                // String filename = recoMode == SpeechRecognitionMode.ShortPhrase ? "whatstheweatherlike.wav" : "batman.wav";
                InputStream fileStream = getAssets().open(filename);
                int bytesRead = 0;
                byte[] buffer = new byte[1024];
                  
                do {
                    // Get  Audio data to send into byte buffer.
                    bytesRead = fileStream.read(buffer);

                    if (bytesRead > -1) {
                        // Send of audio data to service. 
                        dataClient.sendAudio(buffer, bytesRead);
                    }
                } while (bytesRead > 0);
            }
            catch(IOException ex) {
                Contract.fail();
            }
            finally {            
                dataClient.endAudio();
            }
            
            return null;
    	}
    }
    
    void initializeRecoClient()
    {
        String language = "en-us";
        
        String subscriptionKey = this.getString(R.string.subscription_key);
        String luisAppID = this.getString(R.string.luisAppID);
        String luisSubscriptionID = this.getString(R.string.luisSubscriptionID);

        if (m_isMicrophoneReco && null == m_micClient) {
            if (!m_isIntent) {
                m_micClient = SpeechRecognitionServiceFactory.createMicrophoneClient(this,
                                                                                     m_recoMode, 
                                                                                     language,
                                                                                     this,
                                                                                     subscriptionKey);
            }
            else {
                MicrophoneRecognitionClientWithIntent intentMicClient;
                intentMicClient = SpeechRecognitionServiceFactory.createMicrophoneClientWithIntent(this,
                                                                                                   language,
                                                                                                   this,
                                                                                                   subscriptionKey,
                                                                                                   luisAppID,
                                                                                                   luisSubscriptionID);
                m_micClient = intentMicClient;

            }
        }
        else if (!m_isMicrophoneReco && null == m_dataClient) {
            if (!m_isIntent) {
                m_dataClient = SpeechRecognitionServiceFactory.createDataClient(this,
                                                                                m_recoMode, 
                                                                                language,
                                                                                this,
                                                                                subscriptionKey);
            }
            else {
                DataRecognitionClientWithIntent intentDataClient;
                intentDataClient = SpeechRecognitionServiceFactory.createDataClientWithIntent(this, 
                                                                                              language,
                                                                                              this,
                                                                                              subscriptionKey,
                                                                                              luisAppID,
                                                                                              luisSubscriptionID);
                m_dataClient = intentDataClient;
            }
        }
    }

    void addListenerOnButton() 
    {
        final Button startButton = (Button) findViewById(R.id.button1); 
        startButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View arg0) 
            {             
                EditText myEditText = (EditText) findViewById(R.id.editText1);
                myEditText.setText("");
                startButton.setEnabled(false);

                if (m_isMicrophoneReco) {
                    // Speech recognition from the microphone.  The microphone is turned on and data from the microphone
                    // is sent to the Speech Recognition Service.  A built in Silence Detector
                    // is applied to the microphone data before it is sent to the recognition service.
                    m_micClient.startMicAndRecognition();	
                }
                else {
                	String filename = m_recoMode == SpeechRecognitionMode.ShortPhrase ? "whatstheweatherlike.wav" : "batman.wav";
                	RecognitionTask doDataReco = new RecognitionTask(m_dataClient, m_recoMode, filename);
                	try
                	{
        	        	doDataReco.execute().get(m_waitSeconds, TimeUnit.SECONDS);
                	}
                	catch (Exception e)
                	{
                		doDataReco.cancel(true);
                		isReceivedResponse = FinalResponseStatus.Timeout;
                	}
                } 
            }
        });

        final Context appContext = this;
        Button finalResponseButton = (Button) findViewById(R.id.button2);
		
        finalResponseButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View arg0) 
            {
                AlertDialog alertDialog;
                alertDialog = new AlertDialog.Builder(appContext).create();
                alertDialog.setTitle("Final Response");
                EditText myEditText = (EditText) findViewById(R.id.editText1);

                if (m_micClient != null) {
                	while(isReceivedResponse == FinalResponseStatus.NotReceived) {}
                    m_micClient.endMicAndRecognition();
                    String msg = isReceivedResponse == FinalResponseStatus.OK ? "See TextBox below for response.  App Done" : "Timed out.  App Done";
                    alertDialog.setMessage(msg);
                    startButton.setEnabled(false);
                    try {
                        m_micClient.finalize();
                    } catch (Throwable e) {
                        myEditText.append(e + "\n");	
                    }
                }
                else if (m_dataClient != null) {
                    String msg = isReceivedResponse == FinalResponseStatus.OK ? "See TextBox below for response.  App Done" : "Timed out.  App Done";
                    alertDialog.setMessage(msg);
                    startButton.setEnabled(false);
                    try {
                        m_dataClient.finalize();
                    } catch (Throwable e) {
                        myEditText.append(e + "\n");	
                    }
                }
                else {
                    alertDialog.setMessage("Press Start first please!");
                }
                alertDialog.show();
            }
        });
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        if (getString(R.string.subscription_key).startsWith("Please")) {
            new AlertDialog.Builder(this)
                    .setTitle(getString(R.string.add_subscription_key_tip_title))
                    .setMessage(getString(R.string.add_subscription_key_tip))
                    .setCancelable(false)
                    .show();
        }
        
        // Set the mode and microphone flag to your liking

        // m_recoMode can be SpeechRecognitionMode.ShortPhrase or SpeechRecognitionMode.LongDictation
        m_recoMode = SpeechRecognitionMode.ShortPhrase;
        m_isMicrophoneReco = true;
        m_isIntent = false;

        m_waitSeconds = m_recoMode == SpeechRecognitionMode.ShortPhrase ? 20 : 200;
        
        initializeRecoClient();
        
        // setup the buttons
        addListenerOnButton();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }
}
