using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/10/10/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split("\n");
int numRows = inputStrSplit.Count();
int numCols = inputStrSplit[0].Length;
int[,] mapArray = new int[numRows, numCols];
// int[,] nodeArray = new int[numRows, numCols];

// Dictionary<Char, List<int[]>> freqLocations = new Dictionary<Char, List<int[]>>();


// Convert map into 2d int array
for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		mapArray[i, j] = Int32.Parse("" + inputStrSplit[i][j]);
	}
}

/*
Plan is to cycle through each int in the map array, when a 0 is reached start searching for next steps, 
start by looking up and then rotating clockwise, in a separate array count the number of directions checked (1-4),
after a dead end is reached  go back to the previous step and continue checking directions, maintain a list of step coords 
to be able to backtrack, maintain set of 9 coords reached.
*/

string dir = "up";


// Rotates direction clockwise
string changeDir(string dir)
{
	List<string> dirList = ["up", "right", "down", "left", "up"];
	int ind = dirList.IndexOf(dir);
	return dirList[ind + 1];	
}

(int, int) nextCoords(int x, int y, string dir)
{
	if (dir == "up")
	{
		return (x - 1, y);	
	}
	else if (dir == "right")
	{
		return (x, y + 1);	
	}
	else if (dir == "down")
	{
		return (x + 1, y);	
	}
	else if (dir == "left")
	{
		return (x, y - 1);	
	}
	else
	{
		throw new Exception($"invalid dir = {dir}");
	}
}

// Checks to see if target coordinate is in and contains the next number
bool posContainsNextNumber(int[,] mapArray, int currentNum, int x, int y)
{
	if (x < 0 || y < 0 || x >= mapArray.GetLength(0) || y >= mapArray.GetLength(1))
	{
		return false;
	}
	
	return mapArray[x, y] == currentNum + 1;
}

// Finds number 9s reachable from a starting 0 position
int findReachable9s(int[,] mapArray, int x, int y, bool keepDupes)
{
	List<int[]> reachable9s = [];
	int currentNum = 0;
	string dir = "up";
	Dictionary<int[], string> path = new Dictionary<int[], string>();
	
	// directionsCheckedArray counts the number of directions of each coordinate that have been checked. Init array of 0s
	int[,] directionsCheckedArray = new int[mapArray.GetLength(0), mapArray.GetLength(1)];
	
	// Maintain a list of coords travelled to get to current point, when a 9 or a dead end is reached
	// then back track up the list and keep looking. When the list is empty that means there's nothing more to check
	while (path.Count > 0 || directionsCheckedArray[x, y] < 4)
	{
		// If we have checked all directions of current position then go back along the path
		if (directionsCheckedArray[x, y] >= 4)
		{
			List<int[]> keys = path.Keys.ToList();
			int[] prevPos = keys[keys.Count - 1];
			(x, y) = (prevPos[0], prevPos[1]);
			// We also need to resume with the direction we last used
			dir = path[prevPos];
			path.Remove(prevPos);
			currentNum--;
			dir = changeDir(dir);
			continue;
		}
		
		(int x1, int y1) = nextCoords(x, y, dir);
		directionsCheckedArray[x, y]++;
		if (posContainsNextNumber(mapArray, currentNum, x1, y1))
		{
			path.Add([x, y], dir);
			(x, y) = (x1, y1);
			currentNum ++;
			
			// Now check if the next tile has already been completely explored, if so reset the counter to try again
			if (directionsCheckedArray[x, y] == 4)
			{
				directionsCheckedArray[x, y] = 0;
			}
		}
		else
		{
			dir = changeDir(dir);
		}
		
		// If we landed on a 9 then mark all directions as checked. Then next iteration we'll go back down the path
		if (currentNum == 9)
		{
			directionsCheckedArray[x, y] = 4;
			
			bool alreadyCounted = false;
			foreach (int[] coords in reachable9s)
			{
				if (coords[0] == x && coords[1] == y)
				{
					alreadyCounted = true;
				}	
			}
			
			if (!alreadyCounted || keepDupes)
			{
				reachable9s.Add([x, y]);
			}
		}
	}
	
	return reachable9s.Count;
}


int answer1 = 0;

for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		// Console.WriteLine($"i = {i}, j = {j}");
		if (mapArray[i, j] == 0)
		{
			answer1 += findReachable9s(mapArray, i, j, false);
		}
	}
}

Console.WriteLine("Solution 1 = " + answer1);

/* --------------- Part 2 --------------- */
/* 
Now I need to count the number of unique paths in total. Simple change to my findReachable9s function, now
I just need to reset the directionsChecked counter if coming from a different approach and also don't remove 
duplicate reached 9s (as the overall trail will be different).
*/


int answer2 = 0;

for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		if (mapArray[i, j] == 0)
		{
			answer2 += findReachable9s(mapArray, i, j, true);
		}
	}
}

Console.WriteLine("Solution 2 = " + answer2);