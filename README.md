# Background
Recently, I had cause to suspect that the System.Random Pseudo-Random Number Generator was not being sufficiently random.  In a sense, the answer to that is "of course not" - it's a *Pseudo*-Random Number Generator.   But what about the quality of it as a PRNG?  Does it have any kind of predictable behaviour that could be causing what I was seeing?

In searching online, I found claims that the System.Random implemention in .NET Framework 4.8 was flawed and could not pass a [coin flip test](https://fuglede.dk/en/blog/bias-in-net-rng/).  I had already implemented a simple uniformity test with a chi-square fitness test.  This passed for all the different ranges I tried.  I went back to these data and checked even vs. odd - no problem.  I tried the test with a range of 2 - still no problem.  So clearly, this problem was fixed prior to .Net 8.  So it's a better PRNG, but how good?

There are suites for testing like [Die Hard](https://en.wikipedia.org/wiki/Diehard_tests) and [Die Harder](https://rurban.github.io/dieharder/manual/dieharder.pdf).  These would require either hooking them up to call the .NET System.Random or porting to a .NET language.  Neither option seemed appealing.

At some point, I came across [NIST Special Publication 800-22 r1a](https://csrc.nist.gov/pubs/sp/800/22/r1/upd1/final), and an [article](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/government-special-issue/test-run-implementing-the-national-institute-of-standards-and-technology-tests-of-randomness-using-csharp) about a .NET implementation of the first three tests.  This article encouraged me to build this testing application, and implement all 15 tests (in addition to the simple uniformity test I'd started with).

# References
There's an odd notion of global vs. local references in this project.  Global References are those which have general applicabililty to the project as a whole (eg. the aforementioned NIST Special Publication).  Local References are specific to a particular source file.

Global References are called out by letters, and listed in the file References/Sources.ods.  Local References will be listed somewhere in the file.  They may be called out by a number, or, if singular, just referred to as something like "the reference".

# Seeds

Having satisfied myself that the quality of System.Random was unlikely to be causing me problems, I went back to my
previous code work.  I was preparing to fire up about 60 to 64 processes on my cluster when I remembered that the old
implementation used the time of day when no seed was given to the constructor.  Since my cluster's clocks are synchronized,
firing up a bunch of processes at the same time could have potentially caused a problem.  I knew the newer implementation
did something different, but what?

Previously, I'd barely glanced at the System.Random source code.  I treated it as a black box for doing these tests.

Now, I had the question of how does it generate its seed?
So I took a closer look at the source.  I traced it to something called `Interop.RandomBytes`.  How does that work?  It's platform-dependent,
and I traced it to a .c file with an `#ifdef` in it.   So to really answer the question, I'd have to search much more than I was
willing to (all possible paths where that macro may or may not have been conditionally defined)
and repeat that for every different platform.  Could this just be time of day in a deeply disguised format?  Probably not, but I don't know.

On a previous project, when I'd still been under the impression that time of day was used as the seed, I'd created a seed value
from a hash of the time of day, process Id and node names to be sure each process had a unique seed.  Time of day might not be unique
as the clocks are synchronized and all processes are started at (close enough to) the same time.  Process ID might even be correlated
across nodes - the nodes are identical and may have just been booted, giving a risk of two processes on different nodes ending up
with the same id.

Now, in diving into the System.Random source code, I noticed something that surprised me.  If you pass a seed to the constructor,
you get a *completely different* random number generator implementation!  This is accompanied by comments explaining that this is
to maintain backwards compatibility with code written for versions of .NET prior to 6.0 that may have depended upon a fixed seed
producing a specific sequence.  So they've kept the old implementation for when a seed is passed to the constructor.

So there's two paths forward.  First, I can test the seedless System.Random in a clustered configuration to ensure that different
sequences are produced.  Secondly, there's testing the seeded System.Random with the same tests.

# Results

I've moved the detailed results for System.Random without a specified seed to their own [file](./Results/SeedlessSystemRandom.md).

Detailed results for System.Random with a specified seed will be appearing [here](./Results/SeededSystemRandom.md).
