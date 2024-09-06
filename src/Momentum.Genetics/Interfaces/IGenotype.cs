using Momentum.Genetics.Models;

namespace Momentum.Genetics.Interfaces
{
    public interface IGenotype
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the locus identifier.
        /// </summary>
        Guid LocusId { get; }

        /// <summary>
        /// Gets or sets the more dominant allele.
        /// </summary>
        public Allele DominantAllele { get; set; }

        /// <summary>
        /// Gets or sets the less dominant allele. Can be the same as the first allele, different, or unknown.
        /// </summary>
        public Allele? OtherAllele { get; set; }
    } // end interface
} // end namespace