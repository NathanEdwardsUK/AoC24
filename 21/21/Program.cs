using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/21/21/input.txt");
string input = sr.ReadToEnd();
string[] codes = input.Split("\n");

/* 
--------------- Part 1 --------------- 
Make functions which outputs a sequence of movements needed to press a button starting from another button
7 8 9     ^ A
4 5 6   < v >
1 2 3
  0 A
  
Observation: Any sequence of arrows can be rearranged without affecting the length of the sequence e.g. >>^ = >^> = ^>>
Observation: It's more efficient to press the same button repeatedly and then move on e.g. >>^ is more efficient to press than >^>

So I after each iteration of sequence building rearrange the sequence to put all identical characters next to each other (in between A's)
*/

// string pressButton(char _startChar, char _endChar, string padName)
// {
// 	string _sequence = "";
// 	char[,] pad;
	
// 	(int x1, int y1) = (-1, -1);
// 	(int x2, int y2) = (-1, -1);
	
// 	if (padName == "keypad")
// 	{
// 		pad = new char[,] {{'7', '8', '9'}, {'4', '5', '6'}, {'1', '2', '3'}, {'_', '0', 'A'}};
// 	}
// 	else if (padName == "controlPad")
// 	{
// 		pad = new char[,] {{'_', '^', 'A'}, {'<', 'v', '>'}};	
// 	}
// 	else
// 	{
// 		throw new Exception ("invalid padName");
// 	}
	
// 	for (int i = 0; i < pad.GetLength(0); i++)
// 	{
// 		for (int j = 0; j < pad.GetLength(1); j++)
// 		{
// 			if (_startChar == pad[i, j])
// 			{
// 				(x1, y1) = (i, j);	
// 			}
			
// 			if (_endChar == pad[i, j])
// 			{
// 				(x2, y2) = (i, j);	
// 			}	
// 		}
// 	}
	
// 	if (x1 == -1 || y1 == -1 || x2 == -1 || y2 == -1)
// 	{
// 		throw new Exception("char not found");
// 	}
	
// 	(int dx, int dy) = (x2 - x1, y2 - y1);
	
// 	if (dy > 0)
// 	{
// 		_sequence += String.Join("", Enumerable.Repeat(">", dy));
// 	}
// 	else if (dy < 0)
// 	{
// 		_sequence += String.Join("", Enumerable.Repeat("<", -dy));
// 	}
	
// 	if (dx > 0)
// 	{
// 		_sequence += String.Join("", Enumerable.Repeat("v", dx));
// 	}
// 	else if (dx < 0)
// 	{
// 		_sequence += String.Join("", Enumerable.Repeat("^", -dx));
// 	} 
	
// 	return _sequence + "A";
// }

string pressKeyPad(char _startChar, char _endChar)
{
	string _sequence = "";
	char[,] pad = {{'7', '8', '9'}, {'4', '5', '6'}, {'1', '2', '3'}, {'_', '0', 'A'}};
	
	(int x1, int y1) = (-1, -1);
	(int x2, int y2) = (-1, -1);
	
	for (int i = 0; i < pad.GetLength(0); i++)
	{
		for (int j = 0; j < pad.GetLength(1); j++)
		{
			if (_startChar == pad[i, j])
			{
				(x1, y1) = (i, j);	
			}
			
			if (_endChar == pad[i, j])
			{
				(x2, y2) = (i, j);	
			}	
		}
	}
	
	if (x1 == -1 || y1 == -1 || x2 == -1 || y2 == -1)
	{
		throw new Exception("char not found");
	}
	
	(int dx, int dy) = (x2 - x1, y2 - y1);
	
	if (dx < 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("^", -dx));
	} 
	
	if (dy > 0)
	{
		_sequence += String.Join("", Enumerable.Repeat(">", dy));
	}
	
	if (dy < 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("<", -dy));
	}
	
	if (dx > 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("v", dx));
	}
	
	return _sequence + "A";
}

