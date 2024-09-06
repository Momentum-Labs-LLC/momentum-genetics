using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Genetics.Heredity.Interfaces
{
    public interface IIndividual<TId>
        where TId: struct
    {
        /// <summary>
        /// Gets or sets the identifier of the individual
        /// </summary>
        public TId Id { get; }

        /// <summary>
        /// Gets or sets the paternal identifier.
        /// </summary>
        public Nullable<TId> PaternalId { get; }

        /// <summary>
        /// Gets or sets the maternal identifier.
        /// </summary>
        public Nullable<TId> MaternalId { get; }
    } // end interface

    public interface IIndividual : IIndividual<Guid> {} // end interface
} // end namespace