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

class Bytes {
    /**
     * converts an integer array to a byte array. this is useful when defining byte arrays in the
     * code without using
     *
     * (byte) casts.
     *
     * @param intz an integer array.
     * @return a byte array.
     */
    public static byte[] toByteArray(int... intz) {
        byte[] bytez = new byte[intz.length];
        for (int i = 0; i < intz.length; i++) {
            bytez[i] = (byte) (intz[i] & 0xff);
        }
        return bytez;
    }

    /**
     * converts a byte array to an integer. byte array may be with the lenght of 1,2,3 or 4.
     *
     * @param pb        byte array
     * @param bigEndian endianness.
     * @return an integer represented byt the byte array
     * @throws IllegalArgumentException if byte array size is larger than 4
     */
    public static int toInt(byte[] pb, boolean bigEndian) {
        switch (pb.length) {
            case 1:
                return pb[0] & 0xff;
            case 2:
                if (bigEndian)
                    return (pb[0] << 8 & 0xff00) | (pb[1] & 0xff);
                else
                    return (pb[1] << 8 & 0xff00) | (pb[0] & 0xff);
            case 3:
                if (bigEndian)
                    return (pb[0] << 16 & 0xff0000) | (pb[1] << 8 & 0xff00) | (pb[2] & 0xff);
                else
                    return (pb[2] << 16 & 0xff0000) | (pb[1] << 8 & 0xff00) | (pb[0] & 0xff);
            case 4:
                if (bigEndian) {
                    return (pb[0] << 24 & 0xff000000) |
                            (pb[1] << 16 & 0xff0000) |
                            (pb[2] << 8 & 0xff00) |
                            (pb[3] & 0xff);
                } else {
                    return (pb[3] << 24 & 0xff000000) |
                            (pb[2] << 16 & 0xff0000) |
                            (pb[1] << 8 & 0xff00) |
                            (pb[0] & 0xff);
                }
            default:
                throw new IllegalArgumentException("1,2,3 or 4 byte arrays allowed. size:" + pb.length);
        }
    }

    public static byte[] toByteArray(int i, int size, boolean bigEndian) {
        switch (size) {
            case 1:
                return new byte[]{(byte) i};
            case 2:
                if (bigEndian)
                    return new byte[]{(byte) (i >>> 8 & 0xff), (byte) (i & 0xff)};
                else
                    return new byte[]{(byte) (i & 0xff), (byte) (i >>> 8 & 0xff)};
            case 3:
                if (bigEndian)
                    return new byte[]{(byte) (i >>> 16 & 0xff), (byte) (i >>> 8 & 0xff), (byte) (i & 0xff)};
                else
                    return new byte[]{(byte) (i & 0xff), (byte) (i >>> 8 & 0xff), (byte) (i >>> 16 & 0xff)};
            case 4:
                return toByteArray(i, bigEndian);
            default:
                throw new IllegalArgumentException("1,2,3 or 4 size values are allowed. size:" + size);
        }
    }

    /**
     * converts 4 bytes to an integer
     *
     * @param b0        first byte
     * @param b1        second byte
     * @param b2        third byte
     * @param b3        forth byte
     * @param bigEndian , if we want it in big endian format
     * @return integer formed from bytes.
     */
    public static int toInt(byte b0, byte b1, byte b2, byte b3, boolean bigEndian) {
        if (bigEndian) {
            return (b0 << 24 & 0xff000000) |
                    (b1 << 16 & 0xff0000) |
                    (b2 << 8 & 0xff00) |
                    (b3 & 0xff);
        } else {
            return (b3 << 24 & 0xff000000) |
                    (b2 << 16 & 0xff0000) |
                    (b1 << 8 & 0xff00) |
                    (b0 & 0xff);
        }
    }

    /**
     * converts an integer to 4 byte array.
     *
     * @param i         the number.
     * @param bigEndian endianness.
     * @return byte array generated from the integer.
     */
    public static byte[] toByteArray(int i, boolean bigEndian) {
        byte[] ba = new byte[4];
        if (bigEndian) {
            ba[0] = (byte) (i >>> 24);
            ba[1] = (byte) (i >>> 16 & 0xff);
            ba[2] = (byte) (i >>> 8 & 0xff);
            ba[3] = (byte) (i & 0xff);
        } else {
            ba[0] = (byte) (i & 0xff);
            ba[1] = (byte) (i >>> 8 & 0xff);
            ba[2] = (byte) (i >>> 16 & 0xff);
            ba[3] = (byte) (i >>> 24);
        }
        return ba;
    }

    /**
     * converts a short to 2 byte array.
     *
     * @param i         the number.
     * @param bigEndian endianness.
     * @return byte array generated from the short.
     */
    public static byte[] toByteArray(short i, boolean bigEndian) {
        byte[] ba = new byte[2];
        if (bigEndian) {
            ba[0] = (byte) (i >>> 8);
            ba[1] = (byte) (i & 0xff);
        } else {
            ba[0] = (byte) (i & 0xff);
            ba[1] = (byte) (i >>> 8 & 0xff);
        }
        return ba;
    }

