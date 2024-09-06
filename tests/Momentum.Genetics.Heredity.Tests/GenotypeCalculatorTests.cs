using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Interfaces;

namespace Momentum.Genetics.Heredity.Tests
{
    public class GenotypeCalculatorTests
    {
        private Guid _locusId = Guid.NewGuid();
        private Mock<IIndividualRepository> _individualRepo;
        private Mock<IGenotypeRepository> _genotypeRepo;
        private Mock<IAlleleRepository> _alleleRepo;
        private Mock<ILogger<GenotypeCalculator>> _logger;
        private Mock<ILogger<PunnetSquare>> _punnetLogger;
        private GenotypeCalculator _calculator;

        public GenotypeCalculatorTests()
        {
            _individualRepo = new Mock<IIndividualRepository>();
            _genotypeRepo = new Mock<IGenotypeRepository>();
            _alleleRepo = new Mock<IAlleleRepository>();
            _logger = new Mock<ILogger<GenotypeCalculator>>();
            _punnetLogger = new Mock<ILogger<PunnetSquare>>();
            _calculator = new GenotypeCalculator(
                _individualRepo.Object,
                _genotypeRepo.Object,
                _alleleRepo.Object,
                new PunnetSquare(_alleleRepo.Object, _punnetLogger.Object),
                _logger.Object
            );
        } // end method

        [Fact]
        public async Task Where_Did_Tan_Come_From()
        {
            var individual = new Individual();
            var individualGenotype = new Genotype() { LocusId = _locusId };

            var otherParent = new Individual();
            var otherParentGeno = TestAllele.Agouti.BuildGenotype(_locusId);

            var offspring = new Dictionary<Individual, IGenotype>()
            {
                { new Individual(individual, otherParent), TestAllele.Agouti.BuildGenotype(_locusId) },
                { new Individual(individual, otherParent), TestAllele.Tan.BuildGenotype(_locusId) },
                { new Individual(individual, otherParent), TestAllele.Self.BuildGenotype(_locusId) },
            };

            _alleleRepo.Setup(x => x.GetByLocusAsync(_locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Allele>() 
                { 
                    TestAllele.Agouti,
                    TestAllele.Tan,
                    TestAllele.Self
                });

            _individualRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);
            _individualRepo.Setup(x => x.GetOffspringAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring.Select(x => x.Key));

            _genotypeRepo.Setup(x => x.GetAsync(individual.Id, _locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individualGenotype);
            _genotypeRepo.Setup(x => x.GetAsync(otherParent.Id, _locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherParentGeno);
            _genotypeRepo.Setup(x => x.GetOffspringGenotypesAsync(individual.Id, otherParent.Id, _locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring.Select(x => x.Value));

            var result = await _calculator.CalculateGenotypesAsync(individual.Id, _locusId).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());
            Assert.Equal("a(t)/a", result.First().ToString());
        } // end method
    } // end class
} // end namespace