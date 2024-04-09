using Momentum.Genetics.Heredity.Models;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IIndividualRepository<TId>
    {
        Task<Individual<TId>> GetAsync(TId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Individual<TId>>> GetOffspringAsync(TId id, CancellationToken cancellationToken = default);
    } // end interface
} // end namespace