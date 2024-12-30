using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

/* 
--------------- Part 1 --------------- 
I will be copying my code from day 16 again even though it is quite ugly. I don't want to waste 
a lot of time rewriting it.
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

int findNodeIdx(List<int[]> _nodes, int[] _node)
{
	for (int i = 0; i < _nodes.Count; i++)
	{
		if (_nodes[i][0] == _node[0] && _nodes[i][1] == _node[1])
		{
			return i;
		}
	}
	
	return -1;
}

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

void printPath(List<int[]> path)
{
	foreach (int[] p in path)
	{
		Console.Write($"[{p[0]},{p[1]}], ");
	}
	Console.WriteLine();
}

List<int[]> findAllnodes(char[,] _map)
{
	List<int[]> nodes = [];
	
	for (int i = 0; i < _map.GetLength(0); i++)
	{
		for (int j = 0; j < _map.GetLength(1); j++)
		{
			char ch = _map[i, j];
			if (ch == 'E' || ch == 'S')
			{
				nodes.Add([i, j]);
			}
			else if (ch == '.' && findEmptyNeighbours(_map, [i, j]).Count >= 3)
			{
				nodes.Add([i, j]);
			}
		}
	}
	return nodes;
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
(int[], int, int[], List<int[]>) findNextNode(char[,] _map, int[] startPos, int[] dir, int turnCount)
{
	List<int[]> path = [startPos];
	int stepCount = 1;
	int[] nextPos = addPos(startPos, dir);
	char ch = _map[nextPos[0], nextPos[1]];
	
	// If first ch is a wall return -1,-1 (meaning ignore)
	if (ch == '#')
	{
		return ([-1,-1], 0, dir, path);
	}

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
			turnCount++;
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

// Return nodes, nodeApproachedDirs, scores
(List<int[]> , Dictionary<string, int[]> , List<long>) dijkstra(char[,] _map, (int[], int[]) _ignoredNodeDirPair)
{
	int[] ignoredNode = _ignoredNodeDirPair.Item1;
	int[] ignoredDir = _ignoredNodeDirPair.Item2;
	
	// 1.
	List<int[]> uncheckedNodes = findAllnodes(_map);
	List<int[]> checkedNodes = [];
	Dictionary<string, int[]> nodeDirsReached = [];

	// 2.
	List<long> scores = Enumerable.Repeat((long) Int32.MaxValue, uncheckedNodes.Count).ToList();
	List<long> finalScores = [];
	int[] startNode = getNodeFromCh(_map, 'S');
	int startNodeIdx = findNodeIdx(uncheckedNodes, startNode);
	scores[startNodeIdx] = 0;
	nodeDirsReached[String.Join(",", startNode)] = [0, 1];

	// 3.
	while (uncheckedNodes.Count > 0)
	{
		long minScore = scores.Min();
		// If true then no more nodes are reachable
		if (minScore == Int32.MaxValue)
		{
			return (checkedNodes, nodeDirsReached, finalScores);
		}
		int minIdx = scores.IndexOf(minScore);
		
		// Now find all adjacent nodes which haven't been checked yet
		int[] node = uncheckedNodes[minIdx];
		
		int[][] dirs =  [[-1, 0], [0, 1], [1, 0], [0, -1]];
		foreach (int[] dir in dirs)
		{	
			// Check if the direction leaving the node is the same as the dir that reached the node, if not we need to count an extra turn
			int extraTurn = 0;
			if (!Enumerable.SequenceEqual(dir, nodeDirsReached[String.Join(",", node)]))
			{
				extraTurn = 1;	
			}
			(int[] nextNode, long score, int[] approachDir, _) = findNextNode(_map, node, dir, extraTurn);
			int idx = findNodeIdx(uncheckedNodes, nextNode);
			
			if (idx != -1)
			{
				// For part 2, if we are at a specific node moving in a specific direction the skip.
				if (Enumerable.SequenceEqual(nextNode, ignoredNode) && Enumerable.SequenceEqual(approachDir, ignoredDir))
				{
					continue;
				}	
					
				if (scores[minIdx] + score <= scores[idx])
				{
					scores[idx] = scores[minIdx] + score;
					// Record the direction from which the shortest path was reached
					nodeDirsReached[String.Join(",", nextNode)] = approachDir;
				}
			}
		}
		
		checkedNodes.Add(node);
		finalScores.Add(minScore);
		uncheckedNodes.RemoveAt(minIdx);
		scores.RemoveAt(minIdx);
	}
	
	return (checkedNodes, nodeDirsReached, finalScores);
}

(List<int[]> nodes, Dictionary<string, int[]> _, List<long> scores) = dijkstra(map, ([],[]));

int[] startNode = getNodeFromCh(map, 'S');
int[] endNode = getNodeFromCh(map, 'E');
int endNodeIdx = findNodeIdx(nodes, endNode);
long endNodeScore = scores[endNodeIdx];
long trackLength = endNodeScore;

// Now I found the track length without cheating I just need to cycle through every wall object, 
// delete it and check the above again. I could make it more efficient by add a condition checking
// whether a wall is worth deleting

SortedDictionary<long, long> lengthSavedDict = [];

for (int i = 0; i < map.GetLength(0); i++)
{
	Console.WriteLine("i = " + i);
	for (int j = 0; j < map.GetLength(1); j++)
	{
		if (map[i, j] == '#' && findEmptyNeighbours(map, [i, j]).Count > 1)
		{
			map[i, j] = '.';
			(List<int[]> nodes2, Dictionary<string, int[]> _, List<long> scores2) = dijkstra(map, ([],[]));
			map[i, j] = '#';
			int endNodeIdx2 = findNodeIdx(nodes2, endNode);
			long cheatTrackLength = scores2[endNodeIdx2];
			long lengthSaved = trackLength - cheatTrackLength;
			
			if (lengthSaved > 0)
			{
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

// List<long> sortedKeys = lengthSavedDict.Keys.ToList();
// sortedKeys.Sort();
long totalCheats = 0;
foreach (long key in lengthSavedDict.Keys)
{
	Console.WriteLine(key + ": " + lengthSavedDict[key]);
	if (key >= 100)
	{
		totalCheats += lengthSavedDict[key];
	}
}
Console.WriteLine(totalCheats);
// Console.WriteLine(String.Join(",", cheatTrackLengths));