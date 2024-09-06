using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity
{
    public class GenotypeCalculator<TId, TIndividual, TIndividualRepository, TGenotypeRepository> : IGenotypeCalculator<TId>
        where TId : struct, IEquatable<TId>
        where TIndividual : IIndividual<TId>
        where TIndividualRepository : IIndividualRepository<TId, TIndividual>
        where TGenotypeRepository : IGenotypeRepository<TId>
    {
        protected readonly TIndividualRepository _individualRepository;
        protected readonly TGenotypeRepository _genotypeRepository;
        protected readonly IAlleleRepository _alleleRepository;
        protected readonly IPunnetSquare _punnetSquare;
        protected readonly ILogger _logger;

        public GenotypeCalculator(
            TIndividualRepository individualRepository,
            TGenotypeRepository genotypeRepository,
            IAlleleRepository alleleRepository,
            IPunnetSquare punnetSquare,
            ILogger<GenotypeCalculator<TId, TIndividual, TIndividualRepository, TGenotypeRepository>> logger)
        {
            _individualRepository = individualRepository ?? throw new ArgumentNullException(nameof(individualRepository));
            _genotypeRepository = genotypeRepository ?? throw new ArgumentNullException(nameof(genotypeRepository));
            _alleleRepository = alleleRepository ?? throw new ArgumentNullException(nameof(alleleRepository));
            _punnetSquare = punnetSquare ?? throw new ArgumentNullException(nameof(punnetSquare));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<IEnumerable<IGenotype>> CalculateGenotypesAsync(TId individualId, Guid locusId, CancellationToken token = default)
        {
            var result = new List<IGenotype>();

            var individual = await _individualRepository.GetAsync(individualId, token).ConfigureAwait(false);
            var genotype = await _genotypeRepository.GetAsync(individualId, locusId, token).ConfigureAwait(false);
            
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
                            locusId,
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

        protected virtual async Task<IEnumerable<GenotypeCount>> GetPotentialGenotypesFromParentsAsync(
            TIndividual individual,
            IGenotype individualGenotype,
            CancellationToken token = default)
        {
            IGenotype paternalGenotype = new Genotype() { LocusId = individualGenotype.LocusId };
            IGenotype materalGenotype = new Genotype() { LocusId = individualGenotype.LocusId };
            
            if(individual.PaternalId.HasValue)
            {
                paternalGenotype = await _genotypeRepository.GetAsync(individual.PaternalId.Value, individualGenotype.LocusId, token).ConfigureAwait(false);
            } // end if
            
            if(individual.MaternalId.HasValue)
            {
                materalGenotype = await _genotypeRepository.GetAsync(individual.MaternalId.Value, individualGenotype.LocusId, token).ConfigureAwait(false);
            } // end if

            var results = await _punnetSquare.GetOffsprinGenotypesAsync(paternalGenotype, materalGenotype, token).ConfigureAwait(false);
            if(individualGenotype.DominantAllele != null)
            {
                results = results.Where(x => x.Genotype.DominantAllele.Ordinal == individualGenotype.DominantAllele.Ordinal);
            } // end if

            return results;
        } // end method
    
        protected virtual async Task<IEnumerable<IGenotype>> GetApplicableGenotypesFromOffspringAsync(
            TIndividual individual,
            Guid locusId,
            IEnumerable<IGenotype> potentialGenotypes,
            CancellationToken token = default)
        {
            var result = potentialGenotypes.ToList();

            // get all alleles belonging to the locus
            var alleles = await _alleleRepository.GetByLocusAsync(locusId, token).ConfigureAwait(false);

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
                            siblings.Key.PaternalId.Value, 
                            siblings.Key.MaternalId.Value, 
                            locusId,
                            token).ConfigureAwait(false);
                            
                    var expressedGenotypes = siblingGenotypes
                        .Where(x => x.DominantAllele != null)
                        .GroupBy(x => x.ToString())
                        .Select(x => x.First())
                        .OrderBy(x => x.DominantAllele.Ordinal)
                        .ToList();
                    
                    var otherParentId = siblings.Any(x => individual.Id.Equals(x.PaternalId)) ? siblings.First().MaternalId : siblings.First().PaternalId;
                    IEnumerable<IGenotype> otherParentGenotypes = null;
                    if(otherParentId.HasValue)
                    {
                        var otherParentGenotype = await _genotypeRepository.GetAsync(otherParentId.Value, locusId, token).ConfigureAwait(false);                        
                        otherParentGenotypes = otherParentGenotype.BuildPotentialGenotypes(alleles);
                    }
                    else
                    {
                        otherParentGenotypes = new Genotype().BuildPotentialGenotypes(alleles);
                    } // end if

                    if(otherParentGenotypes.Count() > 1)
                    {
                        // restrict the other parent genotypes to ones that could actually help produce the expressed genotypes
                        otherParentGenotypes = await RefinePotentialGenotypesAsync(expressedGenotypes, otherParentGenotypes, result, token).ConfigureAwait(false);
                    } // end if

                    var refinedPotentialGenotypes = await RefinePotentialGenotypesAsync(expressedGenotypes, result, otherParentGenotypes, token).ConfigureAwait(false);

                    result = refinedPotentialGenotypes.ToList();
                } // end foreach
            } // end if
            
            return result;
        } // end method

        protected virtual async Task<IEnumerable<IGenotype>> RefinePotentialGenotypesAsync(
            IEnumerable<IGenotype> expressedGenotypes,
            IEnumerable<IGenotype> parentGenotypes,
            IEnumerable<IGenotype> otherParentGenotypes,
            CancellationToken token = default)
        {
            var result = parentGenotypes.ToList();

            // build a dictionary of each potential genotype and the offspring genotypes it could generate with this other parent
            var potentialGenotypeOffspring = result.ToDictionary(
                // the potential genotypes
                potentialGenotype => potentialGenotype, 
                // the genotypes than can come from this potential genotype
                async potentialGenotype => await BuildOffspringGenotypes(potentialGenotype, otherParentGenotypes, token).ConfigureAwait(false));

            // now remove any potential genotypes that did not produce genotypes that explain all the expressions
            foreach(var potentialGenotypeOffspringKvp in potentialGenotypeOffspring)
            {
                var allExpressedGenotypesRepresented = expressedGenotypes.All(expressedGenotype => 
                    potentialGenotypeOffspringKvp.Value.Result.Any(genotype => 
                        genotype.DominantAllele.Ordinal == expressedGenotype.DominantAllele.Ordinal
                        && (expressedGenotype.OtherAllele == null || genotype.OtherAllele.Ordinal == expressedGenotype.OtherAllele.Ordinal)));

                if(!allExpressedGenotypesRepresented)
                {
                    result.Remove(potentialGenotypeOffspringKvp.Key);
                } // end if
            } // end foreach

            return result;
        } // end method

        protected async Task<IEnumerable<IGenotype>> BuildOffspringGenotypes(
            IGenotype firstParentGenotype, 
            IEnumerable<IGenotype> otherParentGenotypes, 
            CancellationToken token = default)
        {
            var result = new ConcurrentBag<IGenotype>();
            await Parallel.ForEachAsync(otherParentGenotypes, token, async (otherParentGenotype, token) => {
                var offspringGenotypes = await _punnetSquare.GetOffsprinGenotypesAsync(firstParentGenotype, otherParentGenotype, token).ConfigureAwait(false);
                offspringGenotypes.ToList().ForEach(x => result.Add(x.Genotype));
            }).ConfigureAwait(false);

            return result;
        } // end method
    } // end class

    public class GenotypeCalculator<TIndividual, TIndividualRepository, TGenotypeRepository> :
            GenotypeCalculator<Guid, TIndividual, TIndividualRepository, TGenotypeRepository>,
            IGenotypeCalculator<Guid>
        where TIndividual : IIndividual<Guid>
        where TIndividualRepository : IIndividualRepository<Guid, TIndividual>
        where TGenotypeRepository : IGenotypeRepository<Guid>
    {
        public GenotypeCalculator(
                TIndividualRepository individualRepository, 
                TGenotypeRepository genotypeRepository, 
                IAlleleRepository alleleRepository,
                IPunnetSquare punnetSquare, 
                ILogger<GenotypeCalculator<TIndividual, TIndividualRepository, TGenotypeRepository>> logger) 
            : base(individualRepository, genotypeRepository, alleleRepository, punnetSquare, logger)
        {
        } // end method
    } // end method

    public class GenotypeCalculator :
            GenotypeCalculator<IIndividual, IIndividualRepository, IGenotypeRepository>,
            IGenotypeCalculator
    {
        public GenotypeCalculator(
                IIndividualRepository individualRepository, 
                IGenotypeRepository genotypeRepository, 
                IAlleleRepository alleleRepository,
                IPunnetSquare punnetSquare, 
                ILogger<GenotypeCalculator> logger) 
            : base(individualRepository, genotypeRepository, alleleRepository, punnetSquare, logger)
        {
        } // end method
    } // end class
} // end namespace