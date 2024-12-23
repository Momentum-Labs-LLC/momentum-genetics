namespace Momentum.Genetics.Models
{
    public class AlleleRelationship
    {
        public Guid Id { get; set; }
        public Allele FirstAllele { get; set; }
        public Allele SecondAllele { get; set; }
        public DominanceEnum Relationship { get; set; }
    } // end class
} // end namespace