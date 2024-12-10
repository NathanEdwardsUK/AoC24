using System;
using System.IO;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/6/6/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split("\n");
int numRows = inputStrSplit.Count();
int numCols = inputStrSplit[0].Length;
Char[,] mapArray = new Char[numRows, numCols];

// guardCoords are the guards current position. GuardPosition contains all his predicted movements.
int[] guardCoords = new int[2];
int[] initialGuardCoords = new int[2];
string dir = "up";
int[,] guardPositions = new int[numRows, numCols];

// Convert map into 2d int array and gets guards current pos
for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		mapArray[i, j] = inputStrSplit[i][j];
		if (mapArray[i, j] == '^')
		{
			guardCoords = [i, j];
			// used for part 2
			initialGuardCoords = [i, j];
		}
	}
}

// Changes direction 90 degrees clockwise
string changeDirection(string dir)
{
	List<string> dirList = ["up", "right", "down", "left", "up"];
	int ind = dirList.IndexOf(dir);
	return dirList[ind + 1];	
}

bool checkCoordsInsideMap(Char[,] mapArray, int[] coords)
{
	bool inMap = (coords[0] >= 0 && coords[0] < mapArray.GetLength(0) 
				&& coords[1] >= 0 && coords[1] < mapArray.GetLength(1));
	return inMap;
}

bool checkForObstacle(Char[,] mapArray, int[] coords)
{
	return mapArray[coords[0], coords[1]] == '#';
}

int[] getNextCoords(int[] coords, string dir)
{
	if (dir == "up")
	{
		return [coords[0] - 1, coords[1]];
	}
	else if (dir == "right")
	{
		return [coords[0], coords[1] + 1];
	}
	else if (dir == "down")
	{
		return [coords[0] + 1, coords[1]];
	}
	else if (dir == "left")
	{
		return [coords[0], coords[1] - 1];
	}
	throw new Exception("Invalid dir given. dir = " + dir);
}

// Now handle guard movement and fill out array showing which coordinates he passes through
while (true)
{
	guardPositions[guardCoords[0], guardCoords[1]] = 1;
	int[] targetCoords = getNextCoords(guardCoords, dir);
	
	if (!checkCoordsInsideMap(mapArray, targetCoords))
	{
		break;
	}
	
	if (checkForObstacle(mapArray, targetCoords))
	{
		dir = changeDirection(dir);
	}
	else
	{
		guardCoords = targetCoords;
	}
}

int answer1 = guardPositions.Cast<int>().Sum();
Console.WriteLine("Solution 1 = " + answer1);

/* --------------- Part 2 --------------- */
/* 
My approach is the cycle through all non obstacle positions of the mapArray and check whether a loop can be formed.
I will check this by maintaining a list of (position, direction) tuples that the guard travels 
and breaking when a duplicate is found
*/

int convertDirToInt(string dir)
{
	List<string> dirList = ["up", "right", "down", "left"];
	return dirList.IndexOf(dir);
}

int answer2 = 0;

// Cycle through every position
for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numRows; j++)
	{
		int[] obstacleCoords = [i, j];
		
		// The new obstacle cannot be on an existing obstacle or the guards starting position
		if (Enumerable.SequenceEqual(obstacleCoords, initialGuardCoords) || checkForObstacle(mapArray, obstacleCoords))
		{
			continue;
		}
		
		// The thirds dimension of this array represents the direction of the guard
		Array.Copy(initialGuardCoords, guardCoords, 2);
		dir = "up";
		int[,,] guardVectors = new int[numRows, numCols, 4];
		
		// Now handle guard movement again but check for duplicates with the guardVectors variable
		while (true)
		{
			// Checks if we have already seen the guard in this position with this direction
			if (guardVectors[guardCoords[0], guardCoords[1], convertDirToInt(dir)] == 0)
			{
				guardVectors[guardCoords[0], guardCoords[1], convertDirToInt(dir)] = 1;
			}
			// If so then we have found one more potential loop scenario, so check the next one
			else
			{
				Console.WriteLine($"Loop found i = {i} j = {j}");
				answer2++;
				break;
			}

			int[] targetCoords = getNextCoords(guardCoords, dir);
			
			if (!checkCoordsInsideMap(mapArray, targetCoords))
			{
				break;
			}
			
			if (checkForObstacle(mapArray, targetCoords) || Enumerable.SequenceEqual(targetCoords, obstacleCoords))
			{
				dir = changeDirection(dir);
			}
			else
			{
				guardCoords = targetCoords;
			}
		}
	}
}

Console.WriteLine("Solution 2 = " + answer2);