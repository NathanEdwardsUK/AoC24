Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/25/25/input.txt");
string input = sr.ReadToEnd();
string[] splitInput = input.Split("\n\n");

char[,] stringToCharArray(string str)
{
	string[] splitStr = str.Split("\n");
	int numRows = splitStr.Count();
	int numCols = splitStr[0].Length;
	char[,] array = new char[numRows, numCols];

	// Convert map into 2d char array
	for (int i = 0; i < numRows; i++)
	{
		for (int j = 0; j < numCols; j++)
		{
			array[i, j] = splitStr[i][j];
		}
	}
	return array;
}

List<int[]> locks = [];
List<int[]> keys = [];

foreach (string graph in splitInput)
{
	char[,] chArr = stringToCharArray(graph);
	bool isLock = (chArr[0, 0] == '#');
	
	int[] heights = [0, 0, 0, 0, 0];
	
	for (int i = 0; i < 5; i++)
	{
		for (int j = 0; j < 7; j++)
		{
			
			if (isLock && chArr[j, i] == '.')
			{
				heights[i] = j - 1;
				break;
			}
			else if (!isLock && chArr[j, i] == '#')
			{
				heights[i] = 6 - j;
				break;
			}
		}
	}
	
	if (isLock)
	{
		locks.Add(heights);
	}
	else
	{
		keys.Add(heights);
	}
}

/* 
--------------- Part 1 --------------- 
I definitely over complicated this code expecting a hard part 2. Luckily it was a christmas present of a day. Finally finished :)
*/

bool tryKeyInLock(int[] _key, int[] _lock)
{
	for (int i = 0; i < _key.Length; i++)
	{
		if (_key[i] + _lock[i] > 5)
		{
			return false;
		}
	}

	return true;	
}

int count = 0;

foreach (int[] l in locks)
{
	foreach (int[] k in keys)
	{
		// Console.WriteLine("lock = " + String.Join(",", l) + " key = " + String.Join(",", k));
		
		if (tryKeyInLock(k, l))
		{
			count++;
		}
	}
}

Console.WriteLine("solution 1 = " + count);