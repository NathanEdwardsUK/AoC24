using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

int[] findCharInArray(char[,] _map, char ch)
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
	
	throw new Exception("Element not found in array");
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

bool checkInBounds(char[,] _map, int[] pos) => (pos[0] >= 0 && pos[0] < _map.GetLength(0)
											&& pos[1] >= 0 && pos[1] < _map.GetLength(1));

bool checkPosInList(List<int[]> _path, int[] pos)
{
	foreach (int[] _pos in _path)
	{
		if (_pos[0] == pos[0] && _pos[1] == pos[1])
		{
			return true;
		}
	}
	return false;
}

bool checkAllPathsContainEnd(char[,] _map, List<List<int[]>> _paths)
{
	foreach(List<int[]> path in _paths)
	{
		bool hasStart = false;
		
		foreach(int[] pos in path)
		{
			if (_map[pos[0], pos[1]] == 'E')
			{
				hasStart = true;
				break;
			}
		}
		
		if (!hasStart)
		{
			return false;
		}
	}
	
	return true;
}

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/16/16/input.txt");
char[,] map = stringToCharArray(sr.ReadToEnd());
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

void printPath(List<int[]> path)
{
	foreach (int[] p in path)
	{
		Console.Write($"[{p[0]},{p[1]}], ");
	}
	Console.WriteLine();
}

// Find every non divisible path and it's count
(int[], int) findNextNode(char[,] _map, int[] startPos, int[] dir, int turnCount)
{
	int stepCount = 1;
	int[] nextPos = addPos(startPos, dir);
	char ch = _map[nextPos[0], nextPos[1]];

	// While there is only 1 path to contiue through. End the function if there are multiple paths
	while (ch == 'E' || ch == '#' || findEmptyNeighbours(_map, nextPos).Count == 2)
	{
		// If we hit the end point break and return current pos and score
		if (ch == 'E')
		{
			break;
		}
		
		// Then we had to make 1 90degree turn, check if it's clockwise or cclockwise
		if (ch == '#')
		{
			turnCount++;
			dir = nextDir(dir);
			nextPos = addPos(startPos, dir);
			ch = _map[nextPos[0], nextPos[1]];
			
			// Horrible hacky nested if statement but there are only 2 ways to turn and this works
			if (ch == '#')
			{
				dir = nextDir(nextDir(dir));
				nextPos = addPos(startPos, dir);
				ch = _map[nextPos[0], nextPos[1]];
				if (ch == '#') throw new Exception("Should be 2 empty neighbours but couldn't find them");
			}
		}
		else
		{
			startPos = nextPos;
			stepCount++;
			nextPos = addPos(startPos, dir);
			ch = _map[nextPos[0], nextPos[1]];
		}
		
	}
	
	return (nextPos, stepCount + 1000 * turnCount);
}

long findLowestScorePath(char[,] _map, List<int[]> path, long totalScore)
{
	int[] curNode = path.Last();
	
	// If we reached the end then just return the score
	if (_map[curNode[0], curNode[1]] == 'E')
	{
		// Console.WriteLine(totalScore);
		// printPath(path);
		return totalScore;
	}
				
	// If dead end then return +inf score
	if (path.Count > 1 && findEmptyNeighbours(_map, curNode).Count == 1)
	{
		return Int32.MaxValue;
	}

	int[] dir = [-1, 0];
	long minScore = Int32.MaxValue;
	for (int dirCount = 0; dirCount < 4; dirCount++)
	{
		// var path = new List<int[]>(paths[i]);
		int[] nextPos = addPos(curNode, dir);
		char ch = _map[nextPos[0], nextPos[1]];
		if (ch != '#')
		{
			// We are rotating in this function but still need to count it in the score of findNextNode
			int turnCount = 0;
			if (path.Count > 1 && dirCount > 0)
			{
				turnCount = 1;
			}
			
			(int[] nextNode, int score) = findNextNode(_map, curNode, dir, turnCount);
			
			// If the node has already been visited in the list then pass
			if (!checkPosInList(path, nextNode))
			{
				List<int[]> newPath = [.. path, nextNode];
				long newScore = findLowestScorePath(_map, newPath, totalScore + score);
				minScore = Math.Min(minScore, newScore);
			}
		}
		dir = nextDir(dir);
	}
	totalScore = minScore;
		
	return totalScore;
}

// int findLowestScorePath2(char[,] _map)
// {	
// 	int[] startPos = findCharInArray(_map, 'S');
// 	List<List<int[]>> paths = [[startPos]];
// 	List<List<int[]>> completePaths = [];
// 	List<int> scores = [0];
// 	List<int> finalScores = [];
	
// 	// while (!checkAllPathsContainEnd(_map, paths))
// 	while (paths.Count > 0)
// 	{	
// 		// Start at the last path and work backwards as we will be adding new paths to the end
// 		for (int i = paths.Count - 1; i >= 0; i--)
// 		{	
// 			int[] curNode = paths[i].Last();
			
// 			// If we already reached the end then remove this path and print
// 			if (_map[curNode[0], curNode[1]] == 'E')
// 			{
// 				completePaths.Add(paths[i]);
// 				finalScores.Add(scores[i]);
// 				printPath(paths[i]);
// 				paths.RemoveAt(i);
// 				scores.RemoveAt(i);
// 			}
			
// 			int[] dir = [-1, 0];
						
// 			// If dead end delete the path AND block off the last node in the map then continue
// 			if (findEmptyNeighbours(_map, curNode).Count == 1)
// 			{
// 				// printPath(paths[i]);
// 				paths.RemoveAt(i);
// 				scores.RemoveAt(i);
// 				continue;
// 			}

// 			// For each non blocked direction find the next node in the path and add the path to the end
// 			// of paths. Delete the old incomplete path later 
// 			for (int dirCount = 0; dirCount < 4; dirCount++)
// 			{
// 				var path = new List<int[]>(paths[i]);
// 				int[] nextPos = addPos(curNode, dir);
// 				char ch = _map[nextPos[0], nextPos[1]];
// 				if (ch != '#')
// 				{
// 					(int[] nextNode, int score) = findNextNode(_map, nextPos, dir);
// 					// If the node has already been visited in the list dont add it again
// 					if (!checkPosInList(path, nextNode))
// 					{
// 						path.Add(nextNode);
// 						score = scores[i] + score;
// 						paths.Add(path);
// 						scores.Add(score);
// 					}
// 				}
// 				dir = nextDir(dir);
// 			}
			
// 			// Already added all new paths to the end of the list
// 			paths.RemoveAt(i);
// 			scores.RemoveAt(i);
// 		}
// 	}
	
// 	return scores.Min();
// }

int[] startPos = findCharInArray(map, 'S');
(int[] a , int b) = findNextNode(map, startPos, [0, 1], 0);
(int[] a2 , int b2) = findNextNode(map, [4,11], [0,1], 1);
long solution1 = findLowestScorePath(map, [startPos], 0);
Console.WriteLine("Solution 1 = " + solution1);