string pressControlPad(char _startChar, char _endChar)
{
	string _sequence = "";
	char[,] pad = {{'_', '^', 'A'}, {'<', 'v', '>'}};
	
	(int x1, int y1) = (-1, -1);
	(int x2, int y2) = (-1, -1);
	
	for (int i = 0; i < pad.GetLength(0); i++)
	{
		for (int j = 0; j < pad.GetLength(1); j++)
		{
			if (_startChar == pad[i, j])
			{
				(x1, y1) = (i, j);	
			}
			
			if (_endChar == pad[i, j])
			{
				(x2, y2) = (i, j);	
			}	
		}
	}
	
	if (x1 == -1 || y1 == -1 || x2 == -1 || y2 == -1)
	{
		throw new Exception("char not found");
	}
	
	(int dx, int dy) = (x2 - x1, y2 - y1);
	
	if (dy < 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("<", -dy));
	}
	
	if (dx > 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("v", dx));
	}
	
	if (dy > 0)
	{
		_sequence += String.Join("", Enumerable.Repeat(">", dy));
	}
	
	if (dx < 0)
	{
		_sequence += String.Join("", Enumerable.Repeat("^", -dx));
	} 
	
	return _sequence + "A";
}

string getButtonSequence(string _code, bool isKeypad)
{
	char _startCh = 'A';
	string _sequence = "";
	// Console.WriteLine("Code = " + _code);

	foreach (char _nextChar in _code)
	{
		if (isKeypad)
		{
			_sequence += pressKeyPad(_startCh, _nextChar);
		}
		else
		{
			_sequence += pressControlPad(_startCh, _nextChar);
		}
		// Console.WriteLine(pressButton(_startCh, _nextChar, padName));
		_startCh = _nextChar;
	}
	// Console.WriteLine("Sequence = " + _sequence);
	// string _optimisedSequence = optimiseSequence(_sequence);
	return _sequence;
}

string optimiseSequence(string _sequence)
{
	string newSequence = "";
	int i = 0;
	
	while(i < _sequence.Length)
	{
		char c = _sequence.ElementAt(i);
		
		if (c == 'A')
		{
			newSequence += 'A';
			i++;
		}
		else
		{
			Dictionary<char, int> charDict = [];
			int j = i;
			
			// Make a dictionary counting all the different charachters up until the next A in the string
			while (j < _sequence.Length)
			{
				char c2 = _sequence.ElementAt(j);
				if (c2 == 'A')
				{
					break;
				}
				
				if (charDict.Keys.Contains(c2))
				{
					charDict[c2]++;
				}
				else
				{
					charDict[c2] = 1;
				}
				
				j++;
			}
			
			foreach (char key in charDict.Keys)
			{
				newSequence += String.Join("", Enumerable.Repeat(key, charDict[key]));	
			}
			
			i = j;
		}
	}
	
	return newSequence;
}

long solution1 = 0;

foreach (string code in codes)
{
	int num = Int32.Parse(code.Substring(0,3));
	string sequence = getButtonSequence(code, true);
	sequence = getButtonSequence(sequence, false);
	sequence = getButtonSequence(sequence, false);	
	Console.WriteLine("length = " + sequence.Length + ", num = " + num + ", sequence = " + sequence);
	solution1 += num * sequence.Length;
}
	
Console.WriteLine("Solution1 = " + solution1);


// string cd = "980";

// string ssequence = getButtonSequence(cd, "keypad");
// Console.WriteLine(ssequence);
// ssequence = getButtonSequence(ssequence, "controlPad");
// Console.WriteLine(ssequence);
// ssequence = getButtonSequence(ssequence, "controlPad");
// Console.WriteLine(ssequence);

// ^A^^<<A>>AvvvA

// <A>A<AAv<AA^>>AvAA^Av<AAA^>A

// <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A
// v<<A^>>AvA^Av<<A^>>AAv<A<A^>>AA<Av>AA^Av<A^>AA<A>Av<A<A^>>AAA<Av>A^A

/* 
7 8 9   ^A
4 5 6  <v>
1 2 3
  0 A
*/