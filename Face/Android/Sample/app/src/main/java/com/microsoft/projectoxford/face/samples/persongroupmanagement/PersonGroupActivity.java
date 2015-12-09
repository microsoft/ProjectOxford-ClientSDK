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
package com.microsoft.projectoxford.face.samples.persongroupmanagement;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v7.app.ActionBarActivity;
import android.view.ActionMode;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.GridView;
import android.widget.ImageView;
import android.widget.TextView;

import com.microsoft.projectoxford.face.FaceServiceClient;
import com.microsoft.projectoxford.face.samples.R;
import com.microsoft.projectoxford.face.samples.helper.LogHelper;
import com.microsoft.projectoxford.face.samples.helper.SampleApp;
import com.microsoft.projectoxford.face.samples.helper.StorageHelper;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Set;
import java.util.UUID;


public class PersonGroupActivity extends ActionBarActivity {
    // Background task of adding a person group.
    class AddPersonGroupTask extends AsyncTask<String, String, String> {
        // Indicate the next step is to add person in this group, or finish editing this group.
        boolean mAddPerson;

        AddPersonGroupTask(boolean addPerson) {
            mAddPerson = addPerson;
        }

        @Override
        protected String doInBackground(String... params) {
            addLog("Request: Creating person group " + params[0]);

            // Get an instance of face service client.
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            try{
                publishProgress("Syncing with server to add person group...");

                // Start creating person group in server.
                faceServiceClient.createPersonGroup(
                        params[0],
                        getString(R.string.user_provided_person_group_name),
                        getString(R.string.user_provided_person_group_description_data));

                return params[0];
            } catch (Exception e) {
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            setUiBeforeBackgroundTask();
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            setUiDuringBackgroundTask(progress[0]);
        }

        @Override
        protected void onPostExecute(String result) {
            progressDialog.dismiss();

            if (result != null) {
                addLog("Response: Success. Person group " + result + " created");

                personGroupExists = true;
                GridView gridView = (GridView) findViewById(R.id.gridView_persons);
                personGridViewAdapter = new PersonGridViewAdapter();
                gridView.setAdapter(personGridViewAdapter);

                setInfo("Success. Group " + result + " created");

                if (mAddPerson) {
                    addPerson();
                } else {
                    doneAndSave(false);
                }
            }
        }
    }

    class TrainPersonGroupTask extends AsyncTask<String, String, String> {

        @Override
        protected String doInBackground(String... params) {
            addLog("Request: Training group " + params[0]);

            // Get an instance of face service client.
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            try{
                publishProgress("Training person group...");

                faceServiceClient.trainPersonGroup(params[0]);
                return params[0];
            } catch (Exception e) {
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            setUiBeforeBackgroundTask();
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            setUiDuringBackgroundTask(progress[0]);
        }

        @Override
        protected void onPostExecute(String result) {
            progressDialog.dismiss();

            if (result != null) {
                addLog("Response: Success. Group " + result + " training completed");

                finish();
            }
        }
    }

    class DeletePersonTask extends AsyncTask<String, String, String> {
        String mPersonGroupId;
        DeletePersonTask(String personGroupId) {
            mPersonGroupId = personGroupId;
        }
        @Override
        protected String doInBackground(String... params) {
            // Get an instance of face service client.
            FaceServiceClient faceServiceClient = SampleApp.getFaceServiceClient();
            try{
                publishProgress("Deleting selected persons...");
                addLog("Request: Deleting person " + params[0]);

                UUID personId = UUID.fromString(params[0]);
                faceServiceClient.deletePerson(mPersonGroupId, personId);
                return params[0];
            } catch (Exception e) {
                publishProgress(e.getMessage());
                addLog(e.getMessage());
                return null;
            }
        }

        @Override
        protected void onPreExecute() {
            setUiBeforeBackgroundTask();
        }

        @Override
        protected void onProgressUpdate(String... progress) {
            setUiDuringBackgroundTask(progress[0]);
        }

        @Override
        protected void onPostExecute(String result) {
            progressDialog.dismiss();

            if (result != null) {
                setInfo("Person " + result + " successfully deleted");
                addLog("Response: Success. Deleting person " + result + " succeed");
            }
        }
    }

    private void setUiBeforeBackgroundTask() {
        progressDialog.show();
    }

    // Show the status of background detection task on screen.
    private void setUiDuringBackgroundTask(String progress) {
        progressDialog.setMessage(progress);

        setInfo(progress);
    }

    public void addPerson(View view) {
        if (!personGroupExists) {
            new AddPersonGroupTask(true).execute(personGroupId);
        } else {
            addPerson();
        }
    }

    private void addPerson() {
        setInfo("");

        Intent intent = new Intent(this, PersonActivity.class);
        intent.putExtra("AddNewPerson", true);
        intent.putExtra("PersonName", "");
        intent.putExtra("PersonGroupId", personGroupId);
        startActivity(intent);
    }

    boolean addNewPersonGroup;
    boolean personGroupExists;
    String personGroupId;
    String oldPersonGroupName;

    PersonGridViewAdapter personGridViewAdapter;

    // Progress dialog popped up when communicating with server.
    ProgressDialog progressDialog;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_person_group);

        Bundle bundle = getIntent().getExtras();
        if (bundle != null) {
            addNewPersonGroup = bundle.getBoolean("AddNewPersonGroup");
            oldPersonGroupName = bundle.getString("PersonGroupName");
            personGroupId = bundle.getString("PersonGroupId");
            personGroupExists = !addNewPersonGroup;
        }

        initializeGridView();

        progressDialog = new ProgressDialog(this);
        progressDialog.setTitle(getString(R.string.progress_dialog_title));

        EditText editTextPersonGroupName = (EditText)findViewById(R.id.edit_person_group_name);
        editTextPersonGroupName.setText(oldPersonGroupName);
    }

    private void initializeGridView() {
        GridView gridView = (GridView) findViewById(R.id.gridView_persons);

        gridView.setChoiceMode(GridView.CHOICE_MODE_MULTIPLE_MODAL);
        gridView.setMultiChoiceModeListener(new AbsListView.MultiChoiceModeListener() {
            @Override
            public void onItemCheckedStateChanged(ActionMode mode,
                                                  int position, long id, boolean checked) {
                personGridViewAdapter.personChecked.set(position, checked);

                GridView gridView = (GridView) findViewById(R.id.gridView_persons);
                gridView.setAdapter(personGridViewAdapter);
            }

            @Override
            public boolean onCreateActionMode(ActionMode mode, Menu menu) {
                MenuInflater inflater = mode.getMenuInflater();
                inflater.inflate(R.menu.menu_delete_items, menu);

                personGridViewAdapter.longPressed = true;

                GridView gridView = (GridView) findViewById(R.id.gridView_persons);
                gridView.setAdapter(personGridViewAdapter);

                Button addNewItem = (Button)findViewById(R.id.add_person);
                addNewItem.setEnabled(false);

                return true;
            }

            @Override
            public boolean onPrepareActionMode(ActionMode mode, Menu menu) {
                return false;
            }

            @Override
            public boolean onActionItemClicked(ActionMode mode, MenuItem item) {
                switch (item.getItemId()) {
                    case R.id.menu_delete_items:
                        deleteSelectedItems();
                        return true;
                    default:
                        return false;
                }
            }

            @Override
            public void onDestroyActionMode(ActionMode mode) {
                personGridViewAdapter.longPressed = false;

                for (int i = 0; i < personGridViewAdapter.personChecked.size(); ++i) {
                    personGridViewAdapter.personChecked.set(i, false);
                }

                GridView gridView = (GridView) findViewById(R.id.gridView_persons);
                gridView.setAdapter(personGridViewAdapter);

                Button addNewItem = (Button)findViewById(R.id.add_person);
                addNewItem.setEnabled(true);
            }
        });

        gridView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                if (!personGridViewAdapter.longPressed) {
                    String personId = personGridViewAdapter.personIdList.get(position);
                    String personName = StorageHelper.getPersonName(
                            personId, personGroupId, PersonGroupActivity.this);

                    Intent intent = new Intent(PersonGroupActivity.this, PersonActivity.class);
                    intent.putExtra("AddNewPerson", false);
                    intent.putExtra("PersonName", personName);
                    intent.putExtra("PersonId", personId);
                    intent.putExtra("PersonGroupId", personGroupId);
                    startActivity(intent);
                }
            }
        });
    }

    @Override
    protected void onResume() {
        super.onResume();

        if (personGroupExists) {
            GridView gridView = (GridView) findViewById(R.id.gridView_persons);
            personGridViewAdapter = new PersonGridViewAdapter();
            gridView.setAdapter(personGridViewAdapter);
        }
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);

        outState.putBoolean("AddNewPersonGroup", addNewPersonGroup);
        outState.putString("OldPersonGroupName", oldPersonGroupName);
        outState.putString("PersonGroupId", personGroupId);
        outState.putBoolean("PersonGroupExists", personGroupExists);
    }

    @Override
    protected void onRestoreInstanceState(@NonNull Bundle savedInstanceState) {
        super.onRestoreInstanceState(savedInstanceState);

        addNewPersonGroup = savedInstanceState.getBoolean("AddNewPersonGroup");
        personGroupId = savedInstanceState.getString("PersonGroupId");
        oldPersonGroupName = savedInstanceState.getString("OldPersonGroupName");
        personGroupExists = savedInstanceState.getBoolean("PersonGroupExists");
    }

    public void doneAndSave(View view) {
        if (!personGroupExists) {
            new AddPersonGroupTask(false).execute(personGroupId);
        } else {
            doneAndSave(true);
        }
    }

    private void doneAndSave(boolean trainPersonGroup) {
        EditText editTextPersonGroupName = (EditText)findViewById(R.id.edit_person_group_name);
        String newPersonGroupName = editTextPersonGroupName.getText().toString();
        if (newPersonGroupName.equals("")) {
            setInfo("Person group name could not be empty");
            return;
        }

        StorageHelper.setPersonGroupName(personGroupId, newPersonGroupName, PersonGroupActivity.this);

        if (trainPersonGroup) {
            new TrainPersonGroupTask().execute(personGroupId);
        } else {
            finish();
        }
    }

    private void deleteSelectedItems() {
        List<String> newPersonIdList = new ArrayList<>();
        List<Boolean> newPersonChecked = new ArrayList<>();
        List<String> personIdsToDelete = new ArrayList<>();
        for (int i = 0; i < personGridViewAdapter.personChecked.size(); ++i) {
            if (personGridViewAdapter.personChecked.get(i)) {
                String personId = personGridViewAdapter.personIdList.get(i);
                personIdsToDelete.add(personId);
                new DeletePersonTask(personGroupId).execute(personId);
            } else {
                newPersonIdList.add(personGridViewAdapter.personIdList.get(i));
                newPersonChecked.add(false);
            }
        }

        StorageHelper.deletePersons(personIdsToDelete, personGroupId, this);

        personGridViewAdapter.personIdList = newPersonIdList;
        personGridViewAdapter.personChecked = newPersonChecked;
        personGridViewAdapter.notifyDataSetChanged();
    }

    // Add a log item.
    private void addLog(String log) {
        LogHelper.addIdentificationLog(log);
    }

    // Set the information panel on screen.
    private void setInfo(String info) {
        TextView textView = (TextView) findViewById(R.id.info);
        textView.setText(info);
    }

    private class PersonGridViewAdapter extends BaseAdapter {

        List<String> personIdList;
        List<Boolean> personChecked;
        boolean longPressed;

        PersonGridViewAdapter() {
            longPressed = false;
            personIdList = new ArrayList<>();
            personChecked = new ArrayList<>();

            Set<String> personIdSet = StorageHelper.getAllPersonIds(personGroupId, PersonGroupActivity.this);
            for (String personId: personIdSet) {
                personIdList.add(personId);
                personChecked.add(false);
            }
        }

        @Override
        public int getCount() {
            return personIdList.size();
        }

        @Override
        public Object getItem(int position) {
            return personIdList.get(position);
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(final int position, View convertView, ViewGroup parent) {
            // set the item view
            if (convertView == null) {
                LayoutInflater layoutInflater = (LayoutInflater)getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                convertView = layoutInflater.inflate(R.layout.item_person, parent, false);
            }
            convertView.setId(position);

            String personId = personIdList.get(position);
            Set<String> faceIdSet = StorageHelper.getAllFaceIds(personId, PersonGroupActivity.this);
            if (!faceIdSet.isEmpty()) {
                Iterator<String> it = faceIdSet.iterator();
                Uri uri = Uri.parse(StorageHelper.getFaceUri(it.next(), PersonGroupActivity.this));
                ((ImageView)convertView.findViewById(R.id.image_person)).setImageURI(uri);
            } else {
                Drawable drawable = getResources().getDrawable(R.drawable.select_image);
                ((ImageView)convertView.findViewById(R.id.image_person)).setImageDrawable(drawable);
            }

            // set the text of the item
            String personName = StorageHelper.getPersonName(personId, personGroupId, PersonGroupActivity.this);
            ((TextView)convertView.findViewById(R.id.text_person)).setText(personName);

            // set the checked status of the item
            CheckBox checkBox = (CheckBox) convertView.findViewById(R.id.checkbox_person);
            if (longPressed) {
                checkBox.setVisibility(View.VISIBLE);

                checkBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                    @Override
                    public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                        personChecked.set(position, isChecked);
                    }
                });
                checkBox.setChecked(personChecked.get(position));
            } else {
                checkBox.setVisibility(View.INVISIBLE);
            }

            return convertView;
        }
    }
}
