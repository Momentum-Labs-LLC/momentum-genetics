using Microsoft.Extensions.Logging;
using Momentum.Genetics.Heredity.Interfaces;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class PunnetSquareTests
    {
        private Guid _locusId = Guid.NewGuid();
        private Mock<IAlleleRepository> _alleleRepo;
        private Mock<ILogger<PunnetSquare>> _logger;

        private PunnetSquare _punnetSquare;

        public PunnetSquareTests()
        {
            _alleleRepo = new Mock<IAlleleRepository>();
            _logger = new Mock<ILogger<PunnetSquare>>();
            _punnetSquare = new PunnetSquare(_alleleRepo.Object, _logger.Object);

            _alleleRepo.Setup(x => x.GetByLocusAsync(_locusId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TestAllele>() 
                {
                    TestAllele.Agouti,
                    TestAllele.Tan,
                    TestAllele.Self
                });
        } // end method

        [Fact]
        public async Task Execute_AA_AA()
        {            
            var paternalPair = new Genotype(TestAllele.Agouti, TestAllele.Agouti) { LocusId = _locusId };
            var maternalPair = new Genotype(TestAllele.Agouti, TestAllele.Agouti) { LocusId = _locusId };
            var results = await _punnetSquare.GetOffsprinGenotypesAsync(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(1, results.Count());
            var firstResult = results.First();
            Assert.Equal(4, firstResult.Count);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.OtherAllele);
        } // end method

        [Fact]
        public async Task Execute_A_A()
        {
            var paternalPair = new Genotype(TestAllele.Agouti, null) { LocusId = _locusId };
            var maternalPair = new Genotype(TestAllele.Agouti, null) { LocusId = _locusId };
            var results = await _punnetSquare.GetOffsprinGenotypesAsync(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(6, results.Count());
            var firstResult = results.First();
            Assert.Equal(16, firstResult.Count);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.OtherAllele);

            var secondResult = results.ElementAt(1);
            Assert.Equal(8, secondResult.Count);
            Assert.Equal(TestAllele.Agouti, secondResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Tan.ToString(), secondResult.Genotype.OtherAllele.ToString());

            var thirdResult = results.ElementAt(2);
            Assert.Equal(8, thirdResult.Count);
            Assert.Equal(TestAllele.Agouti, thirdResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Self.ToString(), thirdResult.Genotype.OtherAllele.ToString());

            var fourthResult = results.ElementAt(3);
            Assert.Equal(1, fourthResult.Count);
            Assert.Equal(TestAllele.Tan.ToString(), fourthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Tan.ToString(), fourthResult.Genotype.OtherAllele.ToString());

            var fifthResult = results.ElementAt(4);
            Assert.Equal(2, fifthResult.Count);
            Assert.Equal(TestAllele.Tan.ToString(), fifthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Self.ToString(), fifthResult.Genotype.OtherAllele.ToString());

            var sixthResult = results.Last();
            Assert.Equal(1, sixthResult.Count);
            Assert.Equal(TestAllele.Self.ToString(), sixthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Self.ToString(), sixthResult.Genotype.OtherAllele.ToString());
        } // end method

        [Fact]
        public async Task Execute_Unknown()
        {
            var paternalPair = new Genotype() { LocusId = _locusId };
            var maternalPair = new Genotype() { LocusId = _locusId };
            var results = await _punnetSquare.GetOffsprinGenotypesAsync(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(6, results.Count());
        } // end method
    } // end class
} // end namespace