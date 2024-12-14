using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/14/14/input.txt");
string[] lines = sr.ReadToEnd().Split("\n");
var P = new long[lines.Length][];
var V = new long[lines.Length][];

// First parse data
for (int i = 0; i < lines.Length; i++)
{
	string line = lines[i];
	Console.WriteLine(line);
	if (line == "")
	{
		continue;
	}
	
	Match[] matches = Regex.Matches(line, @"(-)?\d+").ToArray();
	P[i] = [Int64.Parse(matches[0].Value), Int64.Parse(matches[1].Value)];	
	V[i] = [Int64.Parse(matches[2].Value), Int64.Parse(matches[3].Value)];	
}

/* --------------- Part 1 --------------- */
/*
*/

long maxTime = 100;
long mapHeight = 101;
long mapWidth = 103;
var PFinal = new long[lines.Length][];


/*
4 mod 11 = 4
1 mod 11 = 1
0 mod 11 = 0
-1 mod 11 = 10 
*/

long mod(long x, long m)
{
	long r = x % m;
	
	if (r < 0)
	{
		r += m;
	}
	
	return r;
}

for (int i = 0; i < P.Length; i++)
{
	// The puzzle inverts x and y in the input compared to usual convention so fixing that here
	(long x, long y) = (P[i][1], P[i][0]);
	(long vx, long vy) = (V[i][1], V[i][0]);
	
	long xFinal = mod(x + vx * maxTime , mapWidth);
	long yFinal = mod(y + vy * maxTime, mapHeight);
	PFinal[i] = [yFinal, xFinal];
	
	Console.WriteLine($"i = {i}, P = [{y},{x}], V = [{vy},{vx}], PF = [{yFinal},{xFinal}]");
}

long[] quadrantsCount = [0, 0, 0, 0];

long[] countRobotsPerQuadrant(long[][] Positions, long size)
{
	long[] count = [0, 0, 0, 0];
	for (int i = 0; i < P.Length; i++)
	{
		(long x, long y) = (Positions[i][1], Positions[i][0]);
		
		// TL
		if (x < (mapWidth - 1) / 2 && y < (mapHeight - 1) / 2
			&& x > (mapWidth - 1) / 2 - size && y > (mapHeight - 1) / 2 - size)
		{
			count[0]++;
		}
		// TR
		else if (x < (mapWidth - 1) / 2 && y > (mapHeight - 1) / 2
				&& x > (mapWidth - 1) / 2 - size && y < (mapHeight - 1) / 2 + size)
		{
			count[1]++;
		}
		// BL
		else if (x > (mapWidth - 1) / 2 && y < (mapHeight - 1) / 2
				&& x < (mapWidth - 1) / 2 + size && y > (mapHeight - 1) / 2 - size)
		{
			count[2]++;
		}
		// BR
		else if (x > (mapWidth - 1) / 2 && y > (mapHeight - 1) / 2
				&& x < (mapWidth - 1) / 2 + size && y < (mapHeight - 1) / 2 + size)
		{
			count[3]++;
		}
	}
	return count;
}

quadrantsCount = countRobotsPerQuadrant(PFinal, 9999);

long solution1 = quadrantsCount[0] * quadrantsCount[1] * quadrantsCount[2] * quadrantsCount[3];
showRobots(PFinal);
Console.WriteLine("Solutions 1 = " + solution1);

/* --------------- Part 2 --------------- */

int maxT = 10000;
for (int t = 0; t < maxT; t++)
{	
	for (int i = 0; i < P.Length; i++)
	{
		// The puzzle inverts x and y in the input compared to usual convention so fixing that here
		(long x, long y) = (P[i][1], P[i][0]);
		(long vx, long vy) = (V[i][1], V[i][0]);
		
		long xFinal = mod(x + vx * t , mapWidth);
		long yFinal = mod(y + vy * t, mapHeight);
		PFinal[i] = [yFinal, xFinal];
		
		// Console.WriteLine($"i = {i}, P = [{y},{x}], V = [{vy},{vx}], PF = [{yFinal},{xFinal}]");
	}
	
	long xMid = (mapWidth - 1) / 2;
	long yMid = (mapHeight - 1) / 2;
	List<long[]> midCoords = [[xMid, yMid], [xMid + 1, yMid], [xMid - 1, yMid]];
	// List<long[]> midCoords = [[yMid, xMid]];
	// if (checkForCoords(midCoords, PFinal))
	if (checkForContinuousLines(PFinal, 8))
	{
		Console.WriteLine(t);
		showRobots(PFinal);
		Console.WriteLine("\n");
		// long[] q = countRobotsPerQuadrant(PFinal, 8);
	
		// if (q.Min() >= 5)
		// {
		// 	if ((q[0] == q[1] && q[2] == q[3]) 
		// 	|| (q[1] == q[2] && q[0] == q[3]))
		// 	{
		// 		Console.WriteLine(t);
		// 		showRobots(PFinal);
		// 		Console.WriteLine("\n");
		// 	}
		// }
	}
}

bool checkForCoords(List<long[]> coords, long[][] positions)
{
	foreach(long[] coord in coords)
	{
		bool found = false;
		foreach(long[] pos in positions)
		{
			if (coord[0] == pos[0] && coord[1] == pos[1])
			{
				found = true;
				break;
			}
		}
		
		if (found)
		{
			continue;
		}
		else
		{
			return false;
		}
	}
	return true;
}

bool checkForCoord(long[] coord, long[][] positions)
{
	foreach(long[] pos in positions)
	{
		if (coord[0] == pos[0] && coord[1] == pos[1])
		{
			return true;
		}
	}
	return false;
}

bool checkForContinuousLines(long[][] positions, int len)
{
	foreach(long[] pos in positions)
	{
		int y = 1;
		while(checkForCoord([pos[0] + y, pos[1]], positions))
		{
			if(y == len)
			{
				return true;
			}
			y++;
		} 
	}

	return false;
}

void showRobots(long[][] positions)
{
	for (int i = 0; i < mapWidth; i++)
	{
		string line = "";
		for (int j = 0; j < mapHeight; j++)
		{
			string ch = " ";
			foreach (long[] p in positions)
			{
				if (p[0] == i && p[1] == j)
				{
					ch += "_";
					break;
				}
			}
			line += ch;
		}
		Console.WriteLine(line);
	}
}