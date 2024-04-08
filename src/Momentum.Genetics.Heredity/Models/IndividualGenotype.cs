using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Models
{
    public class IndividualGenotype<TAllele, TLocus, TId> : Individual<TId>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        public Genotype<TAllele, TLocus> Genotype { get; set; }
    } // end class

    public class IndividualGenotype<TAllele, TLocus> : IndividualGenotype<TAllele, TLocus, Guid>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {} // end class
} // end namespace