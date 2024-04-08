using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Models
{
    public class GenotypeRatio<TAllele, TLocus>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        public Genotype<TAllele, TLocus> Genotype { get; set; }
        public float Ratio { get; set; }
    } // end class
} // end namespace