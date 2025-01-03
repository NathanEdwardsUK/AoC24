using System.Numerics;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/21/21/input.txt");
string input = sr.ReadToEnd();
string[] codes = input.Split("\n");
Dictionary<string, List<string>> cache = [];
Dictionary<string, List<string>> cache3 = [];
Dictionary<string, BigInteger> cache2 = [];

/* 
--------------- Part 1 --------------- 
7 8 9     ^ A
4 5 6   < v >
1 2 3
  0 A
*/

List<string> pressButton(char _startChar, char _endChar, string padName)
{
	string key = "" + _startChar + _endChar;
	
	if (cache3.Keys.Contains(key))
	{
		return cache3[key];
	}
	
	List<string> possibleSequences = [];
	string possibleSeq;
	char[,] pad;
	
	(int x1, int y1) = (-1, -1);
	(int x2, int y2) = (-1, -1);
	
	if (padName == "keypad")
	{
		pad = new char[,] {{'7', '8', '9'}, {'4', '5', '6'}, {'1', '2', '3'}, {'_', '0', 'A'}};
	}
	else if (padName == "controlPad")
	{
		pad = new char[,] {{'_', '^', 'A'}, {'<', 'v', '>'}};	
	}
	else
	{
		throw new Exception ("invalid padName");
	}
	
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
	string yChar = "";
	string xChar = "";
	
	if (dy > 0)
	{
		yChar = ">";
	}
	else if (dy < 0)
	{
		yChar = "<";
	}
	
	if (dx > 0)
	{
		xChar = "v";
	}
	else if (dx < 0)
	{
		xChar = "^";
	} 
	
	// if the string is only made of one character then there is only one permutation
	if (yChar == "")
	{
		possibleSequences = [String.Join("", Enumerable.Repeat(xChar, Math.Abs(dx))) + "A"];
	}
	else if (xChar == "")
	{
		possibleSequences =  [String.Join("", Enumerable.Repeat(yChar, Math.Abs(dy))) + "A"];
	}
	// if the string has 2 arrow chars then there could be 2 permutations. 
	// Have to check first that one doesnt lead to the empty space
	else if (pad[x2, y1] == '_')
	{
		possibleSeq = String.Join("", Enumerable.Repeat(yChar, Math.Abs(dy)))
					+ String.Join("", Enumerable.Repeat(xChar, Math.Abs(dx))) + "A";
		possibleSequences = [possibleSeq];
	}
	else if (pad[x1, y2] == '_')
	{
		possibleSeq = String.Join("", Enumerable.Repeat(xChar, Math.Abs(dx)))
					+ String.Join("", Enumerable.Repeat(yChar, Math.Abs(dy))) + "A";
		possibleSequences = [possibleSeq];
	}
	else
	{
		possibleSequences.Add(String.Join("", Enumerable.Repeat(xChar, Math.Abs(dx)))
							  + String.Join("", Enumerable.Repeat(yChar, Math.Abs(dy))) 
							  + "A");
		
		possibleSequences.Add(String.Join("", Enumerable.Repeat(yChar, Math.Abs(dy)))
							  + String.Join("", Enumerable.Repeat(xChar, Math.Abs(dx))) 
							  + "A");
	}
	
	cache3[key] = possibleSequences;
	return possibleSequences;
}

BigInteger getShortestSequenceLength(char _startCh, string _code, string _padName, int iterations)
{
	if (iterations == 0)
	{
		return _code.Length;
	}
	
	if (_code == "")
	{
		return 0;
	}
	
	char _nextCh = _code.ElementAt(0);
	string key = "" + iterations + ":" + _startCh + _nextCh;
	BigInteger charInstructionsLength;
	
	if (cache2.ContainsKey(key))
	{
		charInstructionsLength = cache2[key];
	}
	else 
	{
		List<string> sequences = pressButton(_startCh, _nextCh, _padName);
		BigInteger minLength = Int64.MaxValue;
		
		foreach (string sequence in sequences)
		{
			BigInteger sequenceLength = getShortestSequenceLength('A', sequence, "controlPad", iterations - 1);
			minLength = BigInteger.Min(minLength, sequenceLength);
		}
		
		charInstructionsLength = minLength;
		cache2[key] = charInstructionsLength;
	}
	
	string _remainingCode = _code.Substring(1);
	BigInteger codeInstructionLength = getShortestSequenceLength(_nextCh, _remainingCode, _padName, iterations);
	return charInstructionsLength + codeInstructionLength;
}

// function for debugging
string reverseSequence(string _sequence)
{
	string _code = "";
	char[,] pad = {{'_', '^', 'A'}, {'<', 'v', '>'}};
	(int x, int y) = (0, 2);
	
	foreach (char move in _sequence)
	{
		if (move == '>')
		{
			y++;
		}
		else if(move == 'v')
		{
			x++;
		}
		else if(move == '<')
		{
			y--;
		}
		else if(move == '^')
		{
			x--;
		}
		else if(move == 'A')
		{
			_code += pad[x, y];
		}
		else
		{
			throw new Exception("invalid char in sequence");
		}
		
		if ((x == 0 && y == 0) || x < 0 || y < 0 || x > 1 || y > 2)
		{
			throw new Exception("invalid sequence applied");
		}
	}
	
	return _code;
}

BigInteger solution1 = 0;
BigInteger solution2 = 0;

foreach (string code in codes)
{
	Console.WriteLine(code);
	int num = Int32.Parse(code.Substring(0,3));
	BigInteger len1 = getShortestSequenceLength('A', code, "keypad", 3);
	BigInteger len2 = getShortestSequenceLength('A', code, "keypad", 26);
	// Console.WriteLine("length = " + len1 + ", num = " + num);
	solution1 += num * len1;
	solution2 += num * len2;
}
	
Console.WriteLine("Solution1 = " + solution1);
Console.WriteLine("Solution2 = " + solution2);


