using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

DateTime t0 = DateTime.Now;

/* 
--------------- Part 1 --------------- 
*/

char[,] stringToCharArray(string str)
{
	string[] splitStr = str.Split("\n");
	int numRows = splitStr.Count();
	int numCols = splitStr[0].Length;
	char[,] array = new char[numRows, numCols];

	// Convert map into 2d char array
	for (int i = 0; i < numRows; i++)
	{
		for (int j = 0; j < numCols; j++)
		{
			array[i, j] = splitStr[i][j];
		}
	}
	return array;
}

void printMap(char[,] _map)
{
	for (int i = 0; i < _map.GetLength(0); i++)
	{
		for (int j = 0; j < _map.GetLength(1); j++)
		{
			Console.Write(_map[i, j]);
		}
		Console.Write("\n");
	}
}

int[] addPos(int[] pos1, int[] pos2) => [pos1[0] + pos2[0], pos1[1] + pos2[1]];

int[] nextDir(int[] dir)
{
	if (Enumerable.SequenceEqual(dir, [-1, 0])) return [0, 1];
	else if (Enumerable.SequenceEqual(dir, [0, 1])) return [1, 0];
	else if (Enumerable.SequenceEqual(dir, [1, 0])) return [0, -1];
	else if (Enumerable.SequenceEqual(dir, [0, -1])) return [-1, 0];
	else throw new Exception("invalid dir");
}

int[] reverseDir(int[] dir) => nextDir(nextDir(dir));

bool checkInBounds(char[,] _map, int[] pos) => (pos[0] >= 0 && pos[0] < _map.GetLength(0)
											&& pos[1] >= 0 && pos[1] < _map.GetLength(1));

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/20/20/input.txt");
string[] strArr = sr.ReadToEnd().Split("\n\n");
char[,] map = stringToCharArray(strArr[0]);
printMap(map);

List<int[]> findEmptyNeighbours(char[,] _map, int[] _pos)
{
	List<int[]> emptyNeighbours = [];
	int dirCount = 0;
	int[] dir = [-1, 0];
	
	while (dirCount < 4)
	{
		int[] nextPos = addPos(_pos, dir);
		if (checkInBounds(_map, nextPos) && _map[nextPos[0], nextPos[1]] != '#')
		{
			emptyNeighbours.Add(nextPos);
		}
		
		dir = nextDir(dir);
		dirCount++;
	}	
	
	return emptyNeighbours;
}

int[] getNodeFromCh(char[,] _map, char ch)
{
	for (int i = 0; i < _map.GetLength(0); i++)
	{
		for (int j = 0; j < _map.GetLength(1); j++)
		{
			if (_map[i, j] == ch)
			{
				return [i, j];
			}	
		}
	}	
	
	throw new Exception("Node not found");
}

// Return next node, score of the path between nodes, dir of approach to next node, path between nodes
(int[], int, int[], List<int[]>) findNextNode(char[,] _map, int[] startPos, int[] dir)
{
	List<int[]> path = [startPos];
	int stepCount = 1;
	int[] nextPos = addPos(startPos, dir);
	char ch = _map[nextPos[0], nextPos[1]];

	// While there is only 1 path to contiue through. End the function if there are multiple paths
	while (ch == 'S' || ch == '#' || findEmptyNeighbours(_map, nextPos).Count == 2)
	{
		// If we hit the end point break and return current pos and score
		if (ch == 'E' || ch == 'S')
		{
			break;
		}
		
		// Then we had to make 1 90degree turn, check if it's clockwise or cclockwise
		if (ch == '#')
		{
			dir = nextDir(dir);
			nextPos = addPos(startPos, dir);
			ch = _map[nextPos[0], nextPos[1]];
			
			// Horrible hacky nested if statement but there are only 2 ways to turn and this works
			if (ch == '#')
			{
				dir = reverseDir(dir);
				nextPos = addPos(startPos, dir);
				ch = _map[nextPos[0], nextPos[1]];
				// if (ch == '#') throw new Exception("Should be 2 empty neighbours but couldn't find them");
				if (ch == '#') return ([-1,-1], 0, dir, path);
			}
		}
		else
		{
			startPos = nextPos;
			path.Add(nextPos);
			stepCount++;
			nextPos = addPos(startPos, dir);
			ch = _map[nextPos[0], nextPos[1]];
		}
		
	}
	
	return (nextPos, stepCount, dir, path);
}

