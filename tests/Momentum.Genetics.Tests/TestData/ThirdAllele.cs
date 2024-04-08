using Momentum.Genetics.Models;

namespace Momentum.Genetics.TestData
{
    public class ThirdAllele : TestAllele
    {
        public ThirdAllele()
        {
            this.Ordinal = 2;
            this.Name = "Third";
            this.Symbol = "a";
            this.Dominance = DominanceEnum.Recessive;
        } // end method
    } // end class

} // end namespace