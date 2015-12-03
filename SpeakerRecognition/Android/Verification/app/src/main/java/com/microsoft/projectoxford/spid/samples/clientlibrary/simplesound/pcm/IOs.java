package com.microsoft.projectoxford.spid.samples.clientlibrary.simplesound.pcm;

import java.io.*;

class IOs {
    /**
     * closes the <code>closeables</code> silently, meaning that if the Closeable is null,
     * or if it throws an exception during close() operation it only creates a system error output,
     * does not throw an exception.
     * this is especially useful when you need to close one or more resources in finally blocks.
     * This method should only be called in finalize{} blocks or wherever it really makes sense.
     *
     * @param closeables zero or more closeable.
     */
    public static void closeSilently(Closeable... closeables) {
        // if closeables is null, return silently.
        if (closeables == null) return;
        for (Closeable closeable : closeables) {
            try {
                if (closeable != null)
                    closeable.close();
            } catch (IOException e) {
                System.err.println("IO Exception during closing stream (" + closeable + ")." + e);
            }
        }
    }

    /**
     * converts an input stream data to byte array. careful with memory usage here.
     *
     * @param is , an input stream
     * @return a byte array representing the stream data.
     * @throws IOException          if an error occurs during the read or write of the streams.
     * @throws NullPointerException if input stream is null
     */
    public static byte[] readAsByteArray(InputStream is) throws IOException {
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        try {
            if (is == null)
                throw new NullPointerException("input stream cannot be null.");
            int b;
            byte[] buffer = new byte[4096];
            while ((b = is.read(buffer, 0, buffer.length)) != -1) {
                baos.write(buffer, 0, b);
            }
            return baos.toByteArray();
        } finally {
            closeSilently(is, baos);
        }
    }

    /**
     * Copies oan input stream content to an output stream.
     *
     * Once the copy is finished streams will be closed.
     *
     * @param is input stream
     * @param os output stream
     * @throws java.io.IOException if an IO error occurs.
     */

    public static void copy(InputStream is, OutputStream os) throws IOException {
        copy(is, os, false);
    }

    /**
     * Copies oan input stream content to an output stream.
     * Once the copy is finished only the input strean is closed by default. Closing of the
     * output stream depends on the boolean parameter..
     *
     * @param is             input stream
     * @param os             output stream
     * @param keepOutputOpen if true, output stream will not be closed.
     * @throws java.io.IOException if an IO error occurs.
     */
    static void copy(InputStream is, OutputStream os, boolean keepOutputOpen) throws IOException {
        try {
            byte[] buf = new byte[4096];
            int i;
            while ((i = is.read(buf)) != -1)
                os.write(buf, 0, i);
        } finally {
            closeSilently(is);
            if (!keepOutputOpen)
                closeSilently(os);
        }
    }
}