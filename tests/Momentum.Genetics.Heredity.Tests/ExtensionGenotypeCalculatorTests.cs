using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class ExtensionGenotypeCalculatorTests
    {
        private Mock<IIndividualRepository<Guid>> _individualRepo;
        private Mock<IGenotypeRepository<ExtensionAllele, ExtensionLocus, Guid>> _genotypeRepo;
        private Mock<ILogger<GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>>> _logger;
        private Mock<ILogger<PunnetSquare<ExtensionAllele, ExtensionLocus>>> _punnetLogger;
        private GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid> _calculator;

        public ExtensionGenotypeCalculatorTests()
        {
            _individualRepo = new Mock<IIndividualRepository<Guid>>();
            _genotypeRepo = new Mock<IGenotypeRepository<ExtensionAllele, ExtensionLocus, Guid>>();
            _logger = new Mock<ILogger<GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>>>();
            _punnetLogger = new Mock<ILogger<PunnetSquare<ExtensionAllele, ExtensionLocus>>>();
            _calculator = new GenotypeCalculator<ExtensionAllele, ExtensionLocus, Guid>(
                _individualRepo.Object,
                _genotypeRepo.Object, 
                new PunnetSquare<ExtensionAllele, ExtensionLocus>(_punnetLogger.Object),
                _logger.Object
            );
        } // end method

        protected void SetupParent(
            Individual<Guid> individual,
            Genotype<ExtensionAllele, ExtensionLocus> genotype,
            IEnumerable<Individual<Guid>> offspring)
        {
            _individualRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);
            _individualRepo.Setup(x => x.GetOffspringAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring);
            _genotypeRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(genotype);
        } // end method

        protected void SetupOffspringGenotypes(
            Guid paternalId, 
            Guid maternalId,
            IEnumerable<Genotype<ExtensionAllele, ExtensionLocus>> offspringGenotypes)
        {
            _genotypeRepo.Setup(x => x.GetOffspringGenotypesAsync(paternalId, maternalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspringGenotypes);
        } // end method

        [Fact]
        public async Task Calculate_On_Extensions_E_Ej()
        {
            var father = new Individual() { Id = Guid.NewGuid() };
            var fatherGenotype = ExtensionAllele.Harlequin.BuildGenotype<ExtensionAllele, ExtensionLocus>();
            var mother1 = new Individual() { Id = Guid.NewGuid() };
            var mother1Geno = ExtensionAllele.Normal.BuildGenotype<ExtensionAllele, ExtensionLocus>();
            var mother2 = new Individual() { Id = Guid.NewGuid() };
            var mother2Geno = ExtensionAllele.Harlequin.BuildGenotype<ExtensionAllele, ExtensionLocus>();
            var mother3 = new Individual() { Id = Guid.NewGuid() };
            var mother3Geno = ExtensionAllele.Normal.BuildGenotype<ExtensionAllele, ExtensionLocus>();

            var offspring = new Dictionary<Individual, Genotype<ExtensionAllele, ExtensionLocus>>()
            {
                { new Individual(father, mother2), ExtensionAllele.Harlequin.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
                { new Individual(father, mother1), ExtensionAllele.Normal.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
                { new Individual(father, mother1), ExtensionAllele.Harlequin.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
                { new Individual(father, mother1), ExtensionAllele.NonExtension.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
                { new Individual(father, mother3), ExtensionAllele.Normal.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
                { new Individual(father, mother3), ExtensionAllele.Harlequin.BuildGenotype<ExtensionAllele, ExtensionLocus>() },
            };
            
            SetupParent(father, fatherGenotype, offspring.Where(x => x.Key.PaternalId.Equals(father.Id)).Select(x => x.Key));
            SetupParent(mother1, mother1Geno, offspring.Where(x => x.Key.MaternalId == mother1.Id).Select(x => x.Key));
            SetupParent(mother2, mother2Geno, offspring.Where(x => x.Key.MaternalId == mother2.Id).Select(x => x.Key));
            SetupParent(mother3, mother3Geno, offspring.Where(x => x.Key.MaternalId == mother3.Id).Select(x => x.Key));
            SetupOffspringGenotypes(father.Id, mother1.Id, offspring.Where(x => x.Key.MaternalId == mother1.Id).Select(x => x.Value));
            SetupOffspringGenotypes(father.Id, mother2.Id, offspring.Where(x => x.Key.MaternalId == mother2.Id).Select(x => x.Value));
            SetupOffspringGenotypes(father.Id, mother3.Id, offspring.Where(x => x.Key.MaternalId == mother3.Id).Select(x => x.Value));

            var genotypes = await _calculator.CalculateGenotypesAsync(father.Id).ConfigureAwait(false);

            Assert.NotNull(genotypes);
            Assert.Equal(1, genotypes.Count());
            Assert.Equal("e(j)/e", genotypes.Last().ToString());
        } // end method
    } // end class
} // end namespace