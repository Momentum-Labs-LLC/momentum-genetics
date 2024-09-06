using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;
using Momentum.Repositories.Interfaces;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IGenotypeRepository<TId> : IRepository<Guid, IGenotype>
        where TId : struct
    {
        Task<IGenotype> GetAsync(TId individualId, Guid locusId, CancellationToken token = default);
        Task<IEnumerable<IGenotype>> GetOffspringGenotypesAsync(TId paternalId, TId maternalId, Guid locusId, CancellationToken token = default);
    } // end interface

    public interface IGenotypeRepository : IGenotypeRepository<Guid>
    {} // end interface
} // end namespace