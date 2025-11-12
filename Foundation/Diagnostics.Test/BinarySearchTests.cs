using Foundation.Collections;
using Xunit;

namespace Foundation.Diagnostics.Test;

public class BinarySearchTests
{
    [Fact]
    public void Test1()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 0;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.LessThanFirst, result.ResultRelation);
        Assert.Equal(0, result.Index);
    }

    [Fact]
    public void Test2()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 10;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.Equals, result.ResultRelation);
        Assert.Equal(0, result.Index);
    }

    [Fact]
    public void Test3()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 11;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.GreaterThan, result.ResultRelation);
        Assert.Equal(0, result.Index);
    }

    [Fact]
    public void Test4()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 90;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.Equals, result.ResultRelation);
        Assert.Equal(8, result.Index);
    }

    [Fact]
    public void Test5()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 91;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.GreaterThan, result.ResultRelation);
        Assert.Equal(8, result.Index);
    }
    
    [Fact]
    public void Test6()
    {
        var array = new[] { 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        var value = 51;
        bool GreaterThan(int index) => value > array[index];
        bool Equals(int index) => value == array[index];
        var result = BinarySearch.Search(0, array.Length - 1, GreaterThan, Equals);
        Assert.Equal(BinarySearchResultRelation.GreaterThan, result.ResultRelation);
        Assert.Equal(4, result.Index);
    }    
}