# Background
Recently, I had cause to suspect that the System.Random Pseudo-Random Number Generator was not being sufficiently random.  In a sense, the answer to that is "of course not" - it's a *Pseudo*-Random Number Generator.   But what about the quality of it as a PRNG?  Does it have any kind of predictable behaviour that could be causing what I was seeing?

In searching online, I found claims that the System.Random implemention in .NET Framework 4.8 was flawed and could not pass a coin flip test.  I had already implemented a simple uniformity test with a chi-square fitness test.  This passed for all the different ranges I tried.  I went back to these data and checked even vs. odd - no problem.  I tried the test with a range of 2 - still no problem.  So clearly, this problem was fixed prior to .Net 8.  So it's a better PRNG, but how good?

There are suites for testing like [Die Hard](https://en.wikipedia.org/wiki/Diehard_tests) and [Die Harder](https://rurban.github.io/dieharder/manual/dieharder.pdf).  These would require either hooking them up to call the .NET System.Random or porting to a .NET language.  Neither option seemed appealing.

At some point, I came across [NIST Special Publication 800-22 r1a](https://csrc.nist.gov/pubs/sp/800/22/r1/upd1/final), and an [article](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/government-special-issue/test-run-implementing-the-national-institute-of-standards-and-technology-tests-of-randomness-using-csharp) about a .NET implementation of the first three tests.  This article encouraged me to build this testing application, and implement all 15 tests (in addition to the simple uniformity test I'd started with).

# References
There's an odd notion of global vs. local references in this project.  Global References are those which have general applicabililty to the project as a whole (eg. the aforementioned NIST Special Publication).  Local References are specific to a particular source file.

Global References are called out by letters, and listed in the file References/Sources.ods.  Local References will be listed somewhere in the file.  They may be called out by a number, or, if singular, just referred to as something like "the reference".

# Results
I've informally run each test a bunch of times, without really tracking results or how many times.  However, my impression is that the System.Random implementation in .NET 8 is a really good PRNG.  I did notice one test (without tracking which one) where the p-Values were all suspiciously very high.

Next, I'm going to go back and run each of the tests numerous times, and evaluate the results per Sections 4.2.1 and 4.2.2 of Reference A (that's the aforementioned NIST Special Publication :-)).
