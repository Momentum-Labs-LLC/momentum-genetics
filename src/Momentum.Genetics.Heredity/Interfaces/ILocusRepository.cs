using Momentum.Genetics.Models;
using Momentum.Repositories.Interfaces;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface ILocusRepository : IRepository<Guid, Locus>
    {
    } // end interface
} // end namespace