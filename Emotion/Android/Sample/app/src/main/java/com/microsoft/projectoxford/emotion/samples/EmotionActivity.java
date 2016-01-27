package com.microsoft.projectoxford.emotion.samples;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.microsoft.emotion.R;
import com.microsoft.projectoxford.emotionlib.EmotionServiceRestClient;
import com.microsoft.projectoxford.emotionlib.contract.EmotionFace;
import com.microsoft.projectoxford.emotion.samples.helper.FaceAdapter;
import com.microsoft.projectoxford.emotion.samples.helper.ImageHelper;
import com.microsoft.projectoxford.emotion.samples.helper.LogHelper;
import com.microsoft.projectoxford.emotion.samples.helper.SampleApp;
import com.microsoft.projectoxford.emotion.samples.helper.SelectImageActivity;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;

public class EmotionActivity extends AppCompatActivity {
    private class DetectionTask extends AsyncTask<InputStream,String,EmotionFace[]> {
        private boolean mSucceed = true;
        @Override
        protected EmotionFace[] doInBackground(InputStream... params) {
            // Get an instance of face service client to detect faces in image.
            EmotionServiceRestClient faceServiceClient = SampleApp.getKey();
            try {
                publishProgress("Detecting...");

                // Start detection.
                return faceServiceClient.detect(params[0]);
            } catch (Exception e) {
                mSucceed = false;
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }
        @Override
        protected void onPreExecute() {
            mProgressDialog.show();
            addLog("Request: Detecting in image " + mImageUri);
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            mProgressDialog.setMessage(progress[0]);
            setInfo(progress[0]);
        }

        @Override
        protected void onPostExecute(EmotionFace[] result) {
            if (mSucceed) {
                // Show the result on screen when detection is done.
                 addLog("Response: Success. Detected " + (result == null ? 0 : result.length) + " face(s) in " + mImageUri);
                Toast.makeText(getApplicationContext(),"Success",Toast.LENGTH_SHORT).show();
            }
            setUiAfterDetection(result, mSucceed);
        }
    }
    private static final int REQUEST_SELECT_IMAGE = 0;
    private Uri mImageUri;
    private Bitmap mBitmap;
    ProgressDialog mProgressDialog;
    Button selectImage, detect;
    TextView infoTxt;
    Context context;
    String debug;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_emotion);
        context=getBaseContext();
        mProgressDialog = new ProgressDialog(this);
        mProgressDialog.setTitle("Loading");
        selectImage = (Button) findViewById(R.id.select_image);
        detect = (Button) findViewById(R.id.detect);
        infoTxt = (TextView) findViewById(R.id.info);

        LogHelper.clearDetectionLog();
        detect.setEnabled(false);

    }

    // Called when image selection is done.
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case REQUEST_SELECT_IMAGE:
                if (resultCode == RESULT_OK) {
                    // If image is selected successfully, set the image URI and bitmap.
                    mImageUri = data.getData();
                    mBitmap = ImageHelper.loadSizeLimitedBitmapFromUri(mImageUri, getContentResolver());
                    if (mBitmap != null) {
                        // Show the image on screen.
                        ImageView imageView = (ImageView) findViewById(R.id.image);
                        imageView.setImageBitmap(mBitmap);

                        // Add detection log.
                        addLog("Image: " + mImageUri + " resized to " + mBitmap.getWidth()+ "x" + mBitmap.getHeight());
                    }
                //setDetectButtonEnabledStatus(true);

                    FaceAdapter faceListAdapter = new FaceAdapter(context,null,null);
                    ListView listView = (ListView) findViewById(R.id.list_detected_faces);
                    listView.setAdapter(faceListAdapter);

                    // Clear the information panel.
                    setInfo("");

                    detect.setEnabled(true);

                }
                break;
            default:
                break;
        }
    }
    public void detect(View view) {
        // Put the image into an input stream for detection.
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        mBitmap.compress(Bitmap.CompressFormat.JPEG, 100, output);
        ByteArrayInputStream inputStream = new ByteArrayInputStream(output.toByteArray());

        // Start a background task to detect faces in the image.
        new DetectionTask().execute(inputStream);

        // Prevent button click during detecting.
       // setAllButtonsEnabledStatus(false);
        detect.setEnabled(false);
    }
    // Called when the "Select Image" button is clicked.
    public void selectImage(View view) {
        Intent intent = new Intent(this, SelectImageActivity.class);
        startActivityForResult(intent, REQUEST_SELECT_IMAGE);
    }
    private void setInfo(String info) {
        infoTxt.setText(info);
    }

    private void setUiAfterDetection(EmotionFace[] result, boolean succeed) {
        // Detection is done, hide the progress dialog.
        mProgressDialog.dismiss();
        int faceCount=0;
        // Enable all the buttons.
        //setAllButtonsEnabledStatus(true);
        // Disable button "detect" as the image has already been detected.
        ///setDetectButtonEnabledStatus(false);

        if (succeed) {
            // The information about the detection result.
            if (result != null) {
                Gson gson = new Gson();

                // convert java object to JSON format,
                // and returned as JSON formatted string
                String json = gson.toJson(result);
                ImageView imageView = (ImageView) findViewById(R.id.image);
                imageView.setImageBitmap(ImageHelper.drawFaceRectanglesOnBitmap(mBitmap, result));
                debug=json;
                FaceAdapter faceListAdapter = new FaceAdapter(context,result,mBitmap);
                faceCount=faceListAdapter.getCount();
                // Show the detailed list of detected faces.
                ListView listView = (ListView) findViewById(R.id.list_detected_faces);
                listView.setAdapter(faceListAdapter);
            } else {
                setInfo("0 face detected");
            }

            setInfo(String.valueOf(faceCount)+" faces detected");

        }

        mImageUri = null;
        mBitmap = null;
    }
    private void addLog(String log) {
        LogHelper.addDetectionLog(log);
    }
    // View the log of service calls.
    public void viewLog(View view) {
        Intent intent = new Intent(this,LogActivity.class);
        startActivity(intent);
    }

}
