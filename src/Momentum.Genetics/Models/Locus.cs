namespace Momentum.Genetics.Models
{
    /// <summary>
    /// A physical location of a chromosone in which a gene, or its alleles, exists.
    /// 
    /// </summary>
    public abstract class Locus<TAllele>
        where TAllele : Allele
    {
        /// <summary>
        /// Gets the alleles of a locus.
        /// </summary>
        public abstract IEnumerable<TAllele> Alleles { get; }

        /// <summary>
        /// Gets or sets the locus.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the gene.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the symbol of the gene.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the description of the gene.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets index of the base pair at which this gene begins on a chromosome.
        /// </summary>
        public CytogenicLocation? CytogenicLocation { get; set; }

        /// <summary>
        /// Gets or sets the number of base pairs that make up this gene.
        /// </summary>
        public long? Length { get; set; }
    } // end class
} // end namespace