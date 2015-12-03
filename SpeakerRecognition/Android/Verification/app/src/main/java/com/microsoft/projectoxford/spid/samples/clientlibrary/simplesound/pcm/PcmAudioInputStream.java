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

import java.io.Closeable;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;

public class PcmAudioInputStream extends InputStream implements Closeable {
    final PcmAudioFormat format;
    final DataInputStream dis;

    public PcmAudioInputStream(PcmAudioFormat format, InputStream is) {
        this.format = format;
        this.dis = new DataInputStream(is);
    }

    public int read() throws IOException {
        return dis.read();
    }

    public int[] readSamplesAsIntArray(int amount) throws IOException {
        byte[] bytez = new byte[amount * format.getBytePerSample()];
        int readAmount = dis.read(bytez);
        return Bytes.toIntArray(bytez, readAmount, format.getBytePerSample(), format.isBigEndian());
    }

    public int[] readAll() throws IOException {
        byte[] all = IOs.readAsByteArray(dis);
        return Bytes.toIntArray(all, all.length, format.getBytePerSample(), format.isBigEndian());
    }

    private static final int BYTE_BUFFER_SIZE = 4096;

    /**
     * reads samples as byte array. if there is not enough data for the amount of samples,
     * remaining
     * data is returned
     *
     * anyway. if the byte amount is not an order of bytes required for sample (such as 51 bytes
     * left but 16 bit samples)
     *
     * an IllegalStateException is thrown.
     *
     * @param amount amount of samples to read.
     * @return byte array.
     * @throws IOException           if there is an IO error.
     * @throws IllegalStateException if the amount of bytes read is not an order of correct.
     */
    public byte[] readSamplesByteArray(int amount) throws IOException {
        byte[] bytez = new byte[amount * format.getBytePerSample()];
        int readCount = dis.read(bytez);
        if (readCount != bytez.length) {
            validateReadCount(readCount);
            byte[] result = new byte[readCount];
            System.arraycopy(bytez, 0, result, 0, readCount);
            return result;
        } else
            return bytez;
    }

    private void validateReadCount(int readCount) {
        if (readCount % format.getBytePerSample() != 0)
            throw new IllegalStateException("unexpected amounts of bytes read from the input stream. " +
                    "Byte count must be an order of:" + format.getBytePerSample());
    }

    public int[] readSamplesAsIntArray(int frameStart, int frameEnd) throws IOException {
        skipSamples(frameStart * format.getBytePerSample());
        return readSamplesAsIntArray(frameEnd - frameStart);
    }

    /**
     * skips samples from the stream. if end of file is reached or if it cannot
     *
     * @param skipAmount amount of samples to skip
     * @return actual skipped sample count.
     * @throws IOException if there is a problem while skipping.
     */
    public int skipSamples(int skipAmount) throws IOException {
        long actualSkipped = dis.skip(skipAmount * format.getBytePerSample());
        return (int) actualSkipped / format.getBytePerSample();
    }

    public double[] readSamplesNormalized(int amount) throws IOException {
        int[] original = readSamplesAsIntArray(amount);
        double[] normalized = new double[original.length];
        final int max = 0x7fffffff >>> (31 - format.getSampleSizeInBits());
        for (int i = 0; i < normalized.length; i++) {
            normalized[i] = original[i] / max;
        }
        return normalized;
    }

    public double[] readSamplesNormalized() throws IOException {
        int[] original = readAll();
        double[] normalized = new double[original.length];
        final int max = 0x7fffffff >>> (31 - format.getSampleSizeInBits());
        for (int i = 0; i < normalized.length; i++) {
            normalized[i] = original[i] / max;
        }
        return normalized;
    }

    public void close() throws IOException {
        dis.close();
    }

    public short[] readSamplesShortArray(int amount) throws IOException {
        byte[] bytez = readSamplesByteArray(amount);
        return Bytes.toShortArray(bytez, bytez.length, format.isBigEndian());
    }
}