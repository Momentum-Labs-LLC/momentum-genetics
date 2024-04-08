using Momentum.Genetics.Models;
using Momentum.Genetics.TestData;
using Xunit;

namespace Momentum.Genetics.Tests.Models
{
    public class GenotypeTests
    {
        [Fact]
        public void Empty_Constructor()
        {
            var genotype = new Genotype<TestAllele, TestLocus>();
            Assert.Null(genotype.DominantAllele);
            Assert.Null(genotype.OtherAllele);
            Assert.Null(genotype.IsHomozygous);
        } // end method

        [Fact]
        public void Constructor_Only_DominantProvided()
        {
            var genotype = new Genotype<TestAllele, TestLocus>(new SecondAllele());
            Assert.NotNull(genotype.DominantAllele);
            Assert.Null(genotype.OtherAllele);
            Assert.Null(genotype.IsHomozygous);
        } // end method

        [Fact]
        public void Constructor_Homozygous()
        {
            var genotype = new Genotype<TestAllele, TestLocus>(new SecondAllele(), new SecondAllele());
            Assert.NotNull(genotype.DominantAllele);
            Assert.NotNull(genotype.OtherAllele);
            Assert.True(genotype.IsHomozygous);
        } // end method

        [Fact]
        public void Constructor_Heterozygous()
        {
            var genotype = new Genotype<TestAllele, TestLocus>(new SecondAllele(), new ThirdAllele());
            Assert.NotNull(genotype.DominantAllele);
            Assert.NotNull(genotype.OtherAllele);
            Assert.False(genotype.IsHomozygous);
        } // end method

        [Fact]
        public void Constructor_OutOfOrder()
        {
            var secondAllele = new SecondAllele();
            var firstAllele = new FirstAllele();
            var genotype = new Genotype<TestAllele, TestLocus>(secondAllele, firstAllele);
            Assert.Equal(firstAllele, genotype.DominantAllele);
            Assert.Equal(secondAllele, genotype.OtherAllele);
        } // end method

        [Fact]
        public void Null_Null_ToString()
        {
            var expected = "_/_";
            var genotype = new Genotype<TestAllele, TestLocus>();
            var genotypeString = genotype.ToString();
            Assert.Equal(expected, genotypeString);
        } // end method

        [Fact]
        public void First_Null_ToString()
        {
            var expected = "A/_";
            var genotype = new Genotype<TestAllele, TestLocus>()
            {
                DominantAllele = new FirstAllele()
            };
            var genotypeString = genotype.ToString();
            Assert.Equal(expected, genotypeString);
        } // end method

        [Fact]
        public void First_Second_ToString()
        {
            var expected = "A/a(t)";
            var genotype = new Genotype<TestAllele, TestLocus>(new FirstAllele(), new SecondAllele());
            var genotypeString = genotype.ToString();
            Assert.Equal(expected, genotypeString);
        } // end method
    } // end class
} // end namespace