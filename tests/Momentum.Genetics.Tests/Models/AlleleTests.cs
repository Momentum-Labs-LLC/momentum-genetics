using Momentum.Genetics.TestData;
using Xunit;

namespace Momentum.Genetics.Tests.Models
{
    public class AlleleTests
    {
        [Fact]
        public void FirstAllele_ToString()
        {
            var allele = new FirstAllele();
            var alleleToString = allele.ToString();

            Assert.NotNull(alleleToString);
            Assert.Equal("A", alleleToString);
        } // end if

        [Fact]
        public void SecondAllele_ToString()
        {
            var allele = new SecondAllele();
            var alleleToString = allele.ToString();

            Assert.NotNull(alleleToString);
            Assert.Equal("a(t)", alleleToString);
        } // end if

        [Fact]
        public void ThirdAllele_ToString()
        {
            var allele = new ThirdAllele();
            var alleleToString = allele.ToString();

            Assert.NotNull(alleleToString);
            Assert.Equal("a", alleleToString);
        } // end if
    } // ned class
} // end namespace