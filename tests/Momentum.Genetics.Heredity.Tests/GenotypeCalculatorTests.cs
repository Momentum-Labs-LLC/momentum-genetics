using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;
using Microsoft.Extensions.Logging;
using Momentum.Genetics.Heredity.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class GenotypeCalculatorTests
    {
        private Mock<IGenotypeRepository<TestAllele, TestLocus, Guid>> _genotypeRepo;
        private Mock<ILogger<GenotypeCalculator<TestAllele, TestLocus, Guid>>> _logger;
        private Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>> _punnetLogger;
        private GenotypeCalculator<TestAllele, TestLocus, Guid> _calculator;

        public GenotypeCalculatorTests()
        {
            _genotypeRepo = new Mock<IGenotypeRepository<TestAllele, TestLocus, Guid>>();
            _logger = new Mock<ILogger<GenotypeCalculator<TestAllele, TestLocus, Guid>>>();
            _punnetLogger = new Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>>();
            _calculator = new GenotypeCalculator<TestAllele, TestLocus, Guid>(
                _genotypeRepo.Object,
                new PunnetSquare<TestAllele, TestLocus>(_punnetLogger.Object),
                _logger.Object
            );
        } // end method

        [Fact]
        public async Task Where_Did_Tan_Come_From()
        {
            var individual = new IndividualGenotype<TestAllele, TestLocus>()
            {
                Id = Guid.NewGuid(),
                Genotype = new Genotype<TestAllele, TestLocus>()
            };

            var otherParent = new IndividualGenotype<TestAllele, TestLocus>()
            {
                Id = Guid.NewGuid(),
                Genotype = new Genotype<TestAllele, TestLocus>()
                {
                    DominantAllele = TestAllele.Agouti
                }
            };

            var offspring = new List<IndividualGenotype<TestAllele, TestLocus>>()
            {
                new IndividualGenotype<TestAllele, TestLocus>()
                {
                    PaternalId = individual.Id,
                    MaternalId = otherParent.Id,
                    Genotype = new Genotype<TestAllele, TestLocus>()
                    {
                        DominantAllele = TestAllele.Agouti
                    }
                },
                new IndividualGenotype<TestAllele, TestLocus>()
                {
                    PaternalId = individual.Id,
                    MaternalId = otherParent.Id,
                    Genotype = new Genotype<TestAllele, TestLocus>()
                    {
                        DominantAllele = TestAllele.Tan
                    }
                },
                new IndividualGenotype<TestAllele, TestLocus>()
                {
                    PaternalId = individual.Id,
                    MaternalId = otherParent.Id,
                    Genotype = new Genotype<TestAllele, TestLocus>()
                    {
                        DominantAllele = TestAllele.Self
                    }
                }
            };

            _genotypeRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);
            _genotypeRepo.Setup(x => x.GetAsync(otherParent.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherParent);
            _genotypeRepo.Setup(x => x.GetOffspringAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring);

            var result = await _calculator.CalculateGenotypesAsync(individual.Id).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(1, result.Count());
            Assert.Equal("a(t)/a", result.First().ToString());
        } // end method
    } // end class
} // end namespace