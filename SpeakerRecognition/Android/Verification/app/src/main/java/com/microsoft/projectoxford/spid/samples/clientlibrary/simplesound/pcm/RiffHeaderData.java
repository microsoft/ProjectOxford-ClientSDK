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

class RiffHeaderData {
    public static final int PCM_RIFF_HEADER_SIZE = 44;
    public static final int RIFF_CHUNK_SIZE_INDEX = 4;
    public static final int RIFF_SUBCHUNK2_SIZE_INDEX = 40;
    private final PcmAudioFormat format;
    private final int totalSamplesInByte;

    public RiffHeaderData(PcmAudioFormat format, int totalSamplesInByte) {
        this.format = format;
        this.totalSamplesInByte = totalSamplesInByte;
    }

    public double timeSeconds() {
        return (double) totalSamplesInByte / format.getBytePerSample() / format.getSampleRate();
    }

    public RiffHeaderData(DataInputStream dis) throws IOException {
        try {
            byte[] buf4 = new byte[4];
            byte[] buf2 = new byte[2];
            dis.skipBytes(4 + 4 + 4 + 4 + 4 + 2);
            dis.readFully(buf2);
            final int channels = Bytes.toInt(buf2, false);
            dis.readFully(buf4);
            final int sampleRate = Bytes.toInt(buf4, false);
            dis.skipBytes(4 + 2);
            dis.readFully(buf2);
            final int sampleSizeInBits = Bytes.toInt(buf2, false);
            dis.skipBytes(4);
            dis.readFully(buf4);
            totalSamplesInByte = Bytes.toInt(buf4, false);
            format = new WavAudioFormat.Builder().
                    channels(channels).
                    sampleRate(sampleRate).
                    sampleSizeInBits(sampleSizeInBits).
                    build();
        } finally {
            IOs.closeSilently(dis);
        }
    }

    public RiffHeaderData(File file) throws IOException {
        this(new DataInputStream(new FileInputStream(file)));
    }

    public byte[] asByteArray() {
        ByteArrayOutputStream baos = null;
        try {
            baos = new ByteArrayOutputStream();
            // ChunkID (the String "RIFF") 4 Bytes
            baos.write(Bytes.toByteArray(0x52494646, true));
            // ChunkSize (Whole file size in byte minus 8 bytes ) , or (4 + (8 + SubChunk1Size) + (8 + SubChunk2Size))
            // little endian 4 Bytes.
            baos.write(Bytes.toByteArray(36 + totalSamplesInByte, false));
            // Format (the String "WAVE") 4 Bytes big endian
            baos.write(Bytes.toByteArray(0x57415645, true));
            // Subchunk1
            // Subchunk1ID (the String "fmt ") 4 bytes big endian.
            baos.write(Bytes.toByteArray(0x666d7420, true));
            // Subchunk1Size. 16 for the PCM. little endian 4 bytes.
            baos.write(Bytes.toByteArray(16, false));
            // AudioFormat , for PCM = 1, Little endian 2 Bytes.
            baos.write(Bytes.toByteArray((short) 1, false));
            // Number of channels Mono = 1, Stereo = 2  Little Endian , 2 bytes.
            int channels = format.getChannels();
            baos.write(Bytes.toByteArray((short) channels, false));
            // SampleRate (8000, 44100 etc.) little endian, 4 bytes
            int sampleRate = format.getSampleRate();
            baos.write(Bytes.toByteArray(sampleRate, false));
            // byte rate (SampleRate * NumChannels * BitsPerSample/8) little endian, 4 bytes.
            baos.write(Bytes.toByteArray(channels * sampleRate * format.getBytePerSample(), false));
            // Block Allign == NumChannels * BitsPerSample/8  The number of bytes for one sample including all channels. LE, 2 bytes
            baos.write(Bytes.toByteArray((short) (channels * format.getBytePerSample()), false));
            // BitsPerSample (8, 16 etc.) LE, 2 bytes
            baos.write(Bytes.toByteArray((short) format.getSampleSizeInBits(), false));
            // Subchunk2
            // SubChunk2ID (String "data") 4 bytes.
            baos.write(Bytes.toByteArray(0x64617461, true));
            // Subchunk2Size    == NumSamples * NumChannels * BitsPerSample/8. This is the number of bytes in the data.
            // You can also think of this as the size of the read of the subchunk following this number. LE, 4 bytes.
            baos.write(Bytes.toByteArray(totalSamplesInByte, false));
            return baos.toByteArray();
        } catch (IOException e) {
            e.printStackTrace();
            return new byte[0];
        } finally {
            IOs.closeSilently(baos);
        }
    }

    public PcmAudioFormat getFormat() {
        return format;
    }

    public int getTotalSamplesInByte() {
        return totalSamplesInByte;
    }

    public int getSampleCount() {
        return totalSamplesInByte / format.getBytePerSample();
    }

    public String toString() {
        return "[ Format: " + format.toString() + " , totalSamplesInByte:" + totalSamplesInByte + "]";
    }
}