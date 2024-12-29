Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/17/17/input.txt");
string[] strArr = sr.ReadToEnd().Split("\n");

long A = Int64.Parse(strArr[0].Split(": ")[1]);
long B = Int64.Parse(strArr[1].Split(": ")[1]);
long C = Int64.Parse(strArr[2].Split(": ")[1]);
int[] program = Array.ConvertAll(strArr[4].Split(" ")[1].Split(","), s => int.Parse(s));

Console.WriteLine($"A = {A} \nB = {B} \nC = {C} \nProgram = {string.Join(",", program)} \n");

long combo(long A, long B, long C, long operand)
{
	if (0 <= operand && operand <= 3)
	{
		return operand;
	}
	else if (operand == 4)
	{
		return A;
	}
	else if (operand == 5)
	{
		return B;
	}
	else if (operand == 6)
	{
		return C;
	}
	else
	{
		throw new Exception("Invalid operand");
	}
}

// Return A, B, C, _out, pointer
(long, long, long, List<string>, long) applyOpcode(long A, long B, long C, List<string> _out, long opcode, long operand, long pointer)
{
	if (opcode == 0) // adv
	{
		A = (long) (A / Math.Pow(2, combo(A, B, C, operand)));
	}
	else if (opcode == 1) // bxl
	{
		B = B ^ operand;
	}
	else if (opcode == 2) // bst
	{
		B = combo(A, B, C, operand) % 8;
	}
	else if (opcode == 3) // jnz
	{
		if (A != 0)
		{
			if (pointer == operand)
			{
				throw new Exception("Unclear from instructions whether pointer should increment by 2 in this case");
			}
			
			pointer = operand;
			pointer -= 2;
		}
	}
	else if (opcode == 4) // bxc
	{
		B = B ^ C;
	}
	else if (opcode == 5) // out
	{
		_out.Add("" + combo(A, B, C, operand) % 8); 
	}
	else if (opcode == 6) // bdv
	{
		B = (long) (A / Math.Pow(2, combo(A, B, C, operand)));
	}
	else if (opcode == 7) // cdv
	{
		C = (long) (A / Math.Pow(2, combo(A, B, C, operand)));
	}
	else
	{
		throw new Exception ("Invalid opcode");
	}
	
	pointer += 2;
	return (A, B, C, _out, pointer);
}

string run(long A, long B, long C, int[] program)
{
	List<string> _out = [];
	long pointer = 0;
	
	while (pointer < program.Length - 1)
	{
		long opcode = program[pointer];
		long operand = program[pointer + 1];
		
		if (opcode == 7)
		{
			// Console.WriteLine($"_A = {A}, _B = {B}, _C = {C}");
		}
		
		(A, B, C, _out, pointer) = applyOpcode(A, B, C, _out, opcode, operand, pointer);
	}
	
	return String.Join(",", _out);
}

Console.WriteLine("Running program\n");
string outputStr = run(A, B, C, program);


Console.WriteLine($"A = {A} \nB = {B} \nC = {C}\n");
Console.WriteLine("Solution 1 = " + outputStr);

/* --------------- Part 2 --------------- */
/*
Now I need to find the lowest A st the program outputs itself. I think i can break this down in steps analytically.
Program = 2,4,1,6,7,5,4,6,1,4,5,5,0,3,3,0 

We only output something with opcode = 5. There is only one opcode 5 in the program, and it has operand 5 => output B % 8
Then at the end of the program as long as A is non 0 we go back to the start

So lets analyse the program and condense it into something easier to handle

(opcode, operand)

1. (2, 4) => B = A % 8 
2. (1, 6) => B = B XOR 6 ( = B XOR 000...000110)
3. (7, 5) => C = A / 2^B
4. (4, 6) => B = B XOR C
5. (1, 4) => B = B XOR 4 ( = B XOR 000...000100)
6. (5, 5) output B % 8
7. (0, 3) A = A / 2^3

Example:
A = 0

1. B = 0
2. B = 000 XOR 110 = 110 = 6
3. C = 0
4. B = B XOR C = 110 XOR 000 = 110
5. B = B XOR 4 = 110 XOR 100 = 010 = 2
6. out 2 % 8
7.A = 0

A = 1

1. B = 1
2. B = 001 XOR 110 = 111 = 7
3. C = 0
4. B = B XOR C = 111 XOR 000 = 111
5. B = B XOR 4 = 111 XOR 100 = 011 = 3
6. out 3 % 8
7.A = 0

A = 0, out = 2
A = 1, out = 3
A = 2, out = 0
A = 3, out = 1
A = 4, out = 7
A = 5, out = 7
A = 6, out = 2
A = 7, out = 6
A = 8, out = 2,3
A = 9, out = 3,3
A = 10, out = 0,3
A = 11, out = 1,3
A = 12, out = 5,3
A = 13, out = 6,3
A = 14, out = 2,3
A = 15, out = 2,3
A = 16, out = 2,0
A = 17, out = 3,0
A = 18, out = 1,0
A = 19, out = 1,0

One important feature to note is that A is divided by 8 and rounded down after each iteration. We can work backwards 
from the final iteration, multiply by 8 and add i for i in range (0, 7). All these are valid possible solutions
*/

long minA = Int64.MaxValue;
long a = 0;
List<long> candidates = [];
for (long i = 0; i < 8; i++)
{
	candidates.Add(i);
}

bool validateTestProgram(string _testProgramStr, int[] _program)
{
	string[] testProgram = _testProgramStr.Split(",");
	
	if (testProgram.Length > _program.Length)
	{
		throw new Exception("test program too long");
	}
	
	for (int j = 0; j < testProgram.Length; j++)
	{
		// If condition is true then the testProgram cannot be a child 
		if (Int32.Parse(testProgram[testProgram.Length - j - 1]) != _program[_program.Length - j - 1])
		{
			return false;
		}
	}
	return true;
}

while (candidates.Count > 0)
{
	a = candidates[0];
	
	for (long i = 0; i < 8; i++)
	{
		long a2 = a * 8 + i;
		string testProgramStr = run(a2, 0, 0, program);
		if (validateTestProgram(testProgramStr, program))
		{
			if (testProgramStr.Equals(string.Join(",", program)))
			{
				minA = Math.Min(minA, a2);
			}
			else
			{
				candidates.Add(a2);
			}
			Console.WriteLine($"a2 = {a2}, out = " + testProgramStr);
		}
	}
	
	candidates.RemoveAt(0);
}

Console.WriteLine("Solution 2 = " + minA);