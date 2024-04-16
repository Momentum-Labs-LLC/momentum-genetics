using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IGenotypeRepository<TAllele, TLocus, TId>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
        where TId : struct
    {
        Task<Genotype<TAllele, TLocus>> GetAsync(TId individualId, CancellationToken token = default);
        Task<IEnumerable<Genotype<TAllele, TLocus>>> GetOffspringGenotypesAsync(TId paternalId, TId maternalId, CancellationToken token = default);
    } // end interface
} // end namespace