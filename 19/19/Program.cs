using System.Numerics;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/19/19/input.txt");
string input = sr.ReadToEnd();
string[] towels = input.Split("\n\n")[0].Split(", ");
string[] patterns = input.Split("\n\n")[1].Split("\n");
Console.WriteLine($"towels = " + String.Join(",", towels));

// Will try a BFS method, find all occurrances of every towel in a pattern, first verify that every char in
// each pattern is represented by one occurrance, then find a way to verify that the pattern can be built

Dictionary<string, List<Match>> findTowelMatches(string[] _towels, string _pattern)
{
	Dictionary<string, List<Match>> towelMatchesDict = new();
	
	foreach (string towel in _towels)
	{
		// MatchCollection matches = Regex.Matches(_pattern, towel);
		List<Match> matches= [];
		Regex reg = new Regex(towel);
		Match match = reg.Match(_pattern);
		
		while (match.Success)
		{
			matches.Add(match);
			match = reg.Match(_pattern, match.Index + 1);
		}
		
		if (matches.Count > 0)
		{
			towelMatchesDict.Add(towel, matches);
		}
	}
	
	return towelMatchesDict;
}

BigInteger countWaysToBuildPattern(string[] _towels, string _pattern)
{
	Dictionary<string, List<Match>> towelMatchesDict = findTowelMatches(_towels, _pattern);
	// Init an array of all 0s same length as the pattern string. 
	int[] charRepresented = new int[_pattern.Length];
	BigInteger _numWaysToBuildPattern = 0;
	List<int[]> matchPositions = [];
	
	// For each match of each towel add 1 to all the positions where the towel is matched in the pattern
	foreach(string towel in towelMatchesDict.Keys)
	{
		foreach(Match match in towelMatchesDict[towel])
		{
			int s = match.Index;
			int l = match.Length;
			matchPositions.Add([s, s + l]);
			
			for (int i = s; i < s + l; i++)
			{
				charRepresented[i]++;
			}
		}
	}
	
	// If there are any 0s left in the charRepresented array then the pattern is certainly not possible
	if (charRepresented.Min() == 0)
	{
		return 0;
	}
	
	// If we get here we still need to check that the string can be constructed by the towels without overlap.
	// I will build an int[2] list of start and end indexes of towels, sort that list and then try something
	List<int[]> uncheckedPositions = matchPositions.OrderBy(p => p[0]).ThenBy(p => p[1]).ToList();
	
	// Now we have a sorted list of nodes I will try to solve this using a version of Dijkstra's again
	// i.e. starting from the first matchPosition, iterate over all nodes and see which can be reached from it
	// move that node to the checked nodes list then repeat
	List<int[]> checkedPositions = [];
	List<BigInteger> scores = Enumerable.Repeat((BigInteger) 0, uncheckedPositions.Count).ToList();
	List<BigInteger> finalScores = [];
	
	for (int i = 0; i < scores.Count; i++)
	{
		if (uncheckedPositions[i][0] == 0)
		{
			scores[i] = 1;
		}
	}
	
	// By the end of this every position that is reachable will have non 0 score. So need a position that
	// ends at the end of the pattern with non zero score
	while (uncheckedPositions.Count > 0)
	{
		int[] pos = uncheckedPositions[0];
		int i = 0;
		while (i < uncheckedPositions.Count && uncheckedPositions[i][0] <= pos[1])
		{
			if (uncheckedPositions[i][0] == pos[1])
			{
				scores[i] += scores[0];
			}
			i++;
		}
		
		checkedPositions.Add(pos);
		finalScores.Add(scores[0]);
		uncheckedPositions.RemoveAt(0);
		scores.RemoveAt(0);
	}
	
	// Now check that there is at least one position which ends at the end of the pattern and has non zero score
	for (int i = 0; i < checkedPositions.Count; i++)
	{
		if (checkedPositions[i][1] == _pattern.Length && finalScores[i] > 0)
		{
			_numWaysToBuildPattern += finalScores[i];
		}
	}
	
	return _numWaysToBuildPattern;
}


int possiblePatterns = 0;
BigInteger solution2 = 0;

foreach (string pattern in patterns)
{
	BigInteger numWaysToBuildPattern = countWaysToBuildPattern(towels, pattern);
	solution2 += numWaysToBuildPattern;
	
	if (numWaysToBuildPattern > 0)
	{
		possiblePatterns++;
	}
	
	// Console.WriteLine($"Pattern = {pattern}, valid = {isValid}");
}

Console.WriteLine("Solution 1 = " + possiblePatterns);
Console.WriteLine("Solution 2 = " + solution2);
