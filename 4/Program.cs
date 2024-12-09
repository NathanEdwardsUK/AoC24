using System;
using System.IO;
using System.Text.RegularExpressions;

StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/4/input.txt");

string wordSearchStr = sr.ReadToEnd();
List<string> stringsList = [];

string[] rows = wordSearchStr.Split("\n");
stringsList.AddRange(rows);

List<string> getColumns(string[] rows)
{
	int numCols = rows[0].Length;
	int numRows = rows.Length;
	List<string> columns = [];
	
	for (int c = 0; c < numCols; c++)
	{
		string str = "";
		for (int r = 0; r < numRows; r++)
		{
			str += rows[r].ElementAt(c);
		}

		columns.Add(str);
	}
	return columns;
}

List<string> getDiagonals(string[] rows)
{
	int numCols = rows[0].Length;
	int numRows = rows.Length;
	List<string> diagonals = [];
	
	// First get all the diagonal strings that contain a letter in the top row
	for (int c = 0; c < numCols; c++)
	{
		int _c = c;
		int _r = 0;
		string diagStr1 = "";
		string diagStr2 = "";
		
		
		while (_c < numCols && _r < numRows)
		{
			diagStr1 += rows[_r].ElementAt(_c);
			diagStr2 += rows[_r].ElementAt(numCols - _c - 1);
			_r ++;
			_c ++;
		}
		diagonals.Add(diagStr1);
		diagonals.Add(diagStr2);
	}
	
	// Then get the diagonal strings than contain a letter in the first column.
	// Exclude r = 0 because those diagonals contains a letter in the top row and so are already counted.
	for (int r = 1; r < numRows; r++)
		{
			int _c = 0;
			int _r = r;
			string diagStr1 = "";
			string diagStr2 = "";
			
			while (_c < numCols && _r < numRows)
			{
				diagStr1 += rows[_r].ElementAt(_c);
				diagStr2 += rows[_r].ElementAt(numCols - _c - 1);
				_r ++;
				_c ++;
			}
			diagonals.Add(diagStr1);
			diagonals.Add(diagStr2);
		}
	return diagonals;
}

List<string> getReversedLines(List<string> lines)
{
	List<string> newLines = new List<string>(lines);
	
	foreach (string line in lines)
	{
		Char[] charArray = line.ToCharArray();
		Array.Reverse(charArray);
		newLines.Add(new string(charArray));
	}
	
	return newLines;
}

List<string> columns = getColumns(rows);
stringsList.AddRange(columns);
List<string> diagonals = getDiagonals(rows);
stringsList.AddRange(diagonals);
stringsList = getReversedLines(stringsList);

int totalMatches1 = 0;

for (int i = 0; i < stringsList.Count(); i++)
{
	string str = stringsList[i];
	int matches = Regex.Matches(str, "(XMAS)").Count();
	
	if (matches > 0)
	{
		Console.Write($"{i} - matches found {matches}\n{str}\n");
	}
	
	totalMatches1 += matches;
}

Console.WriteLine($"Solution 1 = {totalMatches1}");

/* --------------- Part 2 --------------- */

char[,] strToArray(string str)
{
	string[] rows = str.Split("\n");
	int numCols = rows[0].Length;
	int numRows = rows.Length;
	
	Char[,] charArray = new Char[rows[0].Length, rows.Length];
	
	for (int c = 0; c < numCols; c++)
	{
		for (int r = 0; r < numRows; r++)
		{
			charArray[r, c] = rows[r].ElementAt(c);
		}
	}
	return charArray;
}

Char[,] wordSearchArray = strToArray(wordSearchStr);
int numCols = wordSearchArray.GetLength(1);
int numRows = wordSearchArray.GetLength(0);
List<string> stringsList2;
int totalMatches2 = 0;

// The plan is to cycle through every letter in the word search and select the 3x3 array of letters defined by this
for (int c = 0; c < numCols - 2; c++)
{
	for (int r = 0; r < numRows - 2; r++)
	{
		// Then check if the diagonals of that array are both MAS or SAM
		stringsList2 = [];
		string line1 = "" + wordSearchArray[r, c] + wordSearchArray[r + 1, c + 1] + wordSearchArray[r + 2, c + 2];
		string line2 = "" + wordSearchArray[r + 2, c] + wordSearchArray[r + 1, c + 1] + wordSearchArray[r, c + 2];
		
		if ((line1 == "MAS" || line1 == "SAM") && (line2 == "MAS" || line2 == "SAM"))
		{
			totalMatches2++;
		}
	}
}

Console.WriteLine($"Solution 2 = {totalMatches2}");