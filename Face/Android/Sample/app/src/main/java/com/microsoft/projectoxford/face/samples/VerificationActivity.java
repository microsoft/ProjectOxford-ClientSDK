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
package com.microsoft.projectoxford.face.samples;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.microsoft.projectoxford.face.FaceServiceClient;
import com.microsoft.projectoxford.face.contract.Face;
import com.microsoft.projectoxford.face.contract.VerifyResult;
import com.microsoft.projectoxford.face.samples.helper.ImageHelper;
import com.microsoft.projectoxford.face.samples.helper.LogHelper;
import com.microsoft.projectoxford.face.samples.helper.SampleApp;
import com.microsoft.projectoxford.face.samples.helper.SelectImageActivity;
import com.microsoft.projectoxford.face.samples.log.VerificationLogActivity;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;

public class VerificationActivity extends ActionBarActivity {
    // Background task for face verification.
    private class VerificationTask extends AsyncTask<Void, String, VerifyResult> {
        // The IDs of two face to verify.
        private UUID mFaceId0;
        private UUID mFaceId1;

        VerificationTask (UUID faceId0, UUID faceId1) {
            mFaceId0 = faceId0;
            mFaceId1 = faceId1;
        }

        @Override
        protected VerifyResult doInBackground(Void... params) {
            // Get an instance of face service client to detect faces in image.
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            try{
                publishProgress("Verifying...");

                // Start verification.
                return faceServiceClient.verify(
                        mFaceId0,      /* The first face ID to verify */
                        mFaceId1);     /* The second face ID to verify */
            }  catch (Exception e) {
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            progressDialog.show();
            addLog("Request: Verifying face " + mFaceId0 + " and face " + mFaceId1);
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            progressDialog.setMessage(progress[0]);
            setInfo(progress[0]);
        }

        @Override
        protected void onPostExecute(VerifyResult result) {
            if (result != null) {
                addLog("Response: Success. Face " + mFaceId0 + " and face "
                        + mFaceId1 + (result.isIdentical ? " " : " don't ")
                        + "belong to the same person");
            }

            // Show the result on screen when verification is done.
            setUiAfterVerification(result);
        }
    }

    // Background task of face detection.
    private class DetectionTask extends AsyncTask<InputStream, String, Face[]> {
        // Index indicates detecting in which of the two images.
        private int mIndex;
        private boolean mSucceed = true;

        DetectionTask(int index) {
            mIndex = index;
        }

        @Override
        protected Face[] doInBackground(InputStream... params) {
            // Get an instance of face service client to detect faces in image.
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            try{
                publishProgress("Detecting...");

                // Start detection.
                return faceServiceClient.detect(
                        params[0],  /* Input stream of image to detect */
                        true,       /* Whether to return face ID */
                        false,       /* Whether to return face landmarks */
                        /* Which face attributes to analyze, currently we support:
                           age,gender,headPose,smile,facialHair */
                        null);
            }  catch (Exception e) {
                mSucceed = false;
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            progressDialog.show();
            addLog("Request: Detecting in image" + mIndex);
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            progressDialog.setMessage(progress[0]);
            setInfo(progress[0]);
        }

        @Override
        protected void onPostExecute(Face[] result) {
            // Show the result on screen when detection is done.
            setUiAfterDetection(result, mIndex, mSucceed);
        }
    }

    // Flag to indicate which task is to be performed.
    private static final int REQUEST_SELECT_IMAGE_0 = 0;
    private static final int REQUEST_SELECT_IMAGE_1 = 1;

    // The IDs of the two faces to be verified.
    private UUID mFaceId0;
    private UUID mFaceId1;

    // The two images from where we get the two faces to verify.
    private Bitmap mBitmap0;
    private Bitmap mBitmap1;

    // The adapter of the ListView which contains the detected faces from the two images.
    protected FaceListAdapter mFaceListAdapter0;
    protected FaceListAdapter mFaceListAdapter1;

    // Progress dialog popped up when communicating with server.
    ProgressDialog progressDialog;

    // When the activity is created, set all the member variables to initial state.
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_verification);

        // Initialize the two ListViews which contain the thumbnails of the detected faces.
        initializeFaceList(0);
        initializeFaceList(1);

        progressDialog = new ProgressDialog(this);
        progressDialog.setTitle(getString(R.string.progress_dialog_title));

        clearDetectedFaces(0);
        clearDetectedFaces(1);

        // Disable button "verify" as the two face IDs to verify are not ready.
        setVerifyButtonEnabledStatus(false);

        LogHelper.clearVerificationLog();
    }

    // Called when image selection is done. Begin detecting if the image is selected successfully.
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        // Index indicates which of the two images is selected.
        int index;
        if (requestCode == REQUEST_SELECT_IMAGE_0) {
            index = 0;
        } else if (requestCode == REQUEST_SELECT_IMAGE_1) {
            index = 1;
        } else {
            return;
        }

