using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Models;

namespace Momentum.Genetics.Heredity
{
    public class PunnetSquare<TAllele, TLocus> : IPunnetSquare<TAllele, TLocus>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
    {
        protected static readonly TLocus _locus = new TLocus();
        protected readonly ILogger _logger;

        public PunnetSquare(ILogger<PunnetSquare<TAllele, TLocus>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual IEnumerable<GenotypeRatio<TAllele, TLocus>> GetOffsprinGenotypes(
            Genotype<TAllele, TLocus> paternalGenotype, 
            Genotype<TAllele, TLocus> maternalGenotype)
        {
            var paternalAlleles = BuildAlleleSets(paternalGenotype);
            var maternalAlleles = BuildAlleleSets(maternalGenotype);

            var potentials = CombineAlleleSets(paternalAlleles, maternalAlleles);            

            var genotypeRatios = BuildGenotypeRatios(potentials);

            return genotypeRatios;
        } // end method

        /// <summary>
        /// Build the potential alleles of a given genotype.
        /// If one of the genes is unknown then a genotype is created for each equal or higher ordinal (lower dominance) allele.
        /// </summary>
        /// <param name="potentialGenotypes"></param>
        /// <returns></returns>
        protected Dictionary<TAllele, List<TAllele>> BuildAlleleSets(Genotype<TAllele, TLocus> genoType)
        {
            var potentialGenotypes = genoType.BuildPotentialGenotypes();
            return potentialGenotypes
                .GroupBy(x => x.DominantAllele.Ordinal)
                .ToDictionary(group => 
                    group.First().DominantAllele, 
                    group => group.Select(genotype => genotype.OtherAllele).ToList());     
        } // end method        

        /// <summary>
        /// Run punnet square (cross join) for each set of genotypes of each parent.
        /// </summary>
        /// <param name="paternalAlleles"></param>
        /// <param name="maternalAlleles"></param>
        /// <returns></returns>
        protected IEnumerable<Genotype<TAllele, TLocus>> CombineAlleleSets(
            Dictionary<TAllele, List<TAllele>> paternalAlleles, 
            Dictionary<TAllele, List<TAllele>> maternalAlleles)
        {
            var results = new List<Genotype<TAllele, TLocus>>();
            foreach(var paternalKvp in paternalAlleles)
            {
                foreach(var paternalOther in paternalKvp.Value)
                {
                    foreach(var maternalKvp in maternalAlleles)
                    {
                        foreach(var maternalOther in maternalKvp.Value)
                        {
                            results.Add(new Genotype<TAllele, TLocus>(paternalKvp.Key, maternalKvp.Key));
                            results.Add(new Genotype<TAllele, TLocus>(paternalOther, maternalKvp.Key));
                            results.Add(new Genotype<TAllele, TLocus>(maternalOther, paternalKvp.Key));
                            results.Add(new Genotype<TAllele, TLocus>(maternalOther, paternalOther));
                        } // end foreach
                    } // end foreach         
                } // end foreach                
            } // end foreach

            return results;
        } // end method

        /// <summary>
        /// Takes a set of genotypes and returns the distinct ratios of each.
        /// </summary>
        /// <param name="genotypes"></param>
        /// <returns></returns>
        protected IEnumerable<GenotypeRatio<TAllele, TLocus>> BuildGenotypeRatios(IEnumerable<Genotype<TAllele, TLocus>> genotypes)
        {
            return genotypes
                .GroupBy(x => x.ToString())
                .Select(x => new GenotypeRatio<TAllele, TLocus>() 
                    { 
                        Genotype = x.First(),
                        Ratio = (float)x.Count() / (float)genotypes.Count()
                    })
                .ToList();
        } // end method
    } // end class
} // end namespace