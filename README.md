# TF-IDF implementation in C#
TF*IDF = Term Frequency * Inverse Document Frequency

TFIDF is a way of representing a document, based upon its keywords holding values that represent their importance within the document. For a complete description of TFIDF, see http://en.wikipedia.org/wiki/Tf%E2%80%93idf


## Getting Started

you can find the solution in this repository.

2 compilation products:
 * TFIDF.dll
 * Cache.dll

### Description
Cache.dll

in this library you can use cache mechanism and select the caching algorithm at runtime.

API:
	
	V GetElement(K key);
	V PutElement(K key, V value);
	void RemoveElement(K key);


TFIDF.dll

A library to calculate TF, IDF and TFIDF of a term in a document within a corpus

You can calculate TF, IDF & TFIDF directly by accessing the file system on each calculation
by using the following static API:

	Public static double CalculateTF(string path, string filename, string term)
	Public static double CalculateIDF(string path, string term)
	Public static double CalculateTFIDF(string path, string filename, string term)

Or use the Cache mechanism by using the following API:

	Public double CacheCalculateTF(string path, string filename, string term)
	Public double CacheCalculateIDF(string path, string term)
	Public double CacheCalculateTFIDF(string path, string filename, string term)


## Class Diagram
![alt text](https://github.com/davidbinyamin/TFIDF/blob/master/ClassDiagram.png)

### Open Issues
	* Add stream abstraction instead of using filesystem implicitly
	* Implement unit tests for RandomReplacementAlgorithmCache
	* Add Logging mechanism
	* Consider removing stopwords while calculating TF-IDF
	* In case of removing stopwords add localization support

### Prerequisites
Use Visual Studio 2019 to load the solution. with .Net Framework 4.7.2and above


## Running the tests

Use Visual Studio 2019 to load the solution. with .Net Framework 4.7.2and above

Use the command line program to try the TFIDF Library.
"TFIIDF_TestProgram"

## Authors

* **David Binyamin** - *Work in progress* - [davidbinyamin](https://github.com/davidbinyamin)


## License

Copyright (c) 2019 David BInyamin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

