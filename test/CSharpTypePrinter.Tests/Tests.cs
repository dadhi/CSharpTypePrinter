using System;
using NUnit.Framework;

namespace CSharpTypePrinter.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test_triple_nested_non_generic()
        {
            var s = typeof(A<int>.B<string>.Z).ToCSharpCode(true);
            Assert.AreEqual("Tests.A<int>.B<string>.Z", s);

            s = typeof(A<int>.B<string>.Z).ToCSharpCode();
            Assert.AreEqual("CSharpTypePrinter.Tests.Tests.A<int>.B<string>.Z", s);

            s = typeof(A<int>.B<string>.Z[]).ToCSharpCode(true);
            Assert.AreEqual("Tests.A<int>.B<string>.Z[]", s);

            s = typeof(A<int>.B<string>.Z[]).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""));
            Assert.AreEqual("A<int>.B<string>.Z[]", s);
        }

        [Test]
        public void Test_triple_nested_open_generic()
        {
            var s = typeof(A<>).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""));
            Assert.AreEqual("A<>", s);

            s = typeof(A<>).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""), true);
            Assert.AreEqual("A<X>", s);

            s = typeof(A<>.B<>).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""));
            Assert.AreEqual("A<>.B<>", s);

            s = typeof(A<>.B<>.Z).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""));
            Assert.AreEqual("A<>.B<>.Z", s);

            s = typeof(A<>.B<>.Z).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""), true);
            Assert.AreEqual("A<X>.B<Y>.Z", s);
        }

        [Test]
        public void Test_non_generic_classes()
        {
            var s = typeof(A.B.C).ToCSharpCode(true, (_, x) => x.Replace("Tests.", ""));
            Assert.AreEqual("A.B.C", s);
        }

        class A
        {
            public class B
            {
                public class C { }
            }
        }

        class A<X>
        {
            public class B<Y>
            {
                public class Z { }
            }
        }
    }
}
