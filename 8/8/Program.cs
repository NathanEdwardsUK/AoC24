using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/8/8/input.txt");

// First parse data
string inputStr = sr.ReadToEnd();
string[] inputStrSplit = inputStr.Split("\n");
int numRows = inputStrSplit.Count();
int numCols = inputStrSplit[0].Length;
Char[,] mapArray = new Char[numRows, numCols];
int[,] nodeArray = new int[numRows, numCols];

Dictionary<Char, List<int[]>> freqLocations = new Dictionary<Char, List<int[]>>();


// Convert map into 2d int array and populates dictionary of frequencies and locations
for (int i = 0; i < numRows; i++)
{
	for (int j = 0; j < numCols; j++)
	{
		Char chr = inputStrSplit[i][j];
		mapArray[i, j] = chr;
		if (chr != '.')
		{
			if (freqLocations.ContainsKey(chr)) 
			{
				freqLocations[chr].Add([i, j]);
			}
			else
			{
				freqLocations.Add(chr, [[i, j]]);
			}	
		}
	}
}

bool isInMap(int[,] nodeArray, int[] pos)
{
	bool inMap = pos[0] >= 0 && pos[0] < mapArray.GetLength(0) 
				&& pos[1] >= 0 && pos[1] < mapArray.GetLength(1);
	return inMap;
}

int[,] updateNodeArray(int[,] nodeArray, int[] pos)
{
	if (isInMap(nodeArray, pos)) 
	{
		nodeArray[pos[0], pos[1]] = 1;
	}
	
	return nodeArray;
}

/* 
Plan:
Cycle through each frequency (dict key) and every antenna position per freq. 
Try every combination of antenna to find all antinode combinations and update nodesArray.
There will be some double calculating but since double counting is not possible (max 1 node per position)
that doesn't matter. 
 */
 
foreach (Char key in freqLocations.Keys)
{
	foreach (int[] pos1 in freqLocations[key])
	{
		foreach (int[] pos2 in freqLocations[key])
		{
			if (Enumerable.SequenceEqual(pos1, pos2))
			{
				continue;
			}
			
			int[] node1 = [2 * pos1[0] - pos2[0], 2 * pos1[1] - pos2[1]];
			int[] node2 = [2 * pos2[0] - pos1[0], 2 * pos2[1] - pos1[1]];
			
			updateNodeArray(nodeArray, node1);
			updateNodeArray(nodeArray, node2);
		}
	}
}

Console.WriteLine("Solution 1 = " + nodeArray.Cast<int>().Sum());

/* --------------- Part 2 --------------- */
/* 
Now we must generate nodes periodically at multiples of the distance between any 2 antennas.
The approach is the same as above except that now I must loop create nodes until outside of the map.
*/

foreach (Char key in freqLocations.Keys)
{
	foreach (int[] pos1 in freqLocations[key])
	{
		foreach (int[] pos2 in freqLocations[key])
		{
			if (Enumerable.SequenceEqual(pos1, pos2))
			{
				continue;
			}
			
			int x = pos1[0];
			int y = pos1[1];
			int n = 0;
			
			while (isInMap(nodeArray, [x, y]))
			{
				x = pos1[0] - n * (pos2[0] - pos1[0]);
				y = pos1[1] - n * (pos2[1] - pos1[1]);
				updateNodeArray(nodeArray, [x, y]);
				n++;
			}
			
			x = pos2[0];
			y = pos2[1];
			
			while (isInMap(nodeArray, [x, y]))
			{
				x = pos2[0] + n * (pos2[0] - pos1[0]);
				y = pos2[1] + n * (pos2[1] - pos1[1]);
				updateNodeArray(nodeArray, [x, y]);
				n++;
			}

		}
	}
}

Console.WriteLine("Solution 2 = " + nodeArray.Cast<int>().Sum());
