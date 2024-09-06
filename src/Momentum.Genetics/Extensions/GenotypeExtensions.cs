using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Extensions
{
    public static class GenotypeExtensions
    {
        public static IEnumerable<IGenotype> BuildPotentialGenotypes(
                this IGenotype genotype, IEnumerable<Allele> locusAlleles)
        {
            var result = new List<IGenotype>();

            if (genotype.DominantAllele == null)
            {
                result = locusAlleles.SelectMany(dominant =>
                    locusAlleles
                        .Where(other => other.Ordinal >= dominant.Ordinal)
                        .Select(other => new Genotype(dominant, other) as IGenotype))
                    .ToList();
            }
            else if (genotype.OtherAllele == null)
            {
                // dominant is known, need to fill in the other allele.
                result = locusAlleles
                    .Where(other => other.Ordinal >= genotype.DominantAllele.Ordinal)
                    .Select(other => new Genotype(genotype.DominantAllele, other) as IGenotype)
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