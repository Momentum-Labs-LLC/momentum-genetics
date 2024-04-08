using Momentum.Genetics.Models;

namespace Momentum.Genetics.Extensions
{
    public static class GenotypeExtensions
    {
        public static IEnumerable<Genotype<TAllele, TLocus>> BuildPotentialGenotypes<TAllele, TLocus>(
                this Genotype<TAllele, TLocus> genotype)
            where TAllele : Allele
            where TLocus : Locus<TAllele>, new()
        {
            var locus = new TLocus();

            var result = new List<Genotype<TAllele, TLocus>>();

            if (genotype.DominantAllele == null)
            {
                result = locus.Alleles.SelectMany(dominant =>
                    locus.Alleles
                        .Where(other => other.Ordinal >= dominant.Ordinal)
                        .Select(other => new Genotype<TAllele, TLocus>(dominant, other)))
                    .ToList();
            }
            else if (genotype.OtherAllele == null)
            {
                // dominant is known, need to fill in the other allele.
                result = locus.Alleles
                    .Where(other => other.Ordinal >= genotype.DominantAllele.Ordinal)
                    .Select(other => new Genotype<TAllele, TLocus>(genotype.DominantAllele, other))
                    .ToList();
            }
            else
            {
                result.Add(genotype);
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace