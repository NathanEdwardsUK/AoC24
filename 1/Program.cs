using System.IO;
string line = "";
List<int> list1 = []; 
List<int> list2 = []; 
List<int> listDiff = []; 

StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/1/input.txt");

line = sr.ReadLine();
while (line != null)
{
	string[] splitString = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
	list1.Add(Int32.Parse(splitString[0]));
	list2.Add(Int32.Parse(splitString[1]));
	line = sr.ReadLine();
}

List<int> sortedList1 = new List<int>(list1);
List<int> sortedList2 = new List<int>(list2);

sortedList1.Sort();
sortedList2.Sort();

for (int i = 0; i <list1.Count; i++)
{
	int diff = Math.Abs(sortedList1[i] - sortedList2[i]);
	listDiff.Add(diff); 
	Console.WriteLine($"{i} - {sortedList1[i]};{sortedList2[i]} - {listDiff[i]}");
}

int sum1 = listDiff.Sum();
Console.WriteLine($"Solution part 1 = {sum1}");

/* --------------- Part 2 --------------- */

int sum2 = 0;

foreach (int i in list1)
{
	int count = 0;
	
	foreach (int j in list2)
	{
		if (i == j)
		{
			count++;
		}
	}
	
	sum2 += i*count;
}

Console.WriteLine($"Solution part 2 = {sum2}");

