using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

DateTime t0 = DateTime.Now;

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

List<int[]> findDistanceToNeighbourNodes(char[,] _map, int[] _node)
{
	List<int[]> neighbourNodesAndDistances = [];
	int[][] dirs = [[-1, 0], [0, 1], [1, 0], [0, -1]];
		
	foreach (int[] dir in dirs)
	{
		(int[] node2, int score, _, _) = findNextNode(_map, _node, dir);
		
		if (node2[0] == -1)
		{
			continue;
		}
		
		neighbourNodesAndDistances.Add([..node2, score]);
	}
	
	return neighbourNodesAndDistances;
}

// Returns a list of links between neighbouring nodes (by index) and length of path. [node 1 idx, node 2 idx, score]
Dictionary<string, List<int[]>> findNodesDistances(char[,] _map, List<int[]> _nodes)
{
	Dictionary<string, List<int[]>> nodeDistances = [];

	for (int i = 0; i < _nodes.Count; i++)
	{
		int[] node1 = _nodes[i];
		nodeDistances[String.Join(",", node1)] = findDistanceToNeighbourNodes(_map, node1);
	}	
	
	return nodeDistances;
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
(List<int[]> , List<long>) dijkstra(char[,] _map, List<int[]> _nodes, Dictionary<string, List<int[]>>  _nodeDistances)
{	
	// 1.
	List<int[]> uncheckedNodes = new (_nodes);
	List<int[]> checkedNodes = [];

	// 2.
	List<long> scores = Enumerable.Repeat((long) Int32.MaxValue, uncheckedNodes.Count).ToList();
	List<long> finalScores = [];
	int[] startNode = getNodeFromCh(_map, 'S');
	int startNodeIdx = findNodeIdx(uncheckedNodes, startNode);
	scores[startNodeIdx] = 0;

	// 3.
	while (uncheckedNodes.Count > 0)
	{
		long minScore = scores.Min();
		// If true then no more nodes are reachable
		if (minScore == Int32.MaxValue)
		{
			return (checkedNodes, finalScores);
		}
		int minIdx = scores.IndexOf(minScore);
		
		// Now find all adjacent nodes which haven't been checked yet
		int[] node = uncheckedNodes[minIdx];
		
		foreach (int[] nodeAndScore in _nodeDistances[String.Join(",", node)])
		{
			int[] nextNode = [nodeAndScore[0], nodeAndScore[1]];
			int score = nodeAndScore[2];
			int idx = findNodeIdx(uncheckedNodes, nextNode);
			
			if (idx != -1)
			{
				if (scores[minIdx] + score <= scores[idx])
				{
					scores[idx] = scores[minIdx] + score;
				}
			}
		}
		
		checkedNodes.Add(node);
		finalScores.Add(minScore);
		uncheckedNodes.RemoveAt(minIdx);
		scores.RemoveAt(minIdx);
	}
	
	return (checkedNodes, finalScores);
}

List<int[]> initialNodes = findAllnodes(map);
Dictionary<string, List<int[]>> nodeDistances = findNodesDistances(map, initialNodes);
(List<int[]> reachableNodes, List<long> scores) = dijkstra(map, initialNodes, nodeDistances);

int[] startNode = getNodeFromCh(map, 'S');
int[] endNode = getNodeFromCh(map, 'E');
int endNodeIdx = findNodeIdx(reachableNodes, endNode);
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
			List<int[]> initialNodes2 = new (initialNodes);
			Dictionary<string, List<int[]>> nodeDistances2 = new (nodeDistances);

			foreach (int[] neighbour in findEmptyNeighbours(map, [i, j]))
			{
				if (findEmptyNeighbours(map, neighbour).Count > 1)
				{
					initialNodes2.Add(neighbour);
					nodeDistances2[String.Join(",", neighbour)] = findDistanceToNeighbourNodes(map, neighbour);
				}
			}
						
			(List<int[]> nodes2, List<long> scores2) = dijkstra(map, initialNodes2, nodeDistances);
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


long totalCheats = 0;
foreach (long key in lengthSavedDict.Keys)
{
	Console.WriteLine(key + ": " + lengthSavedDict[key]);
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
My original solution to part 1 ran quite slowly so I somewhat optimised my implementation of dijkstra.
Removing wall pairs within in a 20 block radius is equivalent to adding 2 new nodes to my nodes list and the distance
between them to my nodeDistances
*/

