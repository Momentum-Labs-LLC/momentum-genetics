using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Interfaces;

namespace Momentum.Genetics.Heredity
{
    public class PunnetSquare : IPunnetSquare
    {
        protected readonly IAlleleRepository _alleleRepository;
        protected readonly ILogger _logger;

        public PunnetSquare(IAlleleRepository alleleRepository, ILogger<PunnetSquare> logger)
        {
            _alleleRepository = alleleRepository ?? throw new ArgumentNullException(nameof(alleleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<IEnumerable<GenotypeCount>> GetOffsprinGenotypesAsync(
            IGenotype paternalGenotype, 
            IGenotype maternalGenotype,
            CancellationToken token = default)
        {
            var paternalAlleles = await BuildAlleleSetsAsync(paternalGenotype, token).ConfigureAwait(false);
            var maternalAlleles = await BuildAlleleSetsAsync(maternalGenotype, token).ConfigureAwait(false);;

            var potentials = CombineAlleleSets(paternalAlleles, maternalAlleles);            

            var genotypeRatios = BuildGenotypeCounts(potentials);

            return genotypeRatios;
        } // end method

        /// <summary>
        /// Build the potential alleles of a given genotype.
        /// If one of the genes is unknown then a genotype is created for each equal or higher ordinal (lower dominance) allele.
        /// </summary>
        /// <param name="potentialGenotypes"></param>
        /// <returns></returns>
        protected async Task<Dictionary<Allele, List<Allele>>> BuildAlleleSetsAsync(IGenotype genoType, CancellationToken token = default)
        {
            var alleles = await _alleleRepository.GetByLocusAsync(genoType.LocusId, token).ConfigureAwait(false);
            var potentialGenotypes = genoType.BuildPotentialGenotypes(alleles);
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
        protected IEnumerable<IGenotype> CombineAlleleSets(
            Dictionary<Allele, List<Allele>> paternalAlleles, 
            Dictionary<Allele, List<Allele>> maternalAlleles)
        {
            var results = new List<Genotype>();
            foreach(var paternalKvp in paternalAlleles)
            {
                foreach(var paternalOther in paternalKvp.Value)
                {
                    foreach(var maternalKvp in maternalAlleles)
                    {
                        foreach(var maternalOther in maternalKvp.Value)
                        {
                            results.Add(new Genotype(paternalKvp.Key, maternalKvp.Key));
                            results.Add(new Genotype(paternalOther, maternalKvp.Key));
                            results.Add(new Genotype(maternalOther, paternalKvp.Key));
                            results.Add(new Genotype(maternalOther, paternalOther));
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
        protected IEnumerable<GenotypeCount> BuildGenotypeCounts(IEnumerable<IGenotype> genotypes)
        {
            return genotypes
                .GroupBy(x => x.ToString())
                .Select(x => new GenotypeCount() 
                    { 
                        Genotype = x.First(),
                        Count = x.Count()
                    })
                .ToList();
        } // end method
    } // end class
} // end namespace