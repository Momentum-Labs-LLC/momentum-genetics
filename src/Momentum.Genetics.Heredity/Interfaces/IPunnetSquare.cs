using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IPunnetSquare<TAllele, TLocus>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        IEnumerable<GenotypeRatio<TAllele, TLocus>> GetOffsprinGenotypes(
            Genotype<TAllele, TLocus> paternalGenotype, 
            Genotype<TAllele, TLocus> maternalGenotype);
    } // end interface
} // end namespace