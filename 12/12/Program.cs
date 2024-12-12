using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/12/12/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split("\n");
int numRows = inputStrSplit.Count();
int numCols = inputStrSplit[0].Length;
char[,] charArray = new char[numRows, numCols];

// Convert map into 2d char array
for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		charArray[i, j] = inputStrSplit[i][j];
	}
}

/*
Approach is to iterate through each cell in the charArray (garden), iterate through one connected group of letters
at a time, calculate area as sum of connected letters, calculate perimeter as sum of bordering different letters or
no letters. Maintain a list of checked cells and the number of sides checked, keep iterating until all sides of all
cells in the list have been checked then move on.
*/

int fenceCost1 = 0;
int fenceCost2 = 0;
int[,] numNeigboursCheckedArray = new int[numRows, numCols];

bool groupContainsCell(List<int[]> _connectedGroup, int[] _cell)
{
	foreach (int[] cell in _connectedGroup)
	{
		if (Enumerable.SequenceEqual(_cell, cell))
		{
			return true;
		}
	}
	return false;
}

bool cellIsValid(int[] _cell, char _letter, List<int[]> _connectedGroup)
{
	return _cell[0] >= 0 && _cell[0] < numRows
		&& _cell[1] >= 0 && _cell[1] < numCols
		&& charArray[_cell[0], _cell[1]] == _letter
		&& !groupContainsCell(_connectedGroup, _cell);
}

int[] getFirstCellInNextGroup()
{
	for (int i = 0; i < numNeigboursCheckedArray.GetLength(0); i++)
	{
		for (int j = 0; j < numNeigboursCheckedArray.GetLength(1); j++)
		{
			if (numNeigboursCheckedArray[i, j] == 0)
			{
				return [i, j];
			}
			else if (numNeigboursCheckedArray[i, j] == 4)
			{
				continue;
			}
			else 
			{
				throw new Exception($"Found cell with partial neighbours checked during getFirstCellInNextGroup, [{i},{j}]");
			}
		}
	}
	
	Console.WriteLine("Could not find next cell. Process complete");
	return null;
}

int[] getNextCellInGroup(List<int[]> _connectedGroup, char _letter)
{
	foreach (int[] cell in _connectedGroup)
	{
		int numNeigboursChecked  = 0;
		
		while (numNeigboursChecked != 4)
		{	
			int x = cell[0];
			int y = cell[1];
			numNeigboursChecked = numNeigboursCheckedArray[x, y];
			
			if (numNeigboursChecked == 4)
			{
				continue;
			}
			
			int x1;
			int y1;
			numNeigboursCheckedArray[x, y]++;
			
			if (numNeigboursChecked == 0)
			{
				(x1, y1) = (x - 1, y);
			}
			else if (numNeigboursChecked == 1)
			{
				(x1, y1) = (x, y + 1);
			}
			else if (numNeigboursChecked == 2)
			{
				(x1, y1) = (x + 1, y);
			}
			else if (numNeigboursChecked == 3)
			{
				(x1, y1) = (x, y - 1);
			}
			else
			{
				throw new Exception($"numNeigboursChecked = {numNeigboursChecked} not in range [0,3]");
			}
			
			if (cellIsValid([x1, y1], _letter, _connectedGroup))
			{
				return [x1, y1];
			}
		}
	}
	
	return null;
}

int countPerimeter(char _letter, int[] _cell, char[,] _charArray)
{
	int x = _cell[0];
	int y = _cell[1];
	int count = 0;
	
	if (x == 0 || charArray[x - 1, y] != _letter)
	{
		count++;
	}
	
	if (y == 0 || charArray[x, y - 1] != _letter)
	{
		count++;
	}
	
	if (x + 1 == charArray.GetLength(0) || charArray[x + 1, y] != _letter)
	{
		count++;
	}
	
	if (y + 1 == charArray.GetLength(1) || charArray[x, y + 1] != _letter)
	{
		count++;
	}
	
	return count;
}

int n = 1;
char letter;

while (getFirstCellInNextGroup() != null)
{
	// Find next cell that belongs to an unchecked group
	int[] cell = getFirstCellInNextGroup();
	letter = charArray[cell[0], cell[1]];
	// Console.WriteLine($"Next group found cell = [{cell[0]},{cell[1]}], letter = {letter}");
	List<int[]> connectedGroup = [cell];
	
	int area = 1;
	int perimeter = countPerimeter(letter, cell, charArray);

	// Loops until getNextCellInGroup is null
	while (true)
	{
		cell = getNextCellInGroup(connectedGroup, letter);
		if (cell == null)
		{
			break;
		}
		
		connectedGroup.Add(cell);
		area++;
		perimeter += countPerimeter(letter, cell, charArray);
		
		// Console.WriteLine($"Next cell = [{cell[0]},{cell[1]}], perimeter = {perimeter}");
	}
	
	int sides = countSides(connectedGroup);
	
	fenceCost1 += area * perimeter;
	fenceCost2 += area * sides;
	// Console.WriteLine($"{n} No more cells found in group. Letter = {letter}");
	// Console.WriteLine($"{n} area = {area}, perimeter = {perimeter}, cost = {area * perimeter}");
	n++;
}