int[] startNode = getNodeFromCh(map, 'S');
(int[] endNode, _, _, List<int[]> path) = findNextNode(map, startNode, [-1, 0]);
path.Add(endNode);
int[,] scoreMap = new int[map.GetLength(0), map.GetLength(1)];

for (int i = 0; i < path.Count; i++)
{
	scoreMap[path[i][0], path[i][1]] = i;
}

SortedDictionary<long, long> lengthSavedDict = [];

for (int i = 0; i < map.GetLength(0); i++)
{
	// Console.WriteLine("i = " + i);
	for (int j = 0; j < map.GetLength(1); j++)
	{
		if (map[i, j] != '#')
		{
			continue;
		}
		
		List<int[]> neighbours = findEmptyNeighbours(map, [i, j]);
		
		if (neighbours.Count <= 1)
		{
			continue;
		}
		
		List<int> neighbourScores = [];
		foreach (int[] neighbour in neighbours)
		{
			neighbourScores.Add(scoreMap[neighbour[0], neighbour[1]]);
		}
		
		int lengthSaved = neighbourScores.Max() - neighbourScores.Min() - 2;
		
		if (lengthSaved == 0)
		{
			continue;
		}
		
		if (lengthSavedDict.Keys.Contains(lengthSaved))
		{
			lengthSavedDict[lengthSaved]++;
		}
		else
		{
			lengthSavedDict[lengthSaved] = 1;
		}
	}
}

long totalCheats = 0;
foreach (long key in lengthSavedDict.Keys)
{
	// Console.WriteLine(lengthSavedDict[key] + ": " + key);
	if (key >= 100)
	{
		totalCheats += lengthSavedDict[key];
	}
}
Console.WriteLine("Solution 1 = " + totalCheats);
Console.WriteLine("time taken = " + (DateTime.Now - t0));

/* 
--------------- Part 2 --------------- 
Now I need to remove wall pairs with 20 block radii.
*/

lengthSavedDict = [];

for (int i = 0; i < map.GetLength(0); i++)
{
	// Console.WriteLine("i = " + i);
	for (int j = 0; j < map.GetLength(1); j++)
	{
		if (map[i, j] == '#')
		{
			continue;
		}
		
		// Now cycle through every cell in a 20 by 20 radius and check if the cell is a '.' 
		// and within distance 20 and saves 100 length
		for (int di = -20; di <= 20; di++)
		{
			for (int dj = -20; dj <= 20; dj++)
			{
				int x = i + di;
				int y = j + dj;
				
				if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1))
				{
					continue;
				}
				
				int lengthCheat = Math.Abs(di) + Math.Abs(dj);
				
				if (lengthCheat > 20)
				{
					continue;
				}
				
				if (map[x, y] == '#')
				{
					continue;
				}
				
				int lengthSaved = scoreMap[x, y] - scoreMap[i, j] - lengthCheat;
				
				if (lengthSaved < 100)
				{
					continue;
				}
				
				if (lengthSavedDict.Keys.Contains(lengthSaved))
				{
					lengthSavedDict[lengthSaved]++;
				}
				else
				{
					lengthSavedDict[lengthSaved] = 1;
				}
			}
		}
	}
}

totalCheats = 0;
foreach (long key in lengthSavedDict.Keys)
{
	Console.WriteLine(lengthSavedDict[key] + ": " + key);
	totalCheats += lengthSavedDict[key];
}
Console.WriteLine("Solution 2 = " + totalCheats);
Console.WriteLine("time taken = " + (DateTime.Now - t0));