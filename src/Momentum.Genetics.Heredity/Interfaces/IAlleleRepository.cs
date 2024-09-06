using Momentum.Genetics.Models;
using Momentum.Repositories.Interfaces;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IAlleleRepository : IRepository<Guid, Allele>
    {
        Task<IEnumerable<Allele>> GetByLocusAsync(Guid locusId, CancellationToken token = default);
    } // end interface
} // end namespace