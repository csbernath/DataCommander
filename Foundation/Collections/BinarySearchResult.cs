namespace Foundation.Collections;

public class BinarySearchResult
{
    public readonly BinarySearchResultRelation ResultRelation;
    public readonly int Index;

    public BinarySearchResult(BinarySearchResultRelation resultRelation, int index)
    {
        ResultRelation = resultRelation;
        Index = index;
    }
}