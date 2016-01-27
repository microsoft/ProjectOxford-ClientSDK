package com.microsoft.projectoxford.emotion.samples;

import android.app.AlertDialog;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;

import com.microsoft.emotion.R;

/**
 * Created by rcrespoy on 1/10/2016.
 */
public class MainActivity extends AppCompatActivity{
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    if (getString(R.string.subscription_key).startsWith("Please")) {
        new AlertDialog.Builder(this)
                .setTitle(getString(R.string.add_subscription_key_tip_title))
                .setMessage(getString(R.string.add_subscription_key_tip))
                .setCancelable(false)
                .show();
    }else{
        Intent intent = new Intent(this, EmotionActivity.class);
        startActivity(intent);

    }
    }
}
