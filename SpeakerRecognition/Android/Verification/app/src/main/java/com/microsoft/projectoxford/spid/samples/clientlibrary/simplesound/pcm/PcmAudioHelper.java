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

import java.io.*;

public class PcmAudioHelper {
    public static void convertRawToWav(WavAudioFormat af, File rawSource, File wavTarget) throws IOException {
        DataOutputStream dos = new DataOutputStream(new FileOutputStream(wavTarget));
        dos.write(new RiffHeaderData(af, 0).asByteArray());
        DataInputStream dis = new DataInputStream(new FileInputStream(rawSource));
        byte[] buffer = new byte[4096];
        int i;
        int total = 0;
        while ((i = dis.read(buffer)) != -1) {
            total += i;
            dos.write(buffer, 0, i);
        }
        dos.close();
        modifyRiffSizeData(wavTarget, total);
    }

    public static void convertWavToRaw(File wavSource, File rawTarget) throws IOException {
        IOs.copy(new WavFileReader(wavSource).getStream(), new FileOutputStream(rawTarget));
    }

    public static double[] readAllFromWavNormalized(String fileName) throws IOException {
        return new WavFileReader(new File(fileName)).getStream().readSamplesNormalized();
    }

    /**
     * Modifies the size information in a wav file header.
     *
     * @param wavFile a wav file
     * @param size    size to replace the header.
     * @throws IOException if an error occurs whule accesing the data.
     */
    static void modifyRiffSizeData(File wavFile, int size) throws IOException {
        RandomAccessFile raf = new RandomAccessFile(wavFile, "rw");
        raf.seek(RiffHeaderData.RIFF_CHUNK_SIZE_INDEX);
        raf.write(Bytes.toByteArray(size + 36, false));
        raf.seek(RiffHeaderData.RIFF_SUBCHUNK2_SIZE_INDEX);
        raf.write(Bytes.toByteArray(size, false));
        raf.close();
    }

    public static void generateSilenceWavFile(WavAudioFormat wavAudioFormat, File file, double sec) throws IOException {
        WavFileWriter wfr = new WavFileWriter(wavAudioFormat, file);
        int[] empty = new int[(int) (sec * wavAudioFormat.getSampleRate())];
        try {
            wfr.write(empty);
        } finally {
            wfr.close();
        }
    }
}