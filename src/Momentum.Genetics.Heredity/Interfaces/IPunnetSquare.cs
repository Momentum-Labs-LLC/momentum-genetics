using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Interfaces;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IPunnetSquare
    {
        Task<IEnumerable<GenotypeCount>> GetOffsprinGenotypesAsync(
            IGenotype paternalGenotype, 
            IGenotype maternalGenotype,
            CancellationToken token = default);
    } // end interface
} // end namespace