//https://code.google.com/p/simplesound/
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
package com.microsoft.projectoxford.spid.samples.clientlibrary.simplesound.pcm;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;

public class WavFileReader {
    private final File file;
    private final RiffHeaderData riffHeaderData;

    public WavFileReader(String fileName) throws IOException {
        this(new File(fileName));
    }

    public WavFileReader(File file) throws IOException {
        this.file = file;
        riffHeaderData = new RiffHeaderData(file);
    }

    public PcmAudioInputStream getStream() throws IOException {
        PcmAudioInputStream asis = new PcmAudioInputStream(
                riffHeaderData.getFormat(),
                new FileInputStream(file));
        long amount = asis.skip(RiffHeaderData.PCM_RIFF_HEADER_SIZE);
        if (amount < RiffHeaderData.PCM_RIFF_HEADER_SIZE)
            throw new IllegalArgumentException("cannot skip necessary amount of bytes from underlying stream.");
        return asis;
    }

    public short[] getSamplesAsShorts(int frameStart, int frameEnd) throws IOException {
        validateFrameBoundaries(frameStart, frameEnd);
        PcmAudioInputStream stream = getStream();
        try {
            stream.skipSamples(frameStart);
            return stream.readSamplesShortArray(frameEnd - frameStart);
        } finally {
            stream.close();
        }
    }

    private void validateFrameBoundaries(int frameStart, int frameEnd) {
        if (frameStart < 0)
            throw new IllegalArgumentException("Start Frame cannot be negative:" + frameStart);
        if (frameEnd < frameStart)
            throw new IllegalArgumentException("Start Frame cannot be after end frame. Start:"
                    + frameStart + ", end:" + frameEnd);
        if (frameEnd > riffHeaderData.getSampleCount())
            throw new IllegalArgumentException("Frame count out of bounds. Max sample count:"
                    + riffHeaderData.getSampleCount() + " but frame is:" + frameEnd);
    }

    public int[] getAllSamples() throws IOException {
        PcmAudioInputStream stream = getStream();
        try {
            return stream.readAll();
        } finally {
            stream.close();
        }
    }

    public int[] getSamplesAsInts(int frameStart, int frameEnd) throws IOException {
        validateFrameBoundaries(frameStart, frameEnd);
        PcmAudioInputStream stream = getStream();
        try {
            stream.skipSamples(frameStart);
            return stream.readSamplesAsIntArray(frameEnd - frameStart);
        } finally {
            stream.close();
        }
    }

    public PcmAudioFormat getFormat() {
        return riffHeaderData.getFormat();
    }

    public int getSampleCount() {
        return riffHeaderData.getSampleCount();
    }

    public File getFile() {
        return file;
    }
}