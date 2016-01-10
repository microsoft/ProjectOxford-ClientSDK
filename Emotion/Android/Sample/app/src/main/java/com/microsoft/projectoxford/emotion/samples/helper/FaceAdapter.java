package com.microsoft.projectoxford.emotion.samples.helper;

import android.content.Context;
import android.graphics.Bitmap;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.microsoft.emotion.R;
import com.microsoft.projectoxford.emotionlib.contract.EmotionFace;

import java.io.IOException;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

/**
 * Created by rcrespoy on 1/9/2016.
 */
// The adapter of the GridView which contains the details of the detected faces.
public class FaceAdapter extends BaseAdapter {
    // The detected faces.
    List<EmotionFace> faces;

    // The thumbnails of detected faces.
    List<Bitmap> faceThumbnails;
    Context context;
    // Initialize with detection result.
    public FaceAdapter(Context context,EmotionFace[] detectionResult, Bitmap mBitmap) {

        faces = new ArrayList<>();
        faceThumbnails = new ArrayList<>();
        this.context=context;
        if (detectionResult != null) {
            faces = Arrays.asList(detectionResult);
            for (EmotionFace face : faces) {
                try {
                    // Crop face thumbnail with five main landmarks drawn from original image.
                    faceThumbnails.add(ImageHelper.generateFaceThumbnail(
                            mBitmap, face.faceRectangle));
                } catch (IOException e) {
                    // Show the exception when generating face thumbnail fails.
                    ////setInfo(e.getMessage());
                }
            }
        }
    }

    @Override
    public boolean isEnabled(int position) {
        return false;
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
            LayoutInflater layoutInflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            convertView = layoutInflater.inflate(R.layout.item_face_with_description, parent, false);
        }
        convertView.setId(position);

        // Show the face thumbnail.
        ((ImageView) convertView.findViewById(R.id.face_thumbnail)).setImageBitmap(
                faceThumbnails.get(position));

        // Show the face details.
        DecimalFormat formatter = new DecimalFormat("#0.0");

        String face_description = "Anger: " +String.valueOf(faces.get(position).scores.anger) + "\n"
                + "Contempt: " + String.valueOf(faces.get(position).scores.contempt)+ "\n"
                + "Disgust: " + String.valueOf(faces.get(position).scores.disgust)+ "\n"
                + "Fear: " + String.valueOf(faces.get(position).scores.fear)+ "\n"
                + "Happiness: " + String.valueOf(faces.get(position).scores.happiness)+ "\n"
                + "Neutral: " + String.valueOf(faces.get(position).scores.neutral)+ "\n"
                + "Sadness: " + String.valueOf(faces.get(position).scores.sadness)+ "\n"
                + "Surprise: " + String.valueOf(faces.get(position).scores.surprise);

        ((TextView) convertView.findViewById(R.id.text_detected_face)).setText(face_description);

        return convertView;
    }
}