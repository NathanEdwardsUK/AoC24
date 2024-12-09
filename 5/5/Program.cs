using System;
using System.IO;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/5/5/input.txt");

string inputStr = sr.ReadToEnd();
string rulesStr = inputStr.Split("\n\n")[0];
string updatesStr = inputStr.Split("\n\n")[1];

// First parse rules
string[] rulesSplitStr = rulesStr.Split("\n");
/* List<int> rules = new string[rulesSplitStr.Count(),2]; */
List<List<int>> rules = [];
for (int i = 0; i < rulesSplitStr.Count(); i++)
{
	string[] ruleSplitStr = rulesSplitStr[i].Split("|");
	rules.Add([Int32.Parse(ruleSplitStr[0]), Int32.Parse(ruleSplitStr[1])]);
	Console.WriteLine($"{i} - " + ruleSplitStr[0] + "|" + ruleSplitStr[1]);
}

// Then parse updates
string[] updatesStrSplit = updatesStr.Split("\n");
List<List<int>> updates = [];
for (int i = 0; i < updatesStrSplit.Count(); i++)
{
	string[] strList = updatesStrSplit[i].Split(",");
	List<int> intList = [];
	for (int j = 0; j < strList.Count(); j++)
	{
		intList.Add(Int32.Parse(strList[j]));
	}
	updates.Add(intList);
	
	Console.WriteLine($"{i} - " + string.Join(",", updates[i]));
}


bool checkUpdateFollowsRule(List<int> update, List<int> rule)
{
	// First check if both rule numbers are in the update
	if (!(update.Contains(rule[0]) && update.Contains(rule[1])))
	{
		return true;
	}
	
	// Now cycle through each update number and check that the first hit is the first rule number. This means the rule is obeyed
	foreach (int u in update)
	{
		if (u == rule[0])
		{
			return true;
		}
		
		if (u == rule[1])
		{
			return false;
		}
	}
	
	throw new Exception("Problem found in checking update.");
}

int sumMiddleNumbers1 = 0;
// Keep track of the incorrectly ordered updates for part 2
List<List<int>> badUpdates = [];
Console.WriteLine($"\nChecking which updates are correctly ordered.");

// Now cycle through each update and check each rule and hope that this is not too inefficient.
for (int i = 0; i < updates.Count(); i++)
{
	bool updateIsInOrder = true;
	List<int> update = updates[i];
	
	for (int j = 0; j < rules.Count(); j++)
	{
		List<int> rule = rules[j];
		if (!checkUpdateFollowsRule(update, rule))
		{
			updateIsInOrder = false;
			Console.Write($"Rule {j} = " + string.Join("|", rules[j]) + " broken by update ");
			Console.WriteLine($"{i} - " + string.Join(",", updates[i]));
			badUpdates.Add(update);
			break;
		}
	}
	
	if (updateIsInOrder)
	{
		sumMiddleNumbers1 += update[update.Count() / 2];
		Console.WriteLine($"{i} - " + string.Join(",", updates[i]));
	}
	
}

Console.WriteLine("Solution 1 = " + sumMiddleNumbers1);

/* --------------- Part 2 --------------- */
/* Lets try iterating through each bad update and each rule, then for any rule which is broken switch the 2 numbers around.
If that doesn't work can do something more efficient.*/

List<int> fixUpdate(List<int> update, List<int> rule)
{
	// Find the index of the first instance of rule[0] and the last instance of rule[1] and switch them.
	// Assuming the update breaks the rule then index(rule[1]) must be < index(rule[0])
	int ind1 = update.IndexOf(rule[1]);
	int ind2 = update.IndexOf(rule[0]);
	update[ind1] = rule[0];
	update[ind2] = rule[1];
	return update;
}

List<List<int>> fixedUpdates = [];
int sumMiddleNumbers2 = 0;

for (int i = 0; i < badUpdates.Count(); i++)
{
	List<int> update = badUpdates[i];
	
	for (int j = 0; j < rules.Count(); j++)
	{
		List<int> rule = rules[j];
		if (!checkUpdateFollowsRule(update, rule))
		{
			update = fixUpdate(update, rule);
			Console.Write($"Rule {j} = " + string.Join("|", rules[j]) + " broken by update ");
			Console.WriteLine($"{i} - " + string.Join(",", updates[i]));
			j = 0;
		}
	}
	
	fixedUpdates.Add(update);
	sumMiddleNumbers2 += update[update.Count() / 2];
	Console.WriteLine($"{i} - " + string.Join(",", updates[i]));
}

Console.WriteLine("Solution 2 = " + sumMiddleNumbers2);



