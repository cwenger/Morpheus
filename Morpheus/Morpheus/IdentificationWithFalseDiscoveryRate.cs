using System.Globalization;
using System.Text;

namespace Morpheus
{
    public class IdentificationWithFalseDiscoveryRate<T> where T : ITargetDecoy
    {
        public T Identification { get; private set; }
        public int CumulativeTarget { get; private set; }
        public int CumulativeDecoy { get; private set; }
        public double QValue { get; set; }

        public static readonly string Header = "\tTarget?\tDecoy?\tCumulative Target\tCumulative Decoy\tQ-Value (%)";

        public IdentificationWithFalseDiscoveryRate(T identification, int cumulativeTarget, int cumulativeDecoy, double qValue)
        {
            Identification = identification;
            CumulativeTarget = cumulativeTarget;
            CumulativeDecoy = cumulativeDecoy;
            QValue = qValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Identification.ToString() + '\t');

            sb.Append(Identification.Target.ToString() + '\t');
            sb.Append(Identification.Decoy.ToString() + '\t');
            sb.Append(CumulativeTarget.ToString() + '\t');
            sb.Append(CumulativeDecoy.ToString() + '\t');
            sb.Append((QValue * 100.0).ToString(CultureInfo.InvariantCulture));

            return sb.ToString();
        }
    }
}
