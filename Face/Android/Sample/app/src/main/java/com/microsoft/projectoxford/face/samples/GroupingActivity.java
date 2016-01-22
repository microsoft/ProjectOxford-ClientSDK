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
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.microsoft.projectoxford.face.FaceServiceClient;
import com.microsoft.projectoxford.face.contract.Face;
import com.microsoft.projectoxford.face.contract.GroupResult;
import com.microsoft.projectoxford.face.samples.helper.EmbeddedGridView;
import com.microsoft.projectoxford.face.samples.helper.ImageHelper;
import com.microsoft.projectoxford.face.samples.helper.LogHelper;
import com.microsoft.projectoxford.face.samples.helper.SampleApp;
import com.microsoft.projectoxford.face.samples.helper.SelectImageActivity;
import com.microsoft.projectoxford.face.samples.log.GroupingLogActivity;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

public class GroupingActivity extends ActionBarActivity {
    // Background task for face grouping.
    class GroupingTask extends AsyncTask<UUID, String, GroupResult> {
        @Override
        protected GroupResult doInBackground(UUID... params) {
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            addLog("Request: Grouping " + params.length + " face(s)");
            try{
                publishProgress("Grouping...");

                // Start grouping, params are face IDs.
                return faceServiceClient.group(params);
            }  catch (Exception e) {
                addLog(e.getMessage());
                publishProgress(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            mProgressDialog.show();
        }

        @Override
        protected void onProgressUpdate(String... values) {
            setUiDuringBackgroundTask(values[0]);
        }

        @Override
        protected void onPostExecute(GroupResult result) {
            if (result != null) {
                addLog("Response: Success. Grouped into " + result.groups.size() + " face group(s).");
            }
            setUiAfterGrouping(result);
        }
    }

    // Background task for face detection
    class DetectionTask extends AsyncTask<InputStream, String, Face[]> {
        private boolean mSucceed = true;
        @Override
        protected Face[] doInBackground(InputStream... params) {
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
            mProgressDialog.show();
        }

        @Override
        protected void onProgressUpdate(String... values) {
            setUiDuringBackgroundTask(values[0]);
        }

        @Override
        protected void onPostExecute(Face[] result) {
            if (mSucceed) {
                addLog("Response: Success. Detected " + result.length + " Face(s) in image");
            }
            setUiAfterDetection(result);
        }
    }

    void setUiDuringBackgroundTask(String progress) {
        mProgressDialog.setMessage(progress);
        setInfo(progress);
    }

    void setUiAfterDetection(Face[] result) {
        mProgressDialog.dismiss();

        setAllButtonsEnabledStatus(true);

        if (result != null) {
            setInfo("Detection is done");

            // Show the detailed list of original faces.
            mFaceListAdapter.addFaces(result);
            GridView listView = (GridView) findViewById(R.id.all_faces);
            listView.setAdapter(mFaceListAdapter);

            TextView textView = (TextView) findViewById(R.id.text_all_faces);
            textView.setText(String.format(
                    "%d face%s in total",
                    mFaceListAdapter.faces.size(),
                    mFaceListAdapter.faces.size() != 1 ? "s" : ""));
        }

        if (mFaceListAdapter.faces.size() >= 2 && mFaceListAdapter.faces.size() <= 100) {
            setGroupButtonEnabledStatus(true);
        } else {
            setGroupButtonEnabledStatus(false);
        }
    }

    void setUiAfterGrouping(GroupResult result) {
        mProgressDialog.dismiss();

        setAllButtonsEnabledStatus(true);

        if (result != null) {
            setInfo("Grouping is done");
            setGroupButtonEnabledStatus(false);

            // Show the result of face grouping.
            ListView groupedFaces = (ListView) findViewById(R.id.grouped_faces);
            FaceGroupsAdapter faceGroupsAdapter = new FaceGroupsAdapter(result);
            groupedFaces.setAdapter(faceGroupsAdapter);
        }
    }

    // The faces in this image are added to face collection for grouping.
    Bitmap mBitmap;

    // The face collection view adapter.
    FaceListAdapter mFaceListAdapter;

    // Flag to indicate which task is to be performed.
    protected static final int REQUEST_SELECT_IMAGE = 0;

    // Progress dialog popped up when communicating with server.
    ProgressDialog mProgressDialog;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_grouping);

        mProgressDialog = new ProgressDialog(this);
        mProgressDialog.setTitle(getString(R.string.progress_dialog_title));

        mFaceListAdapter = new FaceListAdapter();

        setGroupButtonEnabledStatus(false);

