using Momentum.Genetics.Models;

namespace Momentum.Genetics.TestData
{
    public class TestAllele : Allele {}
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
            new FirstAllele(),
            new SecondAllele(),
            new ThirdAllele()
        };
    } // end class
} // end namespace