    /**
     * Converts a byte array to an integer array. byte array length must be an order of 4.
     *
     * @param ba        byte array
     * @param amount    amount of bytes to convert to int.
     * @param bigEndian true if big endian.
     * @return an integer array formed form byte array.
     * @throws IllegalArgumentException if amount is smaller than 4, larger than byte array, or not
     *                                  an order of 4.
     */
    public static int[] toIntArray(byte[] ba, int amount, boolean bigEndian) {
        final int size = determineSize(amount, ba.length, 4);
        int[] result = new int[size / 4];
        int i = 0;
        for (int j = 0; j < size; j += 4) {
            if (bigEndian) {
                result[i++] = toInt(ba[j], ba[j + 1], ba[j + 2], ba[j + 3], true);
            } else {
                result[i++] = toInt(ba[j + 3], ba[j + 2], ba[j + 1], ba[j], true);
            }
        }
        return result;
    }

    /**
     * Converts a byte array to an integer array. byte array length must be an order of 4.
     *
     * @param ba             byte array
     * @param amount         amount of bytes to convert to int.
     * @param bytePerInteger byte count per integer.
     * @param bigEndian      true if big endian.
     * @return an integer array formed form byte array.
     * @throws IllegalArgumentException if amount is smaller than 4, larger than byte array, or not
     *                                  an order of 4.
     */

    public static int[] toIntArray(byte[] ba, final int amount, final int bytePerInteger, boolean bigEndian) {
        final int size = determineSize(amount, ba.length, bytePerInteger);
        int[] result = new int[size / bytePerInteger];
        int i = 0;
        byte[] bytez = new byte[bytePerInteger];
        for (int j = 0; j < size; j += bytePerInteger) {
            System.arraycopy(ba, j, bytez, 0, bytePerInteger);
            if (bigEndian) {
                result[i++] = toInt(bytez, true);
            } else {
                result[i++] = toInt(bytez, false);
            }
        }
        return result;
    }

    private static int determineSize(int amount, int arrayLength, int order) {
        if (amount < order || amount > arrayLength)
            throw new IllegalArgumentException(
                    "amount of bytes to read cannot be smaller than " + order +
                            " or larger than array length. Amount is:" + amount);
        final int size = amount < arrayLength ? amount : arrayLength;
        if (size % order != 0)
            throw new IllegalArgumentException("array size must be an order of " + order + ". The size is:" + arrayLength);
        return size;
    }

    /**
     * Converts a byte array to a short array. byte array length must be an order of 2.
     *
     * @param ba        byte array
     * @param amount    amount of bytes to convert to short.
     * @param bigEndian true if big endian.
     * @return a short array formed from byte array.
     * @throws IllegalArgumentException if amount is smaller than 2, larger than byte array, or not
     *                                  an order of 2.
     */
    public static short[] toShortArray(byte[] ba, int amount, boolean bigEndian) {
        final int size = determineSize(amount, ba.length, 2);
        short[] result = new short[size / 2];
        int i = 0;
        for (int j = 0; j < size; j += 2) {
            if (bigEndian) {
                result[i++] = (short) (ba[j] << 8 & 0xff00 | ba[j + 1] & 0xff);
            } else {
                result[i++] = (short) (ba[j + 1] << 8 & 0xff00 | ba[j] & 0xff);
            }
        }
        return result;
    }

    /**
     * Converts a given array of shorts to a byte array.
     *
     * @param sa        short array
     * @param amount    amount of data to convert from input array
     * @param bigEndian if it is big endian
     * @return an array of bytes converted from the input array of shorts.
     *
     * 0xBABE becomes 0xBA, 0xBE (Big Endian) or 0xBE, 0xBA (Little Endian)
     */
    public static byte[] toByteArray(short[] sa, int amount, boolean bigEndian) {
        final int size = amount < sa.length ? amount : sa.length;
        byte[] result = new byte[size * 2];
        for (int j = 0; j < size; j++) {
            final byte bh = (byte) (sa[j] >>> 8);
            final byte bl = (byte) (sa[j] & 0xff);
            if (bigEndian) {
                result[j * 2] = bh;
                result[j * 2 + 1] = bl;
            } else {
                result[j * 2] = bl;
                result[j * 2 + 1] = bh;
            }
        }
        return result;
    }

    /**
     * Converts a given array of ints to a byte array.
     *
     * @param ia             <code>int</code> array
     * @param amount         Amount of data to be converted from input array
     * @param bytePerInteger Byte count per integer.
     * @param bigEndian      If it is big endian
     * @return an array of bytes converted from the input array of shorts.
     * when bytePerInteger = 2,  ia = {0x0000CAFE, 0x0000BABE}
     * returns {0xCA, 0xFE, 0xBA, 0xBE} (Big Endian)
     * returns {0xFE, 0xCA, 0xBE, 0xBA } (Little Endian)
     * when bytePerInteger=4, ia = {0xCAFEBABE}
     * return  {0xCA, 0xFE, 0xBA, 0xBE} (Big Endian)
     * returns { 0xBE, 0xBA, 0xFE, 0xCA} (Little Endian)
     */
    public static byte[] toByteArray(int[] ia, int amount, int bytePerInteger, boolean bigEndian) {
        if (bytePerInteger < 1 || bytePerInteger > 4)
            throw new IllegalArgumentException("bytePerInteger parameter can only be 1,2,3 or 4. But it is:" + bytePerInteger);
        if (amount > ia.length || amount < 0)
            throw new IllegalArgumentException("Amount cannot be negative or more than input array length. Amount:" + amount);
        final int size = amount < ia.length ? amount : ia.length;
        byte[] result = new byte[size * bytePerInteger];
        for (int j = 0; j < size; j++) {
            final byte[] piece = toByteArray(ia[j], bytePerInteger, bigEndian);
            System.arraycopy(piece, 0, result, j * bytePerInteger, bytePerInteger);
        }
        return result;
    }
}