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

import java.util.Iterator;

public class FrameReader implements Iterable<DoubleFrame> {
    private final PcmAudioInputStream pcmAudioInputStream;
    private final int frameSize;
    private final int shiftAmount;

    public FrameReader(PcmAudioInputStream pais, int frameSize, int shiftAmount) {
        this.pcmAudioInputStream = pais;
        this.frameSize = frameSize;
        this.shiftAmount = shiftAmount;
    }

    public Iterator<DoubleFrame> iterator() {
        return new FrameIterator();
    }

    private class FrameIterator implements Iterator<DoubleFrame> {
        public boolean hasNext() {
            return false;
        }

        public DoubleFrame next() {
            return null;
        }

        public void remove() {
        }
    }
}