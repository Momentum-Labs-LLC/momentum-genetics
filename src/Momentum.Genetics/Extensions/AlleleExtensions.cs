using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Extensions
{
    public static class AlleleExtensions
    {
        public static IGenotype BuildGenotype(
                this Allele allele,
                Guid locusId,
                Allele? other = null)
        {
            IGenotype result = new Genotype(allele, other) { LocusId = locusId};
            if (other == null && allele.Dominance == DominanceEnum.Recessive)
            {
                result = new Genotype(allele, allele) { LocusId = locusId};
            } // end if
            return result;
        } // end method
    } // end class
} // end namespace