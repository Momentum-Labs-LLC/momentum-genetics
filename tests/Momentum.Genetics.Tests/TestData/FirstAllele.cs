using Momentum.Genetics.Models;

namespace Momentum.Genetics.TestData
{
    public class FirstAllele : TestAllele
    {
        public FirstAllele()
        {
            this.Ordinal = 0;
            this.Name = "First";
            this.Symbol = "A";
            this.Dominance = DominanceEnum.Dominant;
        } // end method
    } // end class

} // end namespace