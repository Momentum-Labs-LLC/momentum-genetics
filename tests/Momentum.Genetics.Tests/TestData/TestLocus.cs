using Momentum.Genetics.Models;

namespace Momentum.Genetics.TestData
{
    public class TestAllele : Allele {}
    public class TestLocus : Locus
    {
        
        public TestLocus() 
        {
            this.Symbol = "A";
            this.Name = "Test";
            this.Description = "test";
            this.Alleles = new List<TestAllele>()
            {
                new FirstAllele(),
                new SecondAllele(),
                new ThirdAllele()
            };
        } // end method
    } // end class
} // end namespace