using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IGenotypeCalculator<TAllele, TLocus, TId>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        Task<IEnumerable<Genotype<TAllele, TLocus>>> CalculateGenotypesAsync(TId individualId, CancellationToken token = default);
    } // end interface
} // end namespace