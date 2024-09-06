using Microsoft.Extensions.Logging;
using Momentum.Genetics.Extensions;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Heredity.Models;
using Momentum.Genetics.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class ExtensionGenotypeCalculatorTests
    {
        private Guid _locusId = Guid.NewGuid();
        private Mock<IIndividualRepository> _individualRepo;
        private Mock<IGenotypeRepository> _genotypeRepo;
        private Mock<IAlleleRepository> _alleleRepo;
        private Mock<ILogger<GenotypeCalculator>> _logger;
        private Mock<ILogger<PunnetSquare>> _punnetLogger;
        private GenotypeCalculator _calculator;

        public ExtensionGenotypeCalculatorTests()
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

        protected void SetupParent(
            Individual individual,
            IGenotype genotype,
            IEnumerable<Individual> offspring)
        {
            _individualRepo.Setup(x => x.GetAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(individual);
            _individualRepo.Setup(x => x.GetOffspringAsync(individual.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspring);
            _genotypeRepo.Setup(x => x.GetAsync(individual.Id, _locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(genotype);
        } // end method


        protected void SetupOffspringGenotypes(
            Guid paternalId, 
            Guid maternalId,
            IEnumerable<IGenotype> offspringGenotypes)
        {
            _genotypeRepo.Setup(x => x.GetOffspringGenotypesAsync(paternalId, maternalId, _locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(offspringGenotypes);
        } // end method

        protected void SetupAlleles() 
        {
            _alleleRepo.Setup(x => x.GetByLocusAsync(_locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExtensionAllele>() 
                {
                    ExtensionAllele.Steel,
                    ExtensionAllele.Normal,
                    ExtensionAllele.Harlequin,
                    ExtensionAllele.NonExtension
                });
        }

        [Fact]
        public async Task Calculate_On_Extensions_E_Ej()
        {
            var father = new Individual() { Id = Guid.NewGuid() };
            var fatherGenotype = ExtensionAllele.Harlequin.BuildGenotype(_locusId);
            var mother1 = new Individual() { Id = Guid.NewGuid() };
            var mother1Geno = ExtensionAllele.Normal.BuildGenotype(_locusId);
            var mother2 = new Individual() { Id = Guid.NewGuid() };
            var mother2Geno = ExtensionAllele.Harlequin.BuildGenotype(_locusId);
            var mother3 = new Individual() { Id = Guid.NewGuid() };
            var mother3Geno = ExtensionAllele.Normal.BuildGenotype(_locusId);

            var offspring = new Dictionary<Individual, IGenotype>()
            {
                { new Individual(father, mother2), ExtensionAllele.Harlequin.BuildGenotype(_locusId) },
                { new Individual(father, mother1), ExtensionAllele.Normal.BuildGenotype(_locusId) },
                { new Individual(father, mother1), ExtensionAllele.Harlequin.BuildGenotype(_locusId) },
                { new Individual(father, mother1), ExtensionAllele.NonExtension.BuildGenotype(_locusId) },
                { new Individual(father, mother3), ExtensionAllele.Normal.BuildGenotype(_locusId) },
                { new Individual(father, mother3), ExtensionAllele.Harlequin.BuildGenotype(_locusId) },
            };
            
            SetupAlleles();
            SetupParent(father, fatherGenotype, offspring.Where(x => x.Key.PaternalId.Equals(father.Id)).Select(x => x.Key));
            SetupParent(mother1, mother1Geno, offspring.Where(x => x.Key.MaternalId == mother1.Id).Select(x => x.Key));
            SetupParent(mother2, mother2Geno, offspring.Where(x => x.Key.MaternalId == mother2.Id).Select(x => x.Key));
            SetupParent(mother3, mother3Geno, offspring.Where(x => x.Key.MaternalId == mother3.Id).Select(x => x.Key));
            SetupOffspringGenotypes(father.Id, mother1.Id, offspring.Where(x => x.Key.MaternalId == mother1.Id).Select(x => x.Value));
            SetupOffspringGenotypes(father.Id, mother2.Id, offspring.Where(x => x.Key.MaternalId == mother2.Id).Select(x => x.Value));
            SetupOffspringGenotypes(father.Id, mother3.Id, offspring.Where(x => x.Key.MaternalId == mother3.Id).Select(x => x.Value));

            var genotypes = await _calculator.CalculateGenotypesAsync(father.Id, _locusId, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            Assert.NotNull(genotypes);
            Assert.Equal(1, genotypes.Count());
            Assert.Equal("e(j)/e", genotypes.Last().ToString());
        } // end method
    } // end class
} // end namespace