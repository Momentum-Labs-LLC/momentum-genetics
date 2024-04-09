namespace Momentum.Genetics.Models
{
    public abstract class Phenotype
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genotype { get; set; }
    } // end class
} // end namespace