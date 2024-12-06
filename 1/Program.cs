using System.IO;
string line = "";
List<int> list1 = []; 
List<int> list2 = []; 
List<int> listDiff = []; 

StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/1/input.txt");

line = sr.ReadLine();
while (line != null)
{
	string[] splitString = line.Split(" ");
	List<int> intList = [];

	foreach (var s in splitString)
	{
		int x = 0;
		bool isInt = Int32.TryParse(s, out x);
		
		if (isInt)
		{
			intList.Add(x);
		}
	}
	list1.Add(intList[0]);
	list2.Add(intList[1]);

	line = sr.ReadLine();
}

list1.Sort();
list2.Sort();

for (int i = 0; i <list1.Count; i++)
{
	int diff = Math.Abs(list1[i] - list2[i]);
	listDiff.Add(diff); 
	Console.WriteLine($"{i} - {list1[i]};{list2[i]} - {listDiff[i]}");
}

int sum = listDiff.Sum();
Console.WriteLine(sum);

