namespace Momentum.Genetics.Heredity.Models
{
    /// <summary>
    /// This class represents an individual member of any given species.
    /// </summary>
    public class Individual<TId>
        where TId : struct
    {
        /// <summary>
        /// Gets or sets the identifier of the individual
        /// </summary>
        public TId Id { get; set; }

        /// <summary>
        /// Gets or sets the paternal identifier.
        /// </summary>
        public Nullable<TId> PaternalId { get; set; }

        /// <summary>
        /// Gets or sets the maternal identifier.
        /// </summary>
        public Nullable<TId> MaternalId { get; set; }
    } // end class

    public class Individual : Individual<Guid> 
    { 
        public Individual()
        {
            Id = Guid.NewGuid();
        } // end method

        public Individual(Individual father, Individual mother) : base()
        {
            PaternalId = father.Id;
            MaternalId = mother.Id;
        } // end method
    } // end class
} // end namespace