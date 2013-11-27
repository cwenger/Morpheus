namespace Morpheus
{
    public enum CleavageSpecificity
    {
        [MinNumberTermini(0)]
        None,
        [MinNumberTermini(1)]
        SemiN,
        [MinNumberTermini(1)]
        SemiC,
        [MinNumberTermini(1)]
        Semi,
        [MinNumberTermini(2)]
        Full
    }
}
