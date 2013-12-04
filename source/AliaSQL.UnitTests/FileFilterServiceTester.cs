using NUnit.Framework;
using AliaSQL.Core.Services.Impl;

namespace AliaSQL.UnitTests.Core.DatabaseManager.Services
{
    [TestFixture]
    public class FileFilterServiceTester
    {
        [Test]
        public void Should_not_filter_anything_when_no_filter_is_specified()
        {
            var filterService = new FileFilterService();
            var filteredFilenames = filterService.GetFilteredFilenames(new[] { "d:/abcd", "d:/xyz" }, "");

            filteredFilenames.Length.ShouldEqual(2);
            CollectionAssert.Contains(filteredFilenames, "d:/abcd");
            CollectionAssert.Contains(filteredFilenames, "d:/xyz");
        }

        [Test]
        public void Should_appy_filter_in_case_sensitive_manner()
        {
            var filterService = new FileFilterService();
            var filteredFilenames = filterService.GetFilteredFilenames(new[] { "d:/abc", "d:/xyz_skip","d:/xyz" }, "skip");

            filteredFilenames.Length.ShouldEqual(2);
            CollectionAssert.Contains(filteredFilenames, "d:/abc");
            CollectionAssert.Contains(filteredFilenames, "d:/xyz");
        }

        [Test]
        public void Should_appy_filter_only_on_file_name_part_in_case_sensitive_manner()
        {
            var filterService = new FileFilterService();
            var filteredFilenames = filterService.GetFilteredFilenames(
                new[]
                    {
                        @"c:\sdf/asdf\abc", @"d:\ioj/sadf\xyz_skip", @"\abc/skip/xyz"
                    }
                , "skip");

            filteredFilenames.Length.ShouldEqual(2);
            CollectionAssert.Contains(filteredFilenames, @"c:\sdf/asdf\abc");
            CollectionAssert.Contains(filteredFilenames, @"\abc/skip/xyz");
        }
    }
}