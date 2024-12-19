Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/17/17/input.txt");
string[] strArr = sr.ReadToEnd().Split("\n");

long A = Int64.Parse(strArr[0].Split(": ")[1]);
long B = Int64.Parse(strArr[1].Split(": ")[1]);
long C = Int64.Parse(strArr[2].Split(": ")[1]);
int[] program = Array.ConvertAll(strArr[4].Split(" ")[1].Split(","), s => int.Parse(s));
long pointer = 0;
List<string> _out = [];

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

Console.WriteLine("Running program\n");

while (pointer < program.Length - 1)
{
	long opcode = program[pointer];
	long operand = program[pointer + 1];
	
	(A, B, C, _out, pointer) = applyOpcode(A, B, C, _out, opcode, operand, pointer);
	
}


Console.WriteLine($"A = {A} \nB = {B} \nC = {C}\n");
Console.WriteLine("Solution 1 = " + String.Join(",", _out));