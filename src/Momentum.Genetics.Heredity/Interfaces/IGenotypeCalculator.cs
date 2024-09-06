using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IGenotypeCalculator<TId>
    {
        Task<IEnumerable<IGenotype>> CalculateGenotypesAsync(TId individualId, Guid locusId, CancellationToken token = default);
    } // end interface

    public interface IGenotypeCalculator : IGenotypeCalculator<Guid> 
    {} // end interface
} // end namespace