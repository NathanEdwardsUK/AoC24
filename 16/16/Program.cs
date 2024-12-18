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

int getNodeIndexFromCh(List<int[]> _nodes, char ch)
{
	for (int i = 0; i < _nodes.Count; i++)
	{
		int[] node = _nodes[i];
		if (map[node[0], node[1]] == ch)
		{
			return i;
		}
	}	
	
	throw new Exception("Node not found");
}

// Return next node, score of the path between nodes, dir of approach to next node, path between nodes
(int[], int, int[], List<int[]>) findNextNode(char[,] _map, int[] startPos, int[] dir, int turnCount)
{
	List<int[]> path = [];
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
				dir = nextDir(nextDir(dir));
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

// 1.
List<int[]> uncheckedNodes = findAllnodes(map);
List<int[]> checkedNodes = [];
Dictionary<string, int[]> nodeDirsReached = [];

// 2.
List<long> scores = Enumerable.Repeat((long) Int32.MaxValue, uncheckedNodes.Count).ToList();
List<long> finalScores = [];
int startNodeIdx = getNodeIndexFromCh(uncheckedNodes, 'S');
int[] startNode = uncheckedNodes[startNodeIdx];
scores[startNodeIdx] = 0;
nodeDirsReached[String.Join(",", startNode)] = [0, 1];

// 3.
while (uncheckedNodes.Count > 0)
{
	long minScore = scores.Min();
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
		(int[] nextNode, long score, int[] approachDir, _) = findNextNode(map, node, dir, extraTurn);
		int idx = findNodeIdx(uncheckedNodes, nextNode);
		
		if (idx != -1)
		{
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

int endNodeIdx = getNodeIndexFromCh(checkedNodes, 'E');
long endNodeScore = finalScores[endNodeIdx];
long solution1 = endNodeScore;
Console.WriteLine("Solution 1 = " + solution1);

// for (int i = 0; i < finalScores.Count; i++)
// {
// 	Console.WriteLine(String.Join(",", checkedNodes[i]) + " - " + finalScores[i]);
// }

/* --------------- Part 2 --------------- */
/*
Now I need to find all best paths and count the number of unique tiles. 
I am going to do this by recursively exploring all paths, stopping when the path score exceeds the endNodeScore, 
and counting all positions in the paths.
*/

int[,] posInBestPath = new int[map.GetLength(0), map.GetLength(1)];
int[] endNode = checkedNodes[endNodeIdx];
int [] endNodeApproachDir = nodeDirsReached[String.Join(",", endNode)];
// I have to flip the end node approach dir as we are working backwards from the end now
endNodeApproachDir = nextDir(nextDir(endNodeApproachDir));

List<(int, int[])> idxsAndDirsOflastNodesInValidPaths = [(endNodeIdx, endNodeApproachDir)];

// while (idxsAndDirsOflastNodesInValidPaths.Count > 0)
// {
// 	int[] curNode = checkedNodes[idxsAndDirsOflastNodesInValidPaths[0].Item1];
// 	long curScore = finalScores[idxsAndDirsOflastNodesInValidPaths[0].Item1];
// 	// int[] approachDir = idxsAndDirsOflastNodesInValidPaths[0].Item2;
// 	int[] approachDir = nextDir(nextDir(nodeDirsReached[String.Join(",", curNode)]));
// 	posInBestPath[curNode[0], curNode[1]] = 1;
	
// 	int[][] dirs =  [[-1, 0], [0, 1], [1, 0], [0, -1]];
// 	foreach (int[] dir in dirs)
// 	{
// 		int extraTurn = 0;
// 		if (!Enumerable.SequenceEqual(approachDir, dir))
// 		{
// 			extraTurn = 1;
// 		}
		
// 		(int[] nextNode, long score, int[] nextApproachDir, List<int[]> path) = findNextNode(map, curNode, dir, extraTurn);
		
// 		int idx = findNodeIdx(checkedNodes, nextNode);
// 		if (idx == -1)
// 		{
// 			continue;
// 		}
		
// 		long nextNodeScore = finalScores[idx];
		
// 		// If this is true then the next node must be in a valid path
// 		if (nextNodeScore + score <= curScore || nextNodeScore + score + 1000 == curScore)
// 		{
// 			bool contained = false;
// 			foreach ((int, int[]) tup in idxsAndDirsOflastNodesInValidPaths)
// 			{
// 				if (idx == tup.Item1 && Enumerable.SequenceEqual(dir, tup.Item2))
// 				{
// 					contained = true;
// 					break;
// 				}
// 			}
			
// 			if (!contained)
// 			{
// 				idxsAndDirsOflastNodesInValidPaths.Add((idx, nextApproachDir));
// 				// approachDirs.Add(nextApproachDir);
// 			}
			
// 			// Then update our array of positions to show that this path is a subset of a best path
// 			foreach (int[] pos in path)
// 			{
// 				posInBestPath[pos[0], pos[1]] = 1;
// 			}
// 		}
// 	}
	
// 	idxsAndDirsOflastNodesInValidPaths.RemoveAt(0);
// 	// approachDirs.RemoveAt(0);
// }

// Hashset recursion attempt
if (false)
{
	HashSet<string> listToStringSet(List<int[]> list)
	{
		HashSet<string> set = new ();
		foreach (int[] pos in list)
		{
			set.Add(String.Join(",", pos));
		}
		
		return set;
	}
	
	HashSet<string> findLowestScorePath(char[,] _map, List<int[]> _path, HashSet<string> _validCells, int[] _node, long _curScore, long _maxScore, int[] _approachDir)
	{
		// If we reached the end then just return the valid cells
		if (_map[_node[0], _node[1]] == 'E')
		{
			_validCells = listToStringSet(_path);
			return _validCells;
		}

		int[][] dirs =  [[-1, 0], [0, 1], [1, 0], [0, -1]];
		foreach (int[] dir in dirs)
		{
			int extraTurn = 0;
			if(!Enumerable.SequenceEqual(dir, _approachDir))
			{
				extraTurn++;
			}
			
			(int[] nextNode, long score, int[] nextApproachDir, List<int[]> nextPath) = findNextNode(_map, _node, dir, extraTurn);
			
			if (findNodeIdx(checkedNodes, nextNode) == -1 || findNodeIdx(_path, nextNode) != -1)
			{
				continue;	
			}
		
			if (_curScore + score > _maxScore)
			{	
				continue;
			}
			
			// Console.WriteLine(String.Join(",",nextNode) + " score = " + (_curScore + score));
			// printPath([.._path, nextNode]);
			// _validCells = [.._validCells, ..findLowestScorePath(_map, [.._path, ..nextPath, nextNode], _validCells, nextNode, _curScore + score, _maxScore, nextApproachDir)];
			foreach (string cell in findLowestScorePath(_map, [.._path, ..nextPath, nextNode], _validCells, nextNode, _curScore + score, _maxScore, nextApproachDir))
			{
				_validCells.Add(cell);
			}
		}

		return _validCells;
	}

	HashSet<string> validCells = findLowestScorePath(map, [startNode], [], startNode, 0, endNodeScore, [0, 1]);
	int solution2 = validCells.Count;

	Console.WriteLine("Solution 2 = " + solution2);
}




