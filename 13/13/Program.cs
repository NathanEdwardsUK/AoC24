using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/13/13/input.txt");

List<long[]> A = [];
List<long[]> B = [];
List<long[]> P = [];

// First parse data

int line_num = 0;
foreach (string line in sr.ReadToEnd().Split("\n"))
{
	Console.WriteLine(line);
	if (line == "")
	{
		continue;
	}
	
	Match[] matches = Regex.Matches(line, @"\d+").ToArray();
	if (line_num % 3 == 0)
	{
		A.Add([Int64.Parse(matches[0].Value), Int64.Parse(matches[1].Value)]);	
	}
	else if (line_num % 3 == 1)
	{
		B.Add([Int64.Parse(matches[0].Value), Int64.Parse(matches[1].Value)]);	
	}
	else
	{
		P.Add([Int64.Parse(matches[0].Value), Int64.Parse(matches[1].Value)]);	
	}
	
	line_num++;
}

/* --------------- Part 1 --------------- */
/*
Minimise 
(1) 3Na + Nb
Subject to 
(2) Na * Ax + Nb * Bx = Px
(3) Na * Ay + Nb * By = Py
and 
0 <= Na, Nb <= 100

Set of 2 simultaneous equations with 2 degrees of freedom has only 1 solution
Nb = (PyAx - PxAy) / (ByAx - AyBx)
Na = (Px - BxNb) / Ax
*/

long totalCost = 0;
long offset = 10000000000000;

for (int i = 0; i < P.Count; i++)
{
	long Ax = A[i][0];
	long Ay = A[i][1];
	long Bx = B[i][0];
	long By = B[i][1];
	long Px = P[i][0] + offset;
	long Py = P[i][1] + offset;
	Console.WriteLine($"\nA = [{Ax},{Ay}], B = [{Bx},{By}], P = [{Px},{Py}],");
	
	
	long Nb = (Py * Ax - Px * Ay) / (By * Ax - Ay * Bx);
	long Na = (Px - Bx * Nb) / Ax;
	long cost = 3 * Na + Nb;
	
	// if (0 <= Na && Na <= 100 && 0 <= Nb && Nb <= 100 && Na * Ax + Nb * Bx == Px)
	if (0 <= Na && 0 <= Nb && Na * Ax + Nb * Bx == Px && Na * Ay + Nb * By == Py)
	{
		Console.WriteLine($"Na = {Na}, Nb = {Nb}, Cost = {cost}");
		totalCost += cost;
	}
}

Console.WriteLine($"solution = {totalCost}");

/* --------------- Part 2 --------------- */
/*
Simply added an offset parameter above and set it to 10^13 then run the script again
*/

