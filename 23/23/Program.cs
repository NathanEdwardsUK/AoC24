using System.Numerics;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/23/23/input.txt");
string input = sr.ReadToEnd();
string[] splitStr = input.Split("\n");

Dictionary<string, List<string>> connections = [];
HashSet<string> computers = [];
HashSet<string> computerGroups = [];

for( int i = 0; i < splitStr.Length; i++)
{
	List<string> connection = splitStr[i].Split("-").ToList();
	connection.Sort(string.Compare);

	if (connections.ContainsKey(connection[0]))
	{
		connections[connection[0]].Add(connection[1]);
	}
	else
	{
		connections[connection[0]] = [connection[1]];
	}
	
	if (connections.ContainsKey(connection[1]))
	{
		connections[connection[1]].Add(connection[0]);
	}
	else
	{
		connections[connection[1]] = [connection[0]];
	}
	
	computers.Add(connection[0]);
	computers.Add(connection[1]);
}

List<string> orderedComputers = computers.ToList();
orderedComputers.Sort(string.Compare);

foreach (string comp in orderedComputers)
{	
	if (comp.ElementAt(0) == 't')
	{
		foreach (string comp2 in connections[comp])
		{
			foreach (string comp3 in connections[comp2])
			{
				if (comp.Equals(comp3))
				{
					continue;
				}
				
				if (connections[comp3].Contains(comp))
				{
					List<string> groupList = [comp, comp2, comp3];
					groupList.Sort();
					computerGroups.Add(String.Join("", groupList));
				}
			}
		}
	}
}

Console.WriteLine("Solution 1 = " + computerGroups.Count);

/* 
--------------- Part 2 --------------- 
There are 520 unique comps each one with 13 connected comps. There are 2^13  ~= 8000 subsets of 
each connection group, 500 * 8000 = 4,000,000 is brute force computable, especially with some logic to
cut dead ends short
*/

foreach (string comp in orderedComputers)
{
	List<string> gr = connections[comp];
	connections[comp].Add(comp);
	gr.Sort(string.Compare);
	connections[comp] = gr;
	Console.WriteLine(comp + " - " + String.Join(",", gr));
}
Console.WriteLine("");

List<string> totalIntersect(List<string> group)
{
	List<string> intersect = group;
	
	foreach (string comp in group)
	{
		intersect = intersect.Intersect(connections[comp]).ToList();
	}
	
	return intersect;
}

int maxGroupSize = 0;
HashSet<string> maxGroups = [];

void computeBiggestGroup(List<string> group, int startIdx)
{
	if (startIdx < group.Count){
		for (int i = startIdx; i < group.Count; i++)
		{
			string comp = group[i];
			List<string> group2 = connections[comp];
			List<string> intersect = group.Intersect(group2).ToList();
			
			if (intersect.Count < maxGroupSize)
			{
				continue;
			}
			
			computeBiggestGroup(intersect, i + 1);
		}
		
		return;
	}
	
	List<string> minimalIntersect = totalIntersect(group);
	
	// We reach here if startIdx = group.Count
	if (minimalIntersect.Count == maxGroupSize)
	{
		maxGroupSize = minimalIntersect.Count;
		maxGroups.Add(String.Join(",", minimalIntersect));
	}
	else if (minimalIntersect.Count > maxGroupSize)
	{
		maxGroupSize = minimalIntersect.Count;
		maxGroups = [String.Join(",", minimalIntersect)];
	}
}

foreach (string comp in orderedComputers)
{	
	computeBiggestGroup(connections[comp], 0);
}

Console.WriteLine("Max group = " + maxGroups.ElementAt(0) + " size = " + maxGroupSize);

