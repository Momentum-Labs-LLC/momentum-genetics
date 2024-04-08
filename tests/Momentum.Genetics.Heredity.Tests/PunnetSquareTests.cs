using Microsoft.Extensions.Logging;
using Momentum.Genetics.Models;

namespace Momentum.Genetics.Heredity.Tests
{
    public class PunnetSquareTests
    {

        private Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>> _logger;

        private PunnetSquare<TestAllele, TestLocus> _punnetSquare;

        public PunnetSquareTests()
        {
            _logger = new Mock<ILogger<PunnetSquare<TestAllele, TestLocus>>>();
            _punnetSquare = new PunnetSquare<TestAllele, TestLocus>(_logger.Object);
        } // end method

        [Fact]
        public void Execute_AA_AA()
        {
            var paternalPair = new Genotype<TestAllele, TestLocus>(TestAllele.Agouti, TestAllele.Agouti);
            var maternalPair = new Genotype<TestAllele, TestLocus>(TestAllele.Agouti, TestAllele.Agouti);
            var results = _punnetSquare.GetOffsprinGenotypes(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(1, results.Count());
            var firstResult = results.First();
            Assert.Equal(1, firstResult.Ratio);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.OtherAllele);
        } // end method

        [Fact]
        public void Execute_A_A()
        {
            var paternalPair = new Genotype<TestAllele, TestLocus>(TestAllele.Agouti, null);
            var maternalPair = new Genotype<TestAllele, TestLocus>(TestAllele.Agouti, null);
            var results = _punnetSquare.GetOffsprinGenotypes(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(6, results.Count());
            var firstResult = results.First();
            Assert.Equal((float)16/(float)36, firstResult.Ratio);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Agouti, firstResult.Genotype.OtherAllele);

            var secondResult = results.ElementAt(1);
            Assert.Equal((float)8/(float)36, secondResult.Ratio);
            Assert.Equal(TestAllele.Agouti, secondResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Tan.ToString(), secondResult.Genotype.OtherAllele.ToString());

            var thirdResult = results.ElementAt(2);
            Assert.Equal((float)8/(float)36, thirdResult.Ratio);
            Assert.Equal(TestAllele.Agouti, thirdResult.Genotype.DominantAllele);
            Assert.Equal(TestAllele.Self.ToString(), thirdResult.Genotype.OtherAllele.ToString());

            var fourthResult = results.ElementAt(3);
            Assert.Equal((float)1/(float)36, fourthResult.Ratio);
            Assert.Equal(TestAllele.Tan.ToString(), fourthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Tan.ToString(), fourthResult.Genotype.OtherAllele.ToString());

            var fifthResult = results.ElementAt(4);
            Assert.Equal((float)2/(float)36, fifthResult.Ratio);
            Assert.Equal(TestAllele.Tan.ToString(), fifthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Self.ToString(), fifthResult.Genotype.OtherAllele.ToString());

            var sixthResult = results.Last();
            Assert.Equal((float)1/(float)36, sixthResult.Ratio);
            Assert.Equal(TestAllele.Self.ToString(), sixthResult.Genotype.DominantAllele.ToString());
            Assert.Equal(TestAllele.Self.ToString(), sixthResult.Genotype.OtherAllele.ToString());
        } // end method

        [Fact]
        public void Execute_Unknown()
        {
            var paternalPair = new Genotype<TestAllele, TestLocus>();
            var maternalPair = new Genotype<TestAllele, TestLocus>();
            var results = _punnetSquare.GetOffsprinGenotypes(paternalPair, maternalPair);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(6, results.Count());
        } // end method
    } // end class
} // end namespace