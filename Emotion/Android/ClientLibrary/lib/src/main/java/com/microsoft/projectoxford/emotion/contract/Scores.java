//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
package com.microsoft.projectoxford.emotion.contract;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

public class Scores {
    public double anger;
    public double contempt;
    public double disgust;
    public double fear;
    public double happiness;
    public double neutral;
    public double sadness;
    public double surprise;
    
    public List<Map.Entry<String, Double>> ToRankedList(Order order)
    {
		
	// create a Map to store each entry
	Map<String, Double> collection = new HashMap<String, Double>() ;
		
	// add each entry with its own key and value
	collection.put("ANGER",anger);
	collection.put("CONTEMPT",contempt);
	collection.put("DISGUST",disgust);
	collection.put("FEAR",fear);
	collection.put("HAPPINESS",happiness);
	collection.put("NEUTRAL",neutral);
	collection.put("SADNESS",sadness);
	collection.put("SURPRISE",surprise);

	// create a list with the entries
	List<Map.Entry<String, Double>> list = new ArrayList<Map.Entry<String, Double>>(collection.entrySet());
		
	// we are going to create a comparator according to the value of the enum order
	switch (order) 
	{
		case ASCENDING:
			Collections.sort(list, new Comparator<Map.Entry<String, Double>>() {
				@Override
			        public int compare(Entry<String, Double> first, Entry<String, Double> second) {
			        	// we should compare the value of the first entry and the value of the second entry
			            return first.getValue().compareTo(second.getValue());
			        }
			});
			break;
			
		case DESCENDING:
			// for ordering descending we should create a reverse order comparator 
			Collections.sort(list, Collections.reverseOrder(new Comparator<Map.Entry<String, Double>>() {
			        @Override
			        public int compare(Entry<String, Double> first, Entry<String, Double> second) {
			            return first.getValue().compareTo(second.getValue());
			        }
			})); 
			break;
				
		default:
			break;
	}

        return list;
        
    }
}
