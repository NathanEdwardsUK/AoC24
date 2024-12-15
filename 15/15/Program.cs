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

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/15/15/input.txt");
string[] parts = sr.ReadToEnd().Split("\n\n");
char[,] map = stringToCharArray(parts[0]);
List<char> moves = parts[1].Replace("\n", "").ToList();
int[] curPos = findCharInArray(map, '@');

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

(char[,], int[]) applyMove(char[,] _map, char _move, int[] _curPos)
{
	// Convert direction charachter into x, y vector
	int [] dir;
	if (_move == '<')
	{
		dir = [0, -1];
	}
	else if (_move == '^')
	{
		dir = [-1, 0];
	}
	else if (_move == '>')
	{
		dir = [0, 1];
	}
	else if (_move == 'v')
	{
		dir = [1, 0];
	}
	else 
	{
		throw new Exception("Invalid move " + _move);
	}
	
	// next robot position
	int[] nextPos = [_curPos[0] + dir[0], _curPos[1] + dir[1]];
	char nextChar = _map[nextPos[0], nextPos[1]];
	
	// If next pos is empty move the robot there and make the previous pos empty
	if (nextChar == '.') 
	{
		_map[nextPos[0], nextPos[1]] = '@';
		_map[_curPos[0], _curPos[1]] = '.';
		_curPos = [_curPos[0] + dir[0], _curPos[1] + dir[1]];
	} 
	// If next pos is wall then make no changes
	else if (nextChar == '#')
	{
		// go to end
	}
	// If next pos is box then find out how many boxes are stacked in a row and what is in the next non box pos
	else if (nextChar == 'O')
	{
		int i = 1;
		while (nextChar == 'O')
		{
			i++;
			nextPos = [_curPos[0] + i * dir[0], _curPos[1] + i * dir[1]];
			nextChar = _map[nextPos[0], nextPos[1]];
		}
		
		// If the next non box pos is a wall then stop
		if (nextChar == '#')
		{
			// go to end
		}
		// If empty then shift the first box to the last pos and move the robot along 1
		else if (nextChar == '.')
		{
			_map[nextPos[0], nextPos[1]] = 'O';
			_map[_curPos[0] + dir[0], _curPos[1] + dir[1]] = '@';
			_map[_curPos[0], _curPos[1]] = '.';
			_curPos = [_curPos[0] + dir[0], _curPos[1] + dir[1]];
		}
		else
		{
			throw new Exception("invalid map object during box push");
		}
	}
	// Part 2 addition
	else if (nextChar == '[' || nextChar == ']')
	{
		// Now we will iterate through layers of boxes, if the top layer is followed by a layer of empty space then move the whole 
		// group along one. Else do nothing
		List<int[][]> checkedBoxes = [];
		List<int[][]> uncheckedBoxes = [];
		int[][] box = getBox(_map, nextPos);
		uncheckedBoxes.Add(box);
		// checkedBoxes.Add(box);
		
		while (uncheckedBoxes.Count > 0)
		{
			box = uncheckedBoxes[0];
			foreach (int[] pos in box)
			{
				// next position after box
				nextPos = [pos[0] + dir[0], pos[1] + dir[1]];
				nextChar = _map[nextPos[0], nextPos[1]];
				
				
				if (nextChar == '.') 
				{
					// Continue
				} 
				// If next pos is wall then we can't move this group of boxes so return the unchanged map
				else if (nextChar == '#')
				{
					return (_map, _curPos);
				}
				else if (nextChar == '[' || nextChar == ']')
				{
					int[][] box2 = getBox(_map, nextPos);
					// Check that the box hasn't already been added to the end of the list
					if (!checkForBoxInBoxes(uncheckedBoxes, box2))
					{
						uncheckedBoxes.Add(box2);
					}
				}
				else
				{
					throw new Exception("invalid map object");
				}
			}
			uncheckedBoxes.RemoveAt(0);
			checkedBoxes.Add(box);
		}
		
		// If we made it this far then the complete group of boxes can be moved along so do that
		for (int i = checkedBoxes.Count - 1; i >= 0; i--)
		{
			box = checkedBoxes[i];
			_map[box[0][0], box[0][1]] = '.';
			_map[box[1][0], box[1][1]] = '.';
			_map[box[0][0] + dir[0], box[0][1] + dir[1]] = '[';
			_map[box[1][0] + dir[0], box[1][1] + dir[1]] = ']';
		}
		// And move the robot
		_map[_curPos[0], _curPos[1]] = '.';
		_curPos = [_curPos[0] + dir[0], _curPos[1] + dir[1]];
		_map[_curPos[0], _curPos[1]] = '@';	
	}
	else
	{
		throw new Exception("invalid map object");
	}

	return (_map, _curPos);
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

foreach (char move in moves)
{
	(map, curPos) = applyMove(map, move, curPos);
}

printMap(map);

// Calculate solution metric (sum 100y + x over all box coordinates)
int answer1 = 0;
for (int i = 0; i < map.GetLength(0); i++)
{
	for (int j = 0; j < map.GetLength(1); j++)
	{
		if (map[i, j] == 'O')
		{
			answer1 += 100 * i + j;
		}
	}
}

Console.WriteLine("Solution 1 = " + answer1);

/* --------------- Part 2 --------------- */
/*
Now I need to transform the original map and change the logic for handline box movements
*/

char[,] transformMap(char[,] _map)
{
	int height = _map.GetLength(0);
	int width = _map.GetLength(1);
	var newMap = new char[height, width * 2];
	for (int i = 0; i < width; i++)
	{
		for (int j = 0; j < height; j++)
		{
			if (_map[i, j] == '#')
			{
				newMap[i, 2 * j] = '#';
				newMap[i, 2 * j + 1] = '#';
			} 
			else if (_map[i, j] == '.')
			{
				newMap[i, 2 * j] = '.';
				newMap[i, 2 * j + 1] = '.';
			} 
			else if (_map[i, j] == 'O')
			{
				newMap[i, 2 * j] = '[';
				newMap[i, 2 * j + 1] = ']';
			} 
			else if (_map[i, j] == '@')
			{
				newMap[i, 2 * j] = '@';
				newMap[i, 2 * j + 1] = '.';
			} 
			else
			{
				throw new Exception("Invalid character");
			}
		}
	}
	return newMap;
}

int[][] getBox(char[,] _map, int[] pos)
{
	int[][] box;
	
	if (_map[pos[0], pos[1]] == '[') 
	{	
		// Error checking
		if (_map[pos[0], pos[1] + 1] != ']')
		{
			throw new Exception("half box found");
		}
		box = [[pos[0], pos[1]], [pos[0], pos[1] + 1]];
	}
	else if (_map[pos[0], pos[1]] == ']') 
	{
		// Error checking
		if (_map[pos[0], pos[1] - 1] != '[')
		{
			throw new Exception("half box found");
		}
		box = [[pos[0], pos[1] - 1], [pos[0], pos[1]]];
	}
	else 
	{
		throw new Exception("invalid box");
	}
	
	return box;
}

bool checkForBoxInBoxes(List<int[][]> boxes, int[][] _box)
{
	foreach (int[][] _box2 in boxes)
	{
		if (_box[0][0] == _box2[0][0] && _box[0][1] == _box2[0][1])
		{
			return true;
		}
	}
	return false;
}

map = stringToCharArray(parts[0]);
char[,] newMap = transformMap(map);
curPos = findCharInArray(newMap, '@');
printMap(newMap);

foreach (char move in moves)
{
	// Console.WriteLine(move);
	(newMap, curPos) = applyMove(newMap, move, curPos);
	// printMap(newMap);
}

printMap(newMap);

// Calculate solution metric (sum 100y + x over all box coordinates)
int answer2 = 0;
for (int i = 0; i < newMap.GetLength(0); i++)
{
	for (int j = 0; j < newMap.GetLength(1); j++)
	{
		if (newMap[i, j] == '[')
		{
			answer2 += 100 * i + j;
		}
	}
}

Console.WriteLine("Solution 2 = " + answer2);

