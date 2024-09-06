using Momentum.Repositories.Interfaces;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IIndividualRepository<TId, TIndividual> : IRepository<TId, TIndividual>
        where TId : struct
        where TIndividual : IIndividual<TId>
    {
        Task<IEnumerable<TIndividual>> GetOffspringAsync(TId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TIndividual>> GetOffspringAsync(TId paternalId, TId maternalId, CancellationToken token = default);
    } // end interface

    public interface IIndividualRepository<TIndividual> : IIndividualRepository<Guid, TIndividual>
        where TIndividual : IIndividual
    {

    } // end interface

    public interface IIndividualRepository : IIndividualRepository<IIndividual> 
    {

    } // end interface
} // end namespace