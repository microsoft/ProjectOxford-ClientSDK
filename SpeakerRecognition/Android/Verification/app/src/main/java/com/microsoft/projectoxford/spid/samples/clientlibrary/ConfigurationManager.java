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

import android.content.Context;
import android.content.SharedPreferences;
import android.content.res.Resources;

/**
 * A Configuration manager for loading and retrieving user and API-specific configurations.
 */
public class ConfigurationManager {
    private static final String PREFS_NAME = "SPID_PREFS";
    private static final String SPEAKER_ID_PARAM_NAME = "SPEAKER_ID";

    /**
     * Retrieves the project oxford subscription key for API-Calls
     *
     * @return Oxford subscription key for API-Calls
     */
    public static String getSubscriptionKey(Context c) {
        return c.getResources().getString(R.string.subscription_key);
    }

    /**
     * Retrieves the speakerId, returns null if not found
     *
     * @param context The context object
     * @return The speaker Id, null if not found
     */
    public static String getSpeakerId(Context context) {
        String speakerId = getValue(SPEAKER_ID_PARAM_NAME, context);
        if (speakerId.trim().length() == 0)
            return null;
        return speakerId;
    }

    /**
     * Persists the current speaker Id
     *
     * @param speakerId The speaker Id
     * @param context   The context object
     */
    public static void setSpeakerId(String speakerId, Context context) {
        setValue(SPEAKER_ID_PARAM_NAME, speakerId, context);
    }

    private static String getValue(String key, Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        return settings.getString(key, "");
    }

    private static void setValue(String key, String value, Context context) {
        SharedPreferences settings = context.getSharedPreferences(PREFS_NAME, 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(key, value);
        // Commit the edits!
        editor.commit();
    }
}
