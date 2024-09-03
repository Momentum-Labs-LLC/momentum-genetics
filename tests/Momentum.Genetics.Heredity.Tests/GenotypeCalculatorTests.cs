using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Extensions;

namespace Momentum.Genetics.Heredity.Tests
{
    public class GenotypeCalculatorTests
    {
        private Mock<IIndividualRepository<Guid>> _individualRepo;
        private Mock<IGenotypeRepository<TestAllele, TestLocus, Guid>> _genotypeRepo;
        private Mock<ILogger<GenotypeCalculator<TestAllele, TestLocus, Guid, IIndividualRepository<Guid>>>> _logger;
        private Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>> _punnetLogger;
        private GenotypeCalculator<TestAllele, TestLocus, Guid, IIndividualRepository<Guid>> _calculator;

        public GenotypeCalculatorTests()
        {
            _individualRepo = new Mock<IIndividualRepository<Guid>>();
            _genotypeRepo = new Mock<IGenotypeRepository<TestAllele, TestLocus, Guid>>();
            _logger = new Mock<ILogger<GenotypeCalculator<TestAllele, TestLocus, Guid, IIndividualRepository<Guid>>>>();
            _punnetLogger = new Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>>();
            _calculator = new GenotypeCalculator<TestAllele, TestLocus, Guid, IIndividualRepository<Guid>>(
                _individualRepo.Object,
                _genotypeRepo.Object,
                new PunnetSquare<TestAllele, TestLocus>(_punnetLogger.Object),
                _logger.Object
            );
        } // end method

        [Fact]
        public async Task Where_Did_Tan_Come_From()
        {
            var individual = new Individual();
            var individualGenotype = new Genotype<TestAllele, TestLocus>();

            var otherParent = new Individual();
            var otherParentGeno = TestAllele.Agouti.BuildGenotype<TestAllele, TestLocus>();

            var offspring = new Dictionary<Individual, Genotype<TestAllele, TestLocus>>()
            {
                { new Individual(individual, otherParent), TestAllele.Agouti.BuildGenotype<TestAllele, TestLocus>() },
                { new Individual(individual, otherParent), TestAllele.Tan.BuildGenotype<TestAllele, TestLocus>() },
                { new Individual(individual, otherParent), TestAllele.Self.BuildGenotype<TestAllele, TestLocus>() },
            };

            _individualRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);
            _individualRepo.Setup(x => x.GetOffspringAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring.Select(x => x.Key));

            _genotypeRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individualGenotype);
            _genotypeRepo.Setup(x => x.GetAsync(otherParent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherParentGeno);
            _genotypeRepo.Setup(x => x.GetOffspringGenotypesAsync(individual.Id, otherParent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring.Select(x => x.Value));

            var result = await _calculator.CalculateGenotypesAsync(individual.Id).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());
            Assert.Equal("a(t)/a", result.First().ToString());
        } // end method
    } // end class
} // end namespace