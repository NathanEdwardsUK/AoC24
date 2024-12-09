using System;
using System.IO;
using System.Text.RegularExpressions;

StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/3/3/input.txt");

// Split up the regex pattern for readability and functionality
string doPattern = @"(do\(\))";
string dontPattern = @"(don\'t\(\))";
string mulPattern = @"(mul\(\d+\,\d+\))";
string regexPattern = doPattern + "|" + dontPattern + "|" + mulPattern;

string line;
int x;
int y;
int result1 = 0;
int result2 = 0;
bool doActive = true;

while ((line = sr.ReadLine()) != null)
{
	string[] splitString = Regex.Split(line, regexPattern);
	foreach (string str in splitString)
	{
		if (Regex.Match(str, regexPattern).Success)
		{
			if (Regex.Match(str, doPattern).Success)
			{
				doActive = true;
			}
			else if (Regex.Match(str, dontPattern).Success)
			{
				doActive = false;
			}
			else
			{
				string intStr = str.Replace("mul(","").Replace(")","");
				string[] splitIntStr = intStr.Split(",");
				x = Int32.Parse(splitIntStr[0]);
				y = Int32.Parse(splitIntStr[1]);
				
				result1 += x * y;
				if (doActive)
				{
					result2 += x * y;
				}
				Console.Write($"mul({x},{y}) - doActive = {doActive}\n");
			}
			
		}
	}
}

Console.Write($"Solution 1 = {result1}\n");
Console.Write($"Solution 2 = {result2}\n");
