using System;
using z3n7.Api;

internal static class Program
{
    private static int Main()
    {
        try
        {
            Equal("demo@example.com", TempMail.CreateAddress("demo", "@example.com"), "address");
            Equal("55502f40dc8b7c769880b10874abc9d0", TempMail.HashEmail("test@example.com"), "hash");
            Console.WriteLine("PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Equal(string expected, string actual, string name)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new Exception(name + ": expected " + expected + ", got " + actual);
    }
}
