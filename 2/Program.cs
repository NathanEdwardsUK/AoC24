using System.IO;
string line = "";
List<List<int>> inputList = []; 
List<bool> safeList = []; 

List<int> stringToIntList(string str)
{
	string[] splitString = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
	
	List<int> intList = [];
	foreach (string s in splitString)
	{
		intList.Add(Int32.Parse(s));
	}
	
	return intList;
}

void printList(List<int> list)
{
	list.ForEach(i => Console.Write("{0} ", i));
	Console.Write("\n");	
}

List<int> removeElement(List<int> intList, int ind)
{
	List<int> newList = new List<int>(intList); 
	newList.RemoveAt(ind);
	return newList;
}

bool isSafe(List<int> list, bool removeBadLevel)
{
	if (list.Count == 1)
	{
		return true;
	}

	int increasingSign = Math.Sign(list[1] - list[0]);
	
	for (int i = 1; i < list.Count; i++)
	{
		if (increasingSign * (list[i] - list[i-1]) <= 0)
		{
			if (removeBadLevel)
			{
				return isSafe(removeElement(list, i), false);
			}
			return false;
		}
		
		if (increasingSign * (list[i] - list[i-1]) >= 4)
		{
			if (removeBadLevel)
			{
				return isSafe(removeElement(list, i), false);
			}
			return false;
		}
	}
	return true;
}


StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/2/input.txt");
int i = 0;
int safeCount1 = 0;
int safeCount2 = 0;

while ((line = sr.ReadLine()) != null)
{
	Console.Write(i + " - ");
	i++;
	
	List<int> intList = stringToIntList(line);
	printList(intList);
	
	bool safe1 = isSafe(intList, false);
	if (safe1)
	{
		safeCount1++;	
	}
	Console.Write($"safe under rule 1 {safe1}\n");

	bool safe2 = false;
	for (int j = 0; j < intList.Count; j++)
	{
		if (safe2 == true)
		{
			continue;
		}
		else
		{
			safe2 = isSafe(removeElement(intList,j), false);
		}
	}
	
	if (safe2)
	{
		safeCount2++;	
	}
	Console.Write($"safe under rule 2 {safe2}\n");
}

Console.Write($"Solution part 1 = {safeCount1}\n");
Console.Write($"Solution part 2 = {safeCount2}\n");