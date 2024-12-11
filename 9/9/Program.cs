using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
StreamReader sr = new StreamReader("/Users/nathanedwards/Dev/AoC/9/9/input.txt");

// First parse data
string diskMapStr = sr.ReadToEnd();
Char[] diskMap = diskMapStr.ToCharArray();


// Next compute the decompressed disk
List<string> decompressedDisk = [];
int id = 0;

for (int i = 0; i < diskMap.Length; i++)
{
	int c = Int32.Parse("" + diskMap[i]);
	
	// If below true then c defines a program
	if (i % 2 == 0)
	{
		for (int j = 0; j < c; j++)
		{
			decompressedDisk.Add("" + id);
		}
		id++;
	}
	// Else c defines free space
	else
	{
		for (int j = 0; j < c; j++)
		{
			decompressedDisk.Add(".");
		}
	}
}

Console.WriteLine("Decompressed disk = " + String.Join("",decompressedDisk));

string[] decompressedDiskArray = decompressedDisk.ToArray();
int n = 0;
int m = decompressedDiskArray.Length - 1;

while (n < decompressedDiskArray.Length && m > n)
{
	if (decompressedDiskArray[n] == ".")
	{
		if (decompressedDiskArray[m] == ".")
		{
			m--;
		}	
		else
		{
			decompressedDiskArray[n] = decompressedDiskArray[m];
			decompressedDiskArray[m] = ".";
		}
	}
	else
	{
		n++;
	}
}

long doCheckSum(string[] array)
{
	long checkSum = 0;
	
	for (int i = 0; i < array.Length; i++)
	{
		if (array[i] != ".")
		{
			checkSum += i * Int64.Parse(decompressedDiskArray[i]);
		}	
	}
	
	return checkSum;
}

Console.WriteLine("Compressed disk = " + String.Join("",decompressedDiskArray) + "\n");

Console.WriteLine("solution 1 = " + doCheckSum(decompressedDiskArray) + "\n");

/* --------------- Part 2 --------------- */
/* 
Now we must moves blocks of id codes into empty spaces only if the whole block fits.
So I will cycle from right to left through blocks, calculate block length, cycle left to right through empty spaces,
calculate empty space length and move the block to the first match.
*/

int getBlockSize(string[] decompressedDiskArray, int ind, string dir)
{
	string str = decompressedDiskArray[ind];
	int arrayLength = decompressedDiskArray.Length;
	int size = 0;
	
	if (dir == "right")
	{
		while (ind < arrayLength && decompressedDiskArray[ind] == str)
		{
			size++;
			ind++;
		}
	}
	else if (dir == "left")
	{
		while (ind >= 0 && decompressedDiskArray[ind] == str)
		{
			size++;
			ind--;
		}
	}
	else
	{
		throw new Exception("incorrect direction");
	}
	
	return size;
}

decompressedDiskArray = decompressedDisk.ToArray();
n = 0;
m = decompressedDiskArray.Length - 1;

int programBlockSize;
int emptyBlockSize;


while (m > 0)
{	
	if (decompressedDiskArray[m] == ".")
	{
		m--;
	}
	else
	{
		bool movedBlock = false;
		programBlockSize = getBlockSize(decompressedDiskArray, m, "left");
		Console.WriteLine("m = " + m);
		
		while (n < m + 1 - programBlockSize)
		{
			if (decompressedDiskArray[n] != ".")
			{
				n++;
			}	
			else
			{
				emptyBlockSize = getBlockSize(decompressedDiskArray, n, "right");
				
				if (emptyBlockSize >= programBlockSize) 
				{
					for (int _ = 0; _ < programBlockSize; _++)
					{
						decompressedDiskArray[n] = decompressedDiskArray[m];
						decompressedDiskArray[m] = ".";
						n++;
						m--;
						movedBlock = true;
					}
					// Console.WriteLine("Compressed disk = " + String.Join("",decompressedDiskArray) + "\n");

					break;
				}	
				else
				{
					n += emptyBlockSize;
				}
			}
		}
		
		if (!movedBlock)
		{
			m -= programBlockSize;
		}
		
		n = 0;
	}
}


Console.WriteLine("Compressed disk = " + String.Join("",decompressedDiskArray) + "\n");

Console.WriteLine("solution 2 = " + doCheckSum(decompressedDiskArray) + "\n");