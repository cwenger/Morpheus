namespace Morpheus
{
    public interface ITargetDecoy
    {
        bool Target { get; }
        bool Decoy { get; }
        double Score { get; }
    }
}
