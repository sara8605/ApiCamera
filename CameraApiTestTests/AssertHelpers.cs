using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CameraApiTestTests;

public static class AssertHelpers
{
    public static void AreEqual(string expected, string actual)
    {
        if (expected == actual)
        {
            Assert.AreEqual(expected, actual);
        }
        else
        {
            int index = FindFirstDifferenceIndex(expected, actual);
            var textlength = Math.Min(expected.Length, actual.Length);
            var length = Math.Min(10, textlength - index);
            Assert.Fail("Strings differ at index {0}. Expected: '{1}', Actual: '{2}'",
                index,
                expected.Substring(index, length),
                actual.Substring(index, length));
        }
    }

    private static int FindFirstDifferenceIndex(string str1, string str2)
    {
        int index = 0;
        int minLength = Math.Min(str1.Length, str2.Length);
        while (index < minLength && str1[index] == str2[index])
        {
            index++;
        }
        return index;
    }
}