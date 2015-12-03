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

/**
 * Represents paramters for raw pcm audio sample data.
 *
 * Channels represents mono or stereo data. mono=1, stereo=2
 */
public class PcmAudioFormat {
    /**
     * Sample frequency in sample/sec.
     */
    private final int sampleRate;
    /**
     * the amount of bits representing samples.
     */
    private final int sampleSizeInBits;
    /**
     * How many bytes are required for representing samples
     */
    private final int bytesRequiredPerSample;
    /**
     * channels. For now only 1 or two channels are allowed.
     */
    private final int channels;
    /**
     * if data is represented as big endian or little endian.
     */
    protected final boolean bigEndian;
    /**
     * if data is signed or unsigned.
     */
    private final boolean signed;

    protected PcmAudioFormat(int sampleRate, int sampleSizeInBits, int channels, boolean bigEndian, boolean signed) {
        if (sampleRate < 1)
            throw new IllegalArgumentException("sampleRate cannot be less than one. But it is:" + sampleRate);
        this.sampleRate = sampleRate;
        if (sampleSizeInBits < 2 || sampleSizeInBits > 31) {
            throw new IllegalArgumentException("sampleSizeInBits must be between (including) 2-31. But it is:" + sampleSizeInBits);
        }
        this.sampleSizeInBits = sampleSizeInBits;
        if (channels < 1 || channels > 2) {
            throw new IllegalArgumentException("channels must be 1 or 2. But it is:" + channels);
        }
        this.channels = channels;
        this.bigEndian = bigEndian;
        this.signed = signed;
        if (sampleSizeInBits % 8 == 0)
            bytesRequiredPerSample = sampleSizeInBits / 8;
        else
            bytesRequiredPerSample = sampleSizeInBits / 8 + 1;
    }

    /**
     * This is a builder class. By default it generates little endian, mono, signed, 16 bits per
     * sample.
     */
    public static class Builder {
        private int _sampleRate;
        private int _sampleSizeInBits = 16;
        private int _channels = 1;
        private boolean _bigEndian = false;
        private boolean _signed = true;

        public Builder(int sampleRate) {
            this._sampleRate = sampleRate;
        }

        public Builder channels(int channels) {
            this._channels = channels;
            return this;
        }

        public Builder bigEndian() {
            this._bigEndian = true;
            return this;
        }

        public Builder unsigned() {
            this._signed = false;
            return this;
        }

        public Builder sampleSizeInBits(int sampleSizeInBits) {
            this._sampleSizeInBits = sampleSizeInBits;
            return this;
        }

        public PcmAudioFormat build() {
            return new PcmAudioFormat(_sampleRate, _sampleSizeInBits, _channels, _bigEndian, _signed);
        }
    }

    PcmAudioFormat mono16BitSignedLittleEndian(int sampleRate) {
        return new PcmAudioFormat(sampleRate, 16, 1, false, true);
    }

    public int getSampleRate() {
        return sampleRate;
    }

    public int getChannels() {
        return channels;
    }

    public int getSampleSizeInBits() {
        return sampleSizeInBits;
    }

    /**
     * returns the required bytes for the sample bit size. Such that, if 4 or 8 bit samples are
     * used.
     *
     * it returns 1, if 12 bit used 2 returns.
     *
     * @return required byte amount for the sample size in bits.
     */

    public int getBytePerSample() {
        return bytesRequiredPerSample;
    }

    public boolean isBigEndian() {
        return bigEndian;
    }

    /**
     * finds the byte location of a given time. if time is negative, exception is thrown.
     *
     * @param second second information
     * @return the byte location in the samples.
     */
    public int calculateSampleIndex(double second) {
        if (second < 0)
            throw new IllegalArgumentException("Time information cannot be negative.");
        int loc = (int) (second * sampleRate * bytesRequiredPerSample);
        if (loc % bytesRequiredPerSample != 0) {
            loc += (bytesRequiredPerSample - loc % bytesRequiredPerSample);
        }
        return loc;
    }

    /**
     * calcualates the time informationn for a given sample.
     *
     * @param sampleIndex sample index.
     * @return approximate seconds information for the given sample.
     */

    public double calculateSampleTime(int sampleIndex) {
        if (sampleIndex < 0)
            throw new IllegalArgumentException("sampleIndex information cannot be negative:" + sampleIndex);
        return (double) sampleIndex / sampleRate;
    }

    public boolean isSigned() {
        return signed;
    }

    public String toString() {
        return "[ Sample Rate:" + sampleRate + " , SampleSizeInBits:" + sampleSizeInBits +
                ", channels:" + channels + ", signed:" + signed + ", bigEndian:" + bigEndian + " ]";
    }
}