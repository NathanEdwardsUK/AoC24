using System;
using System.IO;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/7/7/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split("\n");
List<List<long>> equationsList = [];

// Populate equationsList, the first element of each list is the answer and the rest are the components
for (int i = 0; i < inputStrSplit.Count(); i++)
{
	string[] equationStr = inputStrSplit[i].Split(" ");
	List<long> longList = [];
	
	foreach (string longStr in equationStr)
	{
		longList.Add(Int64.Parse(longStr.Replace(":", "")));
	}
	
	equationsList.Add(longList);
}

long answer1 = 0;

// Now cycle through each list and try every combination of inserting + or * to see if the answer can be reached
for (int i = 0; i < equationsList.Count(); i++)
{
	List<long> equationComponents = equationsList[i];
	// LHS = left hand side of equation AKA answer, RHS = right hand side AKA components. We are checking for LHS = RHS
	long LHS = equationComponents[0];
	
	/*
	Approach:
	For n equationComponents there are 2^(n-1) possible combinations. Cycle from 0 to 2^(n-1), 
	convert to binary, use 0 to represent + and 1 to represent *, check if the equation works and continue.
	*/
	
	long numCombinations = (long) Math.Pow(2, equationComponents.Count - 2);
	for (int c = 0; c < numCombinations; c++)
	{
		string binaryStr = Convert.ToString(c, 2);
		// Need to fill in leading 0s
		while (binaryStr.Length < equationComponents.Count - 2)
		{
			binaryStr = "0" + binaryStr;
		}
		
		char[] binary = binaryStr.ToCharArray();
		long RHS = equationComponents[1];
		
		for (int j = 0; j < binary.Length; j++)
		{
			if (binary[j] == '0')
			{
				RHS += equationComponents[2 + j];
			}
			else
			{
				RHS *= equationComponents[2 + j];
			}
		}
		
		if (LHS == RHS)
		{
			answer1 += LHS;
			Console.WriteLine($"{i} Found good combination " + string.Join(", ", equationComponents));
			break;
		}
	}
}

Console.WriteLine("Solution 1 = " + answer1);

/* --------------- Part 2 --------------- */
/*
Now we have a 3rd operator || - concatenation. Same approach but working in ternary. 
I could make this much cleaner with a generalised n operator function but im just going to copy paste the above.
Update:
unfortunately C# does not natively support conversion to base 3 so also have to make a function for that
*/

string toTernaryString(long l)
{
	string str = "";
	
	while(l > 0)
	{	
		str = l % 3 + str;
		l /= 3;
	}
	
	return str;
}


long answer2 = 0;

// Exact same method as before but in ternary ( ase 3) with an extra operator to account for
for (int i = 0; i < equationsList.Count(); i++)
{
	List<long> equationComponents = equationsList[i];
	long LHS = equationComponents[0];
	
	long numCombinations = (long) Math.Pow(3, equationComponents.Count - 2);
	for (int c = 0; c < numCombinations; c++)
	{
		string ternaryStr = toTernaryString(c);
		while (ternaryStr.Length < equationComponents.Count - 2)
		{
			ternaryStr = "0" + ternaryStr;
		}
		
		char[] ternary = ternaryStr.ToCharArray();
		long RHS = equationComponents[1];
		
		for (int j = 0; j < ternary.Length; j++)
		{
			if (ternary[j] == '0')
			{
				RHS += equationComponents[2 + j];
			}
			else if (ternary[j] == '1')
			{
				RHS *= equationComponents[2 + j];
			}
			else
			{
				string strRHS = "" + RHS + equationComponents[2 + j];
				RHS = Int64.Parse(strRHS);
			}
		}
		
		if (LHS == RHS)
		{
			answer2 += LHS;
			Console.WriteLine($"{i} Found good combination " + string.Join(", ", equationComponents));
			break;
		}
	}
}

Console.WriteLine("Solution 2 = " + answer2);