using Momentum.Genetics.Models;

namespace Momentum.Genetics.Extensions
{
    public static class AlleleExtensions
    {
        public static Genotype<TAllele, TLocus> BuildGenotype<TAllele, TLocus>(
                this TAllele allele,
                TAllele? other = null)
            where TAllele : Allele
            where TLocus : Locus<TAllele>, new()
        {
            Genotype<TAllele, TLocus> result = new Genotype<TAllele, TLocus>(allele, other);
            if (other == null && allele.Dominance == DominanceEnum.Recessive)
            {
                result = new Genotype<TAllele, TLocus>(allele, allele);
            } // end if
            return result;
        } // end method
    } // end class
} // end namespace