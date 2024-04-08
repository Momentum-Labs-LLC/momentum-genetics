using Momentum.Genetics.Models;
using Momentum.Genetics.Heredity.Models;

namespace Momentum.Genetics.Heredity.Extensions
{
    public static class IndividualGenotypeExtensions
    {
        public static IndividualGenotype<TAllele, TLocus, TId> BuildIndividual<TAllele, TLocus, TId>(
                this Genotype<TAllele, TLocus> genotype,
                Func<TId> idProvider,
                TId? paternalId = default,
                TId? maternalId = default)
            where TAllele : Allele
            where TLocus : Locus<TAllele>, new()
        {
            return new IndividualGenotype<TAllele, TLocus, TId>()
            {
                Id = idProvider(),
                Genotype = genotype,
                PaternalId = paternalId,
                MaternalId = maternalId
            };
        } // end method
    } // end class
} // end namespace