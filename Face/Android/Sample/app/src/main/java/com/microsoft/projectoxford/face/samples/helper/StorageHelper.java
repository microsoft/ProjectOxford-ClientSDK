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
package com.microsoft.projectoxford.face.samples.helper;

import android.content.Context;
import android.content.SharedPreferences;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

/**
 * Defined several functions to manage local storage.
 */
public class StorageHelper {
    public static Set<String> getAllPersonGroupIds(Context context) {
        SharedPreferences personGroupIdSet =
                context.getSharedPreferences("PersonGroupIdSet", Context.MODE_PRIVATE);
        return personGroupIdSet.getStringSet("PersonGroupIdSet", new HashSet<String>());
    }

    public static String getPersonGroupName(String personGroupId, Context context) {
        SharedPreferences personGroupIdNameMap =
                context.getSharedPreferences("PersonGroupIdNameMap", Context.MODE_PRIVATE);
        return personGroupIdNameMap.getString(personGroupId, "");
    }

    public static void setPersonGroupName(String personGroupIdToAdd, String personGroupName, Context context) {
        SharedPreferences personGroupIdNameMap =
                context.getSharedPreferences("PersonGroupIdNameMap", Context.MODE_PRIVATE);

        SharedPreferences.Editor personGroupIdNameMapEditor = personGroupIdNameMap.edit();
        personGroupIdNameMapEditor.putString(personGroupIdToAdd, personGroupName);
        personGroupIdNameMapEditor.commit();

        Set<String> personGroupIds = getAllPersonGroupIds(context);
        Set<String> newPersonGroupIds = new HashSet<>();
        for (String personGroupId: personGroupIds) {
            newPersonGroupIds.add(personGroupId);
        }
        newPersonGroupIds.add(personGroupIdToAdd);
        SharedPreferences personGroupIdSet =
                context.getSharedPreferences("PersonGroupIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor personGroupIdSetEditor = personGroupIdSet.edit();
        personGroupIdSetEditor.putStringSet("PersonGroupIdSet", newPersonGroupIds);
        personGroupIdSetEditor.commit();
    }

    public static void deletePersonGroups(List<String> personGroupIdsToDelete, Context context) {
        SharedPreferences personGroupIdNameMap =
                context.getSharedPreferences("PersonGroupIdNameMap", Context.MODE_PRIVATE);
        SharedPreferences.Editor personGroupIdNameMapEditor = personGroupIdNameMap.edit();
        for (String personGroupId: personGroupIdsToDelete) {
            personGroupIdNameMapEditor.remove(personGroupId);
        }
        personGroupIdNameMapEditor.commit();

        Set<String> personGroupIds = getAllPersonGroupIds(context);
        Set<String> newPersonGroupIds = new HashSet<>();
        for (String personGroupId: personGroupIds) {
            if (!personGroupIdsToDelete.contains(personGroupId)) {
                newPersonGroupIds.add(personGroupId);
            }
        }
        SharedPreferences personGroupIdSet =
                context.getSharedPreferences("PersonGroupIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor personGroupIdSetEditor = personGroupIdSet.edit();
        personGroupIdSetEditor.putStringSet("PersonGroupIdSet", newPersonGroupIds);
        personGroupIdSetEditor.commit();
    }

    public static Set<String> getAllPersonIds(String personGroupId, Context context) {
        SharedPreferences personIdSet =
                context.getSharedPreferences(personGroupId + "PersonIdSet", Context.MODE_PRIVATE);
        return personIdSet.getStringSet("PersonIdSet", new HashSet<String>());
    }

    public static String getPersonName(String personId, String personGroupId, Context context) {
        SharedPreferences personIdNameMap =
                context.getSharedPreferences(personGroupId + "PersonIdNameMap", Context.MODE_PRIVATE);
        return personIdNameMap.getString(personId, "");
    }

    public static void setPersonName(String personIdToAdd, String personName, String personGroupId, Context context) {
        SharedPreferences personIdNameMap =
                context.getSharedPreferences(personGroupId + "PersonIdNameMap", Context.MODE_PRIVATE);

        SharedPreferences.Editor personIdNameMapEditor = personIdNameMap.edit();
        personIdNameMapEditor.putString(personIdToAdd, personName);
        personIdNameMapEditor.commit();

        Set<String> personIds = getAllPersonIds(personGroupId, context);
        Set<String> newPersonIds = new HashSet<>();
        for (String personId: personIds) {
            newPersonIds.add(personId);
        }
        newPersonIds.add(personIdToAdd);
        SharedPreferences personIdSet =
                context.getSharedPreferences(personGroupId + "PersonIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor personIdSetEditor = personIdSet.edit();
        personIdSetEditor.putStringSet("PersonIdSet", newPersonIds);
        personIdSetEditor.commit();
    }

    public static void deletePersons(List<String> personIdsToDelete, String personGroupId, Context context) {
        SharedPreferences personIdNameMap =
                context.getSharedPreferences(personGroupId + "PersonIdNameMap", Context.MODE_PRIVATE);
        SharedPreferences.Editor personIdNameMapEditor = personIdNameMap.edit();
        for (String personId: personIdsToDelete) {
            personIdNameMapEditor.remove(personId);
        }
        personIdNameMapEditor.commit();

        Set<String> personIds = getAllPersonIds(personGroupId, context);
        Set<String> newPersonIds = new HashSet<>();
        for (String personId: personIds) {
            if (!personIdsToDelete.contains(personId)) {
                newPersonIds.add(personId);
            }
        }
        SharedPreferences personIdSet =
                context.getSharedPreferences(personGroupId + "PersonIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor personIdSetEditor = personIdSet.edit();
        personIdSetEditor.putStringSet("PersonIdSet", newPersonIds);
        personIdSetEditor.commit();
    }

    public static Set<String> getAllFaceIds(String personId, Context context) {
        SharedPreferences faceIdSet =
                context.getSharedPreferences(personId + "FaceIdSet", Context.MODE_PRIVATE);
        return faceIdSet.getStringSet("FaceIdSet", new HashSet<String>());
    }

    public static String getFaceUri(String faceId, Context context) {
        SharedPreferences faceIdUriMap =
                context.getSharedPreferences("FaceIdUriMap", Context.MODE_PRIVATE);
        return faceIdUriMap.getString(faceId, "");
    }

    public static void setFaceUri(String faceIdToAdd, String faceUri, String personId, Context context) {
        SharedPreferences faceIdUriMap =
                context.getSharedPreferences("FaceIdUriMap", Context.MODE_PRIVATE);

        SharedPreferences.Editor faceIdUriMapEditor = faceIdUriMap.edit();
        faceIdUriMapEditor.putString(faceIdToAdd, faceUri);
        faceIdUriMapEditor.commit();

        Set<String> faceIds = getAllFaceIds(personId, context);
        Set<String> newFaceIds = new HashSet<>();
        for (String faceId: faceIds) {
            newFaceIds.add(faceId);
        }
        newFaceIds.add(faceIdToAdd);
        SharedPreferences faceIdSet =
                context.getSharedPreferences(personId + "FaceIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor faceIdSetEditor = faceIdSet.edit();
        faceIdSetEditor.putStringSet("FaceIdSet", newFaceIds);
        faceIdSetEditor.commit();
    }

    public static void deleteFaces(List<String> faceIdsToDelete, String personId, Context context) {
        Set<String> faceIds = getAllFaceIds(personId, context);
        Set<String> newFaceIds = new HashSet<>();
        for (String faceId: faceIds) {
            if (!faceIdsToDelete.contains(faceId)) {
                newFaceIds.add(faceId);
            }
        }
        SharedPreferences faceIdSet =
                context.getSharedPreferences(personId + "FaceIdSet", Context.MODE_PRIVATE);
        SharedPreferences.Editor faceIdSetEditor = faceIdSet.edit();
        faceIdSetEditor.putStringSet("FaceIdSet", newFaceIds);
        faceIdSetEditor.commit();
    }
}