        LogHelper.clearGroupingLog();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_SELECT_IMAGE) {
            if(resultCode == RESULT_OK) {
                mBitmap = ImageHelper.loadSizeLimitedBitmapFromUri(data.getData(), getContentResolver());
                if (mBitmap != null) {
                    View originalFaces = findViewById(R.id.all_faces);
                    originalFaces.setVisibility(View.VISIBLE);

                    // Show the result of face grouping.
                    ListView groupedFaces = (ListView) findViewById(R.id.grouped_faces);
                    FaceGroupsAdapter faceGroupsAdapter = new FaceGroupsAdapter(null);
                    groupedFaces.setAdapter(faceGroupsAdapter);

                    // Put the image into an input stream for detection.
                    ByteArrayOutputStream output = new ByteArrayOutputStream();
                    mBitmap.compress(Bitmap.CompressFormat.JPEG, 100, output);
                    ByteArrayInputStream inputStream = new ByteArrayInputStream(output.toByteArray());

                    setAllButtonsEnabledStatus(false);

                    addLog("Request: Detecting in image " + data.getData());
                    // Start a background task to detect faces in the image.
                    new DetectionTask().execute(inputStream);
                }
            }
        }
    }

    public void addFaces(View view) {
        Intent intent = new Intent(this, SelectImageActivity.class);
        startActivityForResult(intent, REQUEST_SELECT_IMAGE);
    }

    public void group(View view) {
        List<UUID> faceIds = new ArrayList<>();
        for (Face face: mFaceListAdapter.faces) {
            faceIds.add(face.faceId);
        }

        if (faceIds.size() > 0) {
            new GroupingTask().execute(faceIds.toArray(new UUID[faceIds.size()]));
            setAllButtonsEnabledStatus(false);
        } else {
            TextView textView = (TextView) findViewById(R.id.info);
            textView.setText(R.string.no_face_to_group);
        }
    }

    public void viewLog(View view) {
        Intent intent = new Intent(this, GroupingLogActivity.class);
        startActivity(intent);
    }

    // Set whether the buttons are enabled.
    private void setAllButtonsEnabledStatus(boolean isEnabled) {
        Button selectImageButton = (Button) findViewById(R.id.add_faces);
        selectImageButton.setEnabled(isEnabled);

        Button groupButton = (Button) findViewById(R.id.group);
        groupButton.setEnabled(isEnabled);

        Button ViewLogButton = (Button) findViewById(R.id.view_log);
        ViewLogButton.setEnabled(isEnabled);
    }

    // Set the group button is enabled or not.
    private void setGroupButtonEnabledStatus(boolean isEnabled) {
        Button button = (Button) findViewById(R.id.group);
        button.setEnabled(isEnabled);
    }

    // Set the information panel on screen.
    private void setInfo(String info) {
        TextView textView = (TextView) findViewById(R.id.info);
        textView.setText(info);
    }

    // Add a log item.
    private void addLog(String log) {
        LogHelper.addGroupingLog(log);
    }

    // The adapter of the GridView which contains the thumbnails of the detected faces.
    private class FaceListAdapter extends BaseAdapter {
        // The detected faces.
        List<Face> faces;

        // The thumbnails of detected faces.
        List<Bitmap> faceThumbnails;

        Map<UUID, Bitmap> faceIdThumbnailMap;

        FaceListAdapter() {
            faces = new ArrayList<>();
            faceThumbnails = new ArrayList<>();
            faceIdThumbnailMap = new HashMap<>();
        }

        public void addFaces(Face[] detectionResult) {
            if (detectionResult != null) {
                List<Face> detectedFaces = Arrays.asList(detectionResult);
                for (Face face: detectedFaces) {
                    faces.add(face);
                    try {
                        Bitmap faceThumbnail = ImageHelper.generateFaceThumbnail(
                                mBitmap, face.faceRectangle);
                        faceThumbnails.add(faceThumbnail);
                        faceIdThumbnailMap.put(face.faceId, faceThumbnail);
                    } catch (IOException e) {
                        // Show the exception when generating face thumbnail fails.
                        TextView textView = (TextView)findViewById(R.id.info);
                        textView.setText(e.getMessage());
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
                LayoutInflater layoutInflater = (LayoutInflater)getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = layoutInflater.inflate(R.layout.item_face, parent, false);
            }
            convertView.setId(position);

            // Show the face thumbnail.
            ((ImageView)convertView.findViewById(R.id.image_face)).setImageBitmap(faceThumbnails.get(position));

            return convertView;
        }
    }

    // The adapter of the GridView which contains the thumbnails of the detected faces.
    private class FaceGroupsAdapter extends BaseAdapter {
        // The face groups.
        List<List<UUID> > faceGroups;

        FaceGroupsAdapter(GroupResult result) {
            faceGroups = new ArrayList<>();
            if (result != null) {
                for (UUID[] group: result.groups) {
                    faceGroups.add(Arrays.asList(group));
                }
                faceGroups.add(result.messyGroup);
            }
        }

        @Override
        public int getCount() {
            return faceGroups.size();
        }

        @Override
        public Object getItem(int position) {
            return faceGroups.get(position);
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(final int position, View convertView, ViewGroup parent) {
            if (convertView == null) {
                LayoutInflater layoutInflater = (LayoutInflater)getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = layoutInflater.inflate(R.layout.item_face_group, parent, false);
            }
            convertView.setId(position);

            String faceGroupName = "Group " + position + ": " + faceGroups.get(position).size() + " face(s)";
            if (position == faceGroups.size() - 1) {
                faceGroupName = "Messy Group: " + faceGroups.get(position).size() + " face(s)";
            }
            ((TextView) convertView.findViewById(R.id.face_group_name)).setText(faceGroupName);

            FacesAdapter facesAdapter = new FacesAdapter(faceGroups.get(position));
            EmbeddedGridView gridView = (EmbeddedGridView) convertView.findViewById(R.id.faces);
            gridView.setAdapter(facesAdapter);

            return convertView;
        }
    }

    // The adapter of the GridView which contains the thumbnails of the detected faces.
    private class FacesAdapter extends BaseAdapter {
        // The detected faces.
        List<UUID> faces;

        FacesAdapter(List<UUID> result) {
            faces = new ArrayList<>();
            faces.addAll(result);
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
                LayoutInflater layoutInflater = (LayoutInflater)getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = layoutInflater.inflate(R.layout.item_face, parent, false);
            }
            convertView.setId(position);

            // Show the face thumbnail.
            ((ImageView)convertView.findViewById(R.id.image_face)).setImageBitmap(
                    mFaceListAdapter.faceIdThumbnailMap.get(faces.get(position)));

            return convertView;
        }
    }
}