        if(resultCode == RESULT_OK) {
            // If image is selected successfully, set the image URI and bitmap.
            Bitmap bitmap = ImageHelper.loadSizeLimitedBitmapFromUri(
                    data.getData(), getContentResolver());
            if (bitmap != null) {
                // Image is select but not detected, disable verification button.
                setVerifyButtonEnabledStatus(false);
                clearDetectedFaces(index);

                // Set the image to detect.
                if (index == 0) {
                    mBitmap0 = bitmap;
                    mFaceId0 = null;
                } else {
                    mBitmap1 = bitmap;
                    mFaceId1 = null;
                }

                // Add verification log.
                addLog("Image" + index + ": " + data.getData() + " resized to " + bitmap.getWidth()
                        + "x" + bitmap.getHeight());

                // Start detecting in image.
                detect(bitmap, index);
            }
        }
    }

    // Clear the detected faces indicated by index.
    private void clearDetectedFaces(int index) {
        ListView faceList = (ListView) findViewById(
                index == 0 ? R.id.list_faces_0: R.id.list_faces_1);
        faceList.setVisibility(View.GONE);

        ImageView imageView =
                (ImageView) findViewById(index == 0 ? R.id.image_0: R.id.image_1);
        imageView.setImageResource(android.R.color.transparent);
    }

    // Called when the "Select Image0" button is clicked.
    public void selectImage0(View view) {
        selectImage(0);
    }

    // Called when the "Select Image1" button is clicked.
    public void selectImage1(View view) {
        selectImage(1);
    }

    // Called when the "Verify" button is clicked.
    public void verify(View view) {
        setAllButtonEnabledStatus(false);
        new VerificationTask(mFaceId0, mFaceId1).execute();
    }

    // View the log of service calls.
    public void viewLog(View view) {
        Intent intent = new Intent(this, VerificationLogActivity.class);
        startActivity(intent);
    }

    // Select the image indicated by index.
    private void selectImage(int index) {
        Intent intent = new Intent(this, SelectImageActivity.class);
        startActivityForResult(intent, index == 0 ? REQUEST_SELECT_IMAGE_0: REQUEST_SELECT_IMAGE_1);
    }

    // Set the select image button is enabled or not.
    private void setSelectImageButtonEnabledStatus(boolean isEnabled, int index) {
        Button button;

        if (index == 0) {
            button = (Button) findViewById(R.id.select_image_0);
        } else {
            button = (Button) findViewById(R.id.select_image_1);
        }

        button.setEnabled(isEnabled);

        Button viewLog = (Button) findViewById(R.id.view_log);
        viewLog.setEnabled(isEnabled);
    }

    // Set the verify button is enabled or not.
    private void setVerifyButtonEnabledStatus(boolean isEnabled) {
        Button button = (Button) findViewById(R.id.verify);
        button.setEnabled(isEnabled);
    }

    // Set all the buttons are enabled or not.
    private void setAllButtonEnabledStatus(boolean isEnabled) {
        Button selectImage0 = (Button) findViewById(R.id.select_image_0);
        selectImage0.setEnabled(isEnabled);

        Button selectImage1 = (Button) findViewById(R.id.select_image_1);
        selectImage1.setEnabled(isEnabled);

        Button verify = (Button) findViewById(R.id.verify);
        verify.setEnabled(isEnabled);

        Button viewLog = (Button) findViewById(R.id.view_log);
        viewLog.setEnabled(isEnabled);
    }

    // Initialize the ListView which contains the thumbnails of the detected faces.
    private void initializeFaceList(final int index) {
        ListView listView =
                (ListView) findViewById(index == 0 ? R.id.list_faces_0: R.id.list_faces_1);

        // When a detected face in the GridView is clicked, the face is selected to verify.
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                FaceListAdapter faceListAdapter =
                        index == 0 ? mFaceListAdapter0: mFaceListAdapter1;

                if (!faceListAdapter.faces.get(position).faceId.equals(
                        index == 0 ? mFaceId0: mFaceId1)) {
                    if (index == 0) {
                        mFaceId0 = faceListAdapter.faces.get(position).faceId;
                    } else {
                        mFaceId1 = faceListAdapter.faces.get(position).faceId;
                    }

                    ImageView imageView =
                            (ImageView) findViewById(index == 0 ? R.id.image_0: R.id.image_1);
                    imageView.setImageBitmap(faceListAdapter.faceThumbnails.get(position));

                    setInfo("");
                }

                // Show the list of detected face thumbnails.
                ListView listView = (ListView) findViewById(
                        index == 0 ? R.id.list_faces_0: R.id.list_faces_1);
                listView.setAdapter(faceListAdapter);
            }
        });
    }

    // Show the result on screen when verification is done.
    private void setUiAfterVerification(VerifyResult result) {
        // Verification is done, hide the progress dialog.
        progressDialog.dismiss();

        // Enable all the buttons.
        setAllButtonEnabledStatus(true);

        // Show verification result.
        if (result != null) {
            DecimalFormat formatter = new DecimalFormat("#0.00");
            String verificationResult = (result.isIdentical ? "The same person": "Different persons")
                    + ". The confidence is " + formatter.format(result.confidence);
            setInfo(verificationResult);
        }
    }

    // Show the result on screen when detection in image that indicated by index is done.
    private void setUiAfterDetection(Face[] result, int index, boolean succeed) {
        setSelectImageButtonEnabledStatus(true, index);

        if (succeed) {
            addLog("Response: Success. Detected "
                    + result.length + " face(s) in image" + index);

            setInfo(result.length + " face" + (result.length != 1 ? "s": "")  + " detected");

            // Show the detailed list of detected faces.
            FaceListAdapter faceListAdapter = new FaceListAdapter(result, index);

            // Set the default face ID to the ID of first face, if one or more faces are detected.
            if (faceListAdapter.faces.size() != 0) {
                if (index == 0) {
                    mFaceId0 = faceListAdapter.faces.get(0).faceId;
                } else {
                    mFaceId1 = faceListAdapter.faces.get(0).faceId;
                }
                // Show the thumbnail of the default face.
                ImageView imageView = (ImageView) findViewById(index == 0 ? R.id.image_0: R.id.image_1);
                imageView.setImageBitmap(faceListAdapter.faceThumbnails.get(0));
            }

            // Show the list of detected face thumbnails.
            ListView listView = (ListView) findViewById(
                    index == 0 ? R.id.list_faces_0: R.id.list_faces_1);
            listView.setAdapter(faceListAdapter);
            listView.setVisibility(View.VISIBLE);

            // Set the face list adapters and bitmaps.
            if (index == 0) {
                mFaceListAdapter0 = faceListAdapter;
                mBitmap0 = null;
            } else {
                mFaceListAdapter1 = faceListAdapter;
                mBitmap1 = null;
            }
        }

        if (result != null && result.length == 0) {
            setInfo("No face detected!");
        }

        if ((index == 0 && mBitmap1 == null) || (index == 1 && mBitmap0 == null)) {
            progressDialog.dismiss();
        }

        if (mFaceId0 != null && mFaceId1 != null) {
            setVerifyButtonEnabledStatus(true);
        }
    }

    // Start detecting in image specified by index.
    private void detect(Bitmap bitmap, int index) {
        // Put the image into an input stream for detection.
        ByteArrayOutputStream output = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 100, output);
        ByteArrayInputStream inputStream = new ByteArrayInputStream(output.toByteArray());

        // Start a background task to detect faces in the image.
        new DetectionTask(index).execute(inputStream);

        setSelectImageButtonEnabledStatus(false, index);

        // Set the status to show that detection starts.
        setInfo("Detecting...");
    }

    // Set the information panel on screen.
    private void setInfo(String info) {
        TextView textView = (TextView) findViewById(R.id.info);
        textView.setText(info);
    }

    // Add a log item.
    private void addLog(String log) {
        LogHelper.addVerificationLog(log);
    }

    // The adapter of the GridView which contains the thumbnails of the detected faces.
    private class FaceListAdapter extends BaseAdapter {
        // The detected faces.
        List<Face> faces;

        int mIndex;

        // The thumbnails of detected faces.
        List<Bitmap> faceThumbnails;

        // Initialize with detection result and index indicating on which image the result is got.
        FaceListAdapter(Face[] detectionResult, int index) {
            faces = new ArrayList<>();
            faceThumbnails = new ArrayList<>();
            mIndex = index;

            if (detectionResult != null) {
                faces = Arrays.asList(detectionResult);
                for (Face face: faces) {
                    try {
                        // Crop face thumbnail without landmarks drawn.
                        faceThumbnails.add(ImageHelper.generateFaceThumbnail(
                                index == 0 ? mBitmap0: mBitmap1, face.faceRectangle));
                    } catch (IOException e) {
                        // Show the exception when generating face thumbnail fails.
                        setInfo(e.getMessage());
                    }
                }
            }
        }

        @Override
        public int getCount() {
            return faces.size();
        }

        @Override
        public Object getItem(int position) {
            return faces.get(position);
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(final int position, View convertView, ViewGroup parent) {
            if (convertView == null) {
                LayoutInflater layoutInflater =
                        (LayoutInflater)getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = layoutInflater.inflate(R.layout.item_face, parent, false);
            }
            convertView.setId(position);

            Bitmap thumbnailToShow = faceThumbnails.get(position);
            if (mIndex == 0 && faces.get(position).faceId.equals(mFaceId0)) {
                thumbnailToShow = ImageHelper.highlightSelectedFaceThumbnail(thumbnailToShow);
            } else if (mIndex == 1 && faces.get(position).faceId.equals(mFaceId1)){
                thumbnailToShow = ImageHelper.highlightSelectedFaceThumbnail(thumbnailToShow);
            }

            // Show the face thumbnail.
            ((ImageView)convertView.findViewById(R.id.image_face)).setImageBitmap(thumbnailToShow);

            return convertView;
        }
    }
}
