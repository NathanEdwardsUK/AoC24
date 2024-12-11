using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/11/11/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split(" ");
List<long> stonesList = [];

foreach(string str in inputStrSplit)
{
	stonesList.Add(Int64.Parse(str));
}

Console.WriteLine("Starting list of numbers on stones = " + String.Join(" ", stonesList));

/*
For this one we have numbers on stones which change (sometimes splitting) with every blink (iteration) according to 3 rules.
We have 8 starting stones and 25 blinks to do, the number of stones increase less than exponentially but still very fast.
I am going to try to solve this recursively to find the final number of stones, it's not not certain that this method will
be efficient enough. Should be better than keeping all stones in memory at once though.

--------- Update ---------
The approach was efficient enough for part 1 but not part 2 which simply increases blinks to 25. Observing that many (hoping most) 
numbers that result from applying the rules are < 100, So I will start caching results in a hashtable and doing a check in there.
*/


Hashtable[] hashTableArray = new Hashtable[100]; 

for (int i = 0; i < 100; i++){
	hashTableArray[i] = new Hashtable();
}

long checkIfInHashTable(long n, int blinks)
{
	if (hashTableArray[blinks].ContainsKey(n))
	{
		return (long) hashTableArray[blinks][n];
	}
	else 
	{
		return 0;
	}
	
}

long applyRules(long n, int blinks)
{	

	if (blinks == 0)
	{
		// Console.WriteLine($"Out = {n}");
		return 1;
	}
	
	long cachedValue = checkIfInHashTable(n, blinks);
	if (cachedValue != 0)
	{
		return cachedValue;
	}
	
	string nStr = "" + n;
	int nStrLen = nStr.Length;
	long result;
	
	if (n == 0)
	{
		result = applyRules(1, blinks - 1);
		
	}
	else if (nStr.Length % 2 == 0)
	{
		long n1 = Int64.Parse(nStr.Substring(0, nStrLen / 2));
		long n2 = Int64.Parse(nStr.Substring(nStrLen / 2, nStrLen / 2));
		result = applyRules(n1, blinks - 1) + applyRules(n2, blinks - 1);
	}
	else 
	{
		result = applyRules(n * 2024, blinks - 1);
	}
	
	hashTableArray[blinks].Add(n, result);
	return result;
}

long totalStones = 0;
foreach (long stone in stonesList)
{
	Console.WriteLine($"Checking stone {stone}");
	totalStones += applyRules(stone, 75);
}


Console.WriteLine("Solution 1 = " + totalStones);