Console.WriteLine("solution 1 = " + fenceCost1);

/* --------------- Part 2 --------------- */
/* 
Now I need to repeat the same as above but instead of calculating perimeter calculate the number of straight edges in the group.
This should be quite easy as I already keep track of the entire group, I just need to create a function to calculate the group sides.
*/

string getNextDir(List<string> alreadyCheckedDirs)
{
	string dir = "";
	List<string> directions = ["Up", "Right", "Down", "Left"];
	
	foreach (string _dir in directions)
	{
		// If true then we have not checked the dir side of this cell
		if (!alreadyCheckedDirs.Contains(_dir))
		{
			dir = _dir;
			break;
		}
	}
	
	if (dir == "")
	{
		// throw new Exception("No remaining direction found");
	}
	
	return dir;
}

int[] getNextCell(int[] cell, string dir)
{
	(int x, int y) = (cell[0], cell[1]);
	int x1;
	int y1;
	
	if (dir == "Up")
	{
		(x1, y1) = (x - 1, y);
	}
	else if (dir == "Right")
	{
		(x1, y1) = (x, y + 1);
	}
	else if (dir == "Down")
	{
		(x1, y1) = (x + 1, y);
	}
	else if (dir == "Left")
	{
		(x1, y1) = (x, y - 1);
	}
	else
	{
		throw new Exception($"Dir = {dir} not valid");
	}
	
	return [x1, y1];
}

string[] getPerpendicularDirs(string dir)
{
	if (dir == "Up" || dir == "Down")
	{
		return ["Left", "Right"];
	}
	else if (dir == "Left" || dir == "Right")
	{
		return ["Up", "Down"];
	}
	else
	{
		throw new Exception();
	}
}

string arrayToString(int[] intArray){
	return $"[{intArray[0]},{intArray[1]}]";
}

bool allCellsChecked(Dictionary<string, List<string>> _sidesCheckedDict)
{
	foreach (List<string> dirsChecked in _sidesCheckedDict.Values)
	{
		if (dirsChecked.Count < 4)
		{
			return false;
		}
	}
	return true;
}

int countSides(List<int[]> _connectedGroup)
{
	Console.WriteLine("\n\nCounting sides for group of size " + _connectedGroup.Count);
	int sides = 0;
	
	// keys will be cells (hackily converted to strings) and values the list of sides 
	// already checked ("Up", "Right", "Down", "left")
	Dictionary<string, List<string>> sidesCheckedDict = new Dictionary<string, List<string>>();
	foreach (int[] cell in _connectedGroup)
	{
		sidesCheckedDict.Add(arrayToString(cell), []);
	}
	
	while (!allCellsChecked(sidesCheckedDict))
	{
		foreach (int[] cell in _connectedGroup)
		{
			string cellKey = arrayToString(cell);
			
			// If all dir have been checked already then move on
			if (sidesCheckedDict[cellKey].Count == 4)
			{
				continue;
			}
			
			// Get next direction that has not been checked yet for this cell
			string dir = getNextDir(sidesCheckedDict[cellKey]);
			// Console.WriteLine($"{cellKey} next dir = {dir}. Already checked dirs = [{String.Join(",",sidesCheckedDict[cellKey])}]");
			sidesCheckedDict[cellKey].Add(dir);
			int[] nextCell = getNextCell(cell, dir);

			// Check if the next cell is in our group, if so then no new side was discovered so go to next iteration
			if (sidesCheckedDict.Keys.ToList().Contains(arrayToString(nextCell)))
			{
				continue;
			}
			
			// If we make it here then we have found a new side and just need to check how far along it goes and 
			// update that we have cheked this dir for the other cells that comprise this side
			
			
			sides ++;
			string[] perpendicularDirs = getPerpendicularDirs(dir);
			Console.WriteLine($"New side found! {cellKey} {dir}");

			foreach (string perpDir in perpendicularDirs)
			{
				int[] prevCell = cell;
				while (true)
				{
					nextCell = getNextCell(prevCell, perpDir);
					string nextCellKey = arrayToString(nextCell);
					
					// Need to also check the the cell above (in the case of dir = "Up") the perpendicular 
					// cell is also not in the group
					
					int[] nextCellPerp = getNextCell(nextCell, dir);
					string nextCellPerpKey = arrayToString(nextCellPerp);
					
					if (!sidesCheckedDict.Keys.ToList().Contains(nextCellKey))
					{
						// Console.WriteLine($"nextCell {perpDir} is not in our group");
						break;
					}
					
					if (sidesCheckedDict.Keys.ToList().Contains(nextCellPerpKey))
					{
						// Console.WriteLine($"nextCell {perpDir} is in our group but not part of the edge");
						break;
					}
					
					if (sidesCheckedDict[nextCellKey].Contains(dir))
					{
						throw new Exception();
					}
					
					sidesCheckedDict[nextCellKey].Add(dir);
					// Console.WriteLine($"added {nextCellKey} to the the edge and updated sidesCheckedDict");
					prevCell = nextCell;
				}
			}
		}
	}
	
	return sides;
}

Console.WriteLine("solution 2 = " + fenceCost2);