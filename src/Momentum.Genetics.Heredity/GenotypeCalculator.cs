using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity
{
    public class GenotypeCalculator<TAllele, TLocus, TId> : IGenotypeCalculator<TAllele, TLocus, TId>
        where TAllele : Allele
        where TLocus : Locus<TAllele>, new()
        where TId : IEquatable<TId>
    {
        protected readonly IIndividualRepository<TId> _individualRepository;
        protected readonly IGenotypeRepository<TAllele, TLocus, TId> _genotypeRepository;
        protected readonly IPunnetSquare<TAllele, TLocus> _punnetSquare;
        protected readonly ILogger _logger;

        public GenotypeCalculator(
            IIndividualRepository<TId> individualRepository,
            IGenotypeRepository<TAllele, TLocus, TId> genotypeRepository,
            IPunnetSquare<TAllele, TLocus> punnetSquare,
            ILogger<GenotypeCalculator<TAllele, TLocus, TId>> logger)
        {
            _individualRepository = individualRepository ?? throw new ArgumentNullException(nameof(individualRepository));
            _genotypeRepository = genotypeRepository ?? throw new ArgumentNullException(nameof(genotypeRepository));
            _punnetSquare = punnetSquare ?? throw new ArgumentNullException(nameof(punnetSquare));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<IEnumerable<Genotype<TAllele, TLocus>>> CalculateGenotypesAsync(TId individualId, CancellationToken token = default)
        {
            var result = new List<Genotype<TAllele, TLocus>>();

            var individual = await _individualRepository.GetAsync(individualId, token).ConfigureAwait(false);
            var genotype = await _genotypeRepository.GetAsync(individualId, token).ConfigureAwait(false);
            
            if(genotype.DominantAllele == null 
                || genotype.OtherAllele == null)
            {
                // first go from parents
                var relevantGenotypesFromParents = await GetPotentialGenotypesFromParentsAsync(individual, genotype, token).ConfigureAwait(false);
                var potentialGenotypes = relevantGenotypesFromParents.Select(x => x.Genotype);

                if(potentialGenotypes.Count() > 1)
                {
                    // then go against children and the fellow parent of those children.
                    var applicableGenotypesFromChildren = await GetApplicableGenotypesFromOffspringAsync(
                            individual, 
                            potentialGenotypes,
                            token)
                        .ConfigureAwait(false);

                    result = applicableGenotypesFromChildren.ToList();
                }
                else
                {
                    result = potentialGenotypes.ToList();
                } // end if
            }
            else
            {
                result.Add(genotype);
            } // end if

            return result;
        } // end method

        protected virtual async Task<IEnumerable<GenotypeRatio<TAllele, TLocus>>> GetPotentialGenotypesFromParentsAsync(
            Individual<TId> individual,
            Genotype<TAllele, TLocus> individualGenotype,
            CancellationToken token = default)
        {
            var paternalGenotype = new Genotype<TAllele, TLocus>();
            var materalGenotype = new Genotype<TAllele, TLocus>();
            
            if(!individual.PaternalId.Equals(default(TId?)))
            {
                paternalGenotype = await _genotypeRepository.GetAsync(individual.PaternalId, token).ConfigureAwait(false);
            } // end if
            
            if(!individual.MaternalId.Equals(default(TId?)))
            {
                materalGenotype = await _genotypeRepository.GetAsync(individual.MaternalId, token).ConfigureAwait(false);
            } // end if

            var results = _punnetSquare.GetOffsprinGenotypes(paternalGenotype, materalGenotype);
            if(individualGenotype.DominantAllele != null)
            {
                results = results.Where(x => x.Genotype.DominantAllele.Ordinal == individualGenotype.DominantAllele.Ordinal);
            } // end if

            return results;
        } // end method
    
        protected virtual async Task<IEnumerable<Genotype<TAllele, TLocus>>> GetApplicableGenotypesFromOffspringAsync(
            Individual<TId> individual,
            IEnumerable<Genotype<TAllele, TLocus>> potentialGenotypes,
            CancellationToken token = default)
        {
            var result = potentialGenotypes.ToList();
            // Get all the offspring of the individual
            var allOffspring = await _individualRepository.GetOffspringAsync(individual.Id, token).ConfigureAwait(false);

            // are there any offspring?
            if(allOffspring != null && allOffspring.Any())
            {
                // group the offspring by both parents
                var siblingGroups = allOffspring
                    .GroupBy(x => new { PaternalId = x.PaternalId, MaternalId = x.MaternalId });

                foreach(var siblings in siblingGroups)
                {
                    var siblingGenotypes = await _genotypeRepository.GetOffspringGenotypesAsync(
                            siblings.Key.PaternalId, 
                            siblings.Key.MaternalId, 
                            token).ConfigureAwait(false);
                            
                    var expressedGenotypes = siblingGenotypes
                        .Where(x => x.DominantAllele != null)
                        .GroupBy(x => x.ToString())
                        .Select(x => x.First())
                        .OrderBy(x => x.DominantAllele.Ordinal)
                        .ToList();
                    
                    var otherParentId = siblings.Any(x => individual.Id.Equals(x.PaternalId)) ? siblings.First().MaternalId : siblings.First().PaternalId;
                    IEnumerable<Genotype<TAllele, TLocus>> otherParentGenotypes = null;
                    if(otherParentId != null)
                    {
                        var otherParentGenotype = await _genotypeRepository.GetAsync(otherParentId, token).ConfigureAwait(false);
                        otherParentGenotypes = otherParentGenotype.BuildPotentialGenotypes();
                    }
                    else
                    {
                        otherParentGenotypes = new Genotype<TAllele, TLocus>().BuildPotentialGenotypes();
                    } // end if

                    if(otherParentGenotypes.Count() > 1)
                    {
                        // restrict the other parent genotypes to ones that could actually help produce the expressed genotypes
                        otherParentGenotypes = RefinePotentialGenotypes(expressedGenotypes, otherParentGenotypes, result);
                    } // end if

                    var refinedPotentialGenotypes = RefinePotentialGenotypes(expressedGenotypes, result, otherParentGenotypes);

                    result = refinedPotentialGenotypes.ToList();
                } // end foreach
            } // end if
            
            return result;
        } // end method

        protected virtual IEnumerable<Genotype<TAllele, TLocus>> RefinePotentialGenotypes(
            IEnumerable<Genotype<TAllele, TLocus>> expressedGenotypes,
            IEnumerable<Genotype<TAllele, TLocus>> parentGenotypes,
            IEnumerable<Genotype<TAllele, TLocus>> otherParentGenotypes)
        {
            var result = parentGenotypes.ToList();

            // build a dictionary of each potential genotype and the offspring genotypes it could generate with this other parent
            var potentialGenotypeOffspring = result.ToDictionary(
                // the potential genotypes
                potentialGenotype => potentialGenotype, 
                // the genotypes than can come from this potential genotype
                potentialGenotype => otherParentGenotypes.SelectMany(otherParentGenotype => 
                        _punnetSquare.GetOffsprinGenotypes(potentialGenotype, otherParentGenotype)
                            .Select(x => x.Genotype)));

            // now remove any potential genotypes that did not produce genotypes that explain all the expressions
            foreach(var potentialGenotypeOffspringKvp in potentialGenotypeOffspring)
            {
                var allExpressedGenotypesRepresented = expressedGenotypes.All(expressedGenotype => 
                    potentialGenotypeOffspringKvp.Value.Any(genotype => 
                        genotype.DominantAllele.Ordinal == expressedGenotype.DominantAllele.Ordinal
                        && (expressedGenotype.OtherAllele == null || genotype.OtherAllele.Ordinal == expressedGenotype.OtherAllele.Ordinal)));

                if(!allExpressedGenotypesRepresented)
                {
                    result.Remove(potentialGenotypeOffspringKvp.Key);
                } // end if
            } // end foreach

            return result;
        } // end method
    } // end class
} // end namespace