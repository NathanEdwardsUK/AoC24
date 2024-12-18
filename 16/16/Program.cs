using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/16/16/input.txt");
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

/* --------------- Part 1 --------------- */
/*
Will need to implement Dijkstra's algorithm. Define a node as an empty space with at least 3 neighbouring empty spaces.
1. Make an array of all unchecked nodes and an empty array for checked nodes
2. Assign distance values to all nodes, 0 to starting node, inf (int32.MaxValue) to rest.
3. Starting at the node with lowest value in the unchecked set. If empty go to 6
4. Check the score distance to each adjacent node and set their distance values to the min of 
   themselves and the prev nodes value + distance
5. Move node to checked nodes
6. Return the score of the end node

The only tricky thing with this specific problem is that the direction of arriving at each node matters to the score.
So I could arrive at one node from 2 different directions with the same score but the direction of arrival matters when 
calculating the score of the next node. So I need to keep track of the min cost of arriving at each node PER direction.
*/

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
	
	return (nextPos, stepCount + 1000 * turnCount, dir, path);
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

(List<int[]> nodes, Dictionary<string, int[]> nodeApproachedDirs, List<long> scores) = dijkstra(map, ([],[]));

int[] startNode = getNodeFromCh(map, 'S');
int[] endNode = getNodeFromCh(map, 'E');
int endNodeIdx = findNodeIdx(nodes, endNode);
long endNodeScore = scores[endNodeIdx];
long solution1 = endNodeScore;
Console.WriteLine("Solution 1 = " + solution1);

/* --------------- Part 2 --------------- */
/*
Now I need to find all best paths and count the number of unique tiles. 
I am going to do this by recursively exploring all paths, stopping when the path score exceeds the endNodeScore, 
and counting all positions in the paths.
*/

(List<int[]>, List<int[]>) backTrackPath(char[,] _map, Dictionary<string, int[]> _nodeApproachedDirs)
{
	int[] curNode = getNodeFromCh(_map, 'E');
	int[] startNode = getNodeFromCh(_map, 'S');
	List<int[]> pathNodes = [curNode];
	List<int[]> totalPath = [curNode];
	
	while (!Enumerable.SequenceEqual(curNode, startNode))
	{
		int[] dir = _nodeApproachedDirs[String.Join(",", curNode)];
		int[] revDir = reverseDir(dir);
		
		// Getting the previous node as we are working backwards from the end node to the start
		(int[] prevNode, _, _, List<int[]> prevPath) = findNextNode(_map, curNode, revDir, 0);
		pathNodes.Add(prevNode);
		totalPath = [..prevPath, ..totalPath];
		curNode = prevNode;
	}
	
	return (pathNodes, totalPath);

}

(List<int[]> shortPathNodes, List<int[]> shortPath) = backTrackPath(map, nodeApproachedDirs);

List<int[]> findAlternatePath(char[,] _map, (int[], int[]) _ignoredNodeDirPair, long _scoreToMatch)
{
	(List<int[]> newNodes, Dictionary<string, int[]> newNodeApproachedDirs, List<long> newScores) 
		= dijkstra(map, _ignoredNodeDirPair);
	int[] endNode = getNodeFromCh(map, 'E');
	int endNodeIdx = findNodeIdx(newNodes, endNode);
	// If true then the end node couldnt be reached
	if (endNodeIdx == -1)
	{
		return[];
	}
	long endNodeScore = newScores[endNodeIdx];
	
	if (endNodeScore < _scoreToMatch)
	{
		throw new Exception("should not be possible to find a lower score");
	}
	else if (endNodeScore == _scoreToMatch)
	{
		(List<int[]> deletemenodes, List<int[]> newPath) = backTrackPath(_map, newNodeApproachedDirs);
		return newPath;
	}
	
	return [];
}

HashSet<string> goodSeats = [];
goodSeats.Add(String.Join(",", startNode));
goodSeats.Add(String.Join(",", endNode));
foreach (int[] pos in shortPath)
{
	goodSeats.Add(String.Join(",", pos));
}
printSeats(goodSeats, map);

void printSeats(HashSet<string> seats, char[,] _map)
{
	for (int i = 0; i < _map.GetLength(0); i++)
	{
		for (int j = 0; j < _map.GetLength(1); j++)
		{
			if (i == 0 || i == _map.GetLength(0) - 1 || j == 0 || j == _map.GetLength(1) - 1)
			{
				Console.Write("#");
				continue;
			}
			
			if (seats.Contains($"{i},{j}"))
			{
				Console.Write("0");	
			}
			else
			{
				Console.Write(" ");
			}
		}
		Console.Write("\n");
	}
}

foreach (int[] ignoredNode in shortPathNodes)
{
	if (Enumerable.SequenceEqual(ignoredNode, endNode) || Enumerable.SequenceEqual(ignoredNode, startNode))
	{
		continue;
	}
	int[] ignoredDir = nodeApproachedDirs[String.Join(",", ignoredNode)];
	List<int[]> newPath = findAlternatePath(map, (ignoredNode, ignoredDir), endNodeScore);
	foreach (int[] pos in newPath)
	{
		goodSeats.Add(String.Join(",", pos));
	}

}

printSeats(goodSeats, map);
Console.WriteLine("Solution 2 = " + goodSeats.Count);
