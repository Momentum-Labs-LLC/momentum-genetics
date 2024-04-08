using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class TestAllele : Allele
    {
        public static readonly TestAllele Agouti = new TestAllele(1, nameof(Agouti), "A");
        public static readonly TestAllele Tan = new TestAllele(2, nameof(Tan), "a", "t");
        public static readonly TestAllele Self = new TestAllele(3, nameof(Self), "a", dominance: DominanceEnum.Recessive);

        public TestAllele(
            int ordinal, 
            string name, 
            string symbol, 
            string? genotypeSymbol = null,
            DominanceEnum dominance = DominanceEnum.Dominant)
        {
            this.Ordinal = ordinal;
            this.Name = name;
            this.Symbol = symbol;
            this.GenotypeSymbol = genotypeSymbol;
            this.Dominance = dominance;
        } // end method
    } // end method

    public class TestLocus : Locus<TestAllele>
    {
        
        public TestLocus() 
        {
            this.Symbol = "A";
            this.Name = "Test";
            this.Description = "test";
        } // end method

        public override IEnumerable<TestAllele> Alleles => new List<TestAllele>()
        {
            TestAllele.Agouti,
            TestAllele.Tan,
            TestAllele.Self
        };
    } // end class
}