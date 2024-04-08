using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Extensions;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Heredity.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class ExtensionGenotypeCalculatorTests
    {
        private Mock<IGenotypeRepository<ExtensionAllele, ExtensionLocus, Guid>> _genotypeRepo;
        private Mock<ILogger<GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>>> _logger;
        private Mock<ILogger<PunnetSquare<ExtensionAllele, ExtensionLocus>>> _punnetLogger;
        private GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid> _calculator;

        public ExtensionGenotypeCalculatorTests()
        {
            _genotypeRepo = new Mock<IGenotypeRepository<ExtensionAllele, ExtensionLocus, Guid>>();
            _logger = new Mock<ILogger<GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>>>();
            _punnetLogger = new Mock<ILogger<PunnetSquare<ExtensionAllele, ExtensionLocus>>>();
            _calculator = new GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>(
                _genotypeRepo.Object, 
                new PunnetSquare<ExtensionAllele, ExtensionLocus>(_punnetLogger.Object),
                _logger.Object
            );
        } // end method

        protected void SetupParent(IndividualGenotype<ExtensionAllele, ExtensionLocus, Guid> individualGenotype)
        {
            _genotypeRepo.Setup(x => x.GetAsync(individualGenotype.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individualGenotype);
        } // end method

        protected void SetupOffspring(
            Guid individualId, 
            IEnumerable<IndividualGenotype<ExtensionAllele, ExtensionLocus, Guid>> offspringGenotypes)
        {
            _genotypeRepo.Setup(x => x.GetOffspringAsync(individualId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspringGenotypes);
        } // end method

        [Fact]
        public async Task Calculate_On_Extensions_E_Ej()
        {
            var father = ExtensionAllele.Harlequin
                .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                .BuildIndividual(() => Guid.NewGuid());
            var mother1 = ExtensionAllele.Normal
                .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                .BuildIndividual(() => Guid.NewGuid());
            var mother2 = ExtensionAllele.Harlequin
                .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                .BuildIndividual(() => Guid.NewGuid());
            var mother3 = ExtensionAllele.Normal
                .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                .BuildIndividual(() => Guid.NewGuid());

            var offspring = new List<IndividualGenotype<ExtensionAllele, ExtensionLocus, Guid>>()
            {
                ExtensionAllele.Harlequin
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother2.Id),
                ExtensionAllele.Normal
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother1.Id),
                ExtensionAllele.Harlequin
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother1.Id),
                ExtensionAllele.NonExtension
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother1.Id),
                ExtensionAllele.Normal
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother3.Id),
                ExtensionAllele.Harlequin
                    .BuildGenotype<ExtensionAllele, ExtensionLocus>()
                    .BuildIndividual(() => Guid.NewGuid(), father.Id, mother3.Id)
            };
            
            SetupParent(father);
            SetupParent(mother1);
            SetupParent(mother2);
            SetupParent(mother3);
            SetupOffspring(father.Id, offspring);

            var genotypes = await _calculator.CalculateGenotypesAsync(father.Id).ConfigureAwait(false);

            Assert.NotNull(genotypes);
            Assert.Equal(1, genotypes.Count());
            Assert.Equal("e(j)/e", genotypes.Last().ToString());
        } // end method
    } // end class
} // end namespace