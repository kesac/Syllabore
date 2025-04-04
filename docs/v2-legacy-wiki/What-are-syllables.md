## What's a syllable?
 * A syllable consists of a `nucleus` and optional `onset` or `coda`
 * A syllable has the following form: `onset` - `nucleus` - `coda`
 * For example, the word _github_ is composed of two syllables with the vowels 'i' and 'u' as the nuclei 
 * And the word _breach_, which is the same length, only has one syllable

![](https://i.imgur.com/gSzfGSF.png)
![](https://i.imgur.com/hZ8HeYs.png)

## How does Syllabore model syllables?
 * **Syllabore** models syllables as `onset` - `nucleus` - `coda`, but with a different naming scheme 
 * The name scheme is is: `leading consonant` - `vowel` - `trailing consonant`
 * While **Syllabore** uses the terms `vowels` and `consonants` to refer to nucleuses, onsets, and codas, there is nothing that would prevent you from defining non-vowels as nucleuses or non-consonants as onsets and codas
 * By default, a syllable must have a vowel in **Syllabore**, but it is possible to configure syllable providers to generate syllables that have no vowels if that is what is desired

![](https://i.imgur.com/W2J7TPH.png)

 * Onset clusters are onsets with more than one consonant (eg. "tr"). In **Syllabore**, clusters are called `sequences` and onset clusters are called `leading consonant sequences`
 * **Syllabore** also extends this concept to nucleuses and codas, so you will also see `vowel sequences` and `trailing consonant sequences`.

## How does Syllabore generate names?
 * Name generation is accomplished by first generating syllables from a pool of vowels and consonants, then structuring those syllables into names. There are three major components to a Syllabore name generator:
 * **Providers** are used to *provide* randomly generated syllables. Internally they maintain a pool of nucleuses, onsets, codas, clusters, etc. which are used to construct syllables from scratch
 * **Transformers** are an optional mechanism to randomly adjust *or transform* names during the generation process. This is useful in iterating or evolving a name by replacing syllables, swapping a vowel for another vowel, adding new components to the name, etc.
 * **Filters** are an optional mechanism to validate syllable or letter sequences during name generation. A name generator will only output names that pass through its filter. This is useful in avoiding undesirable letter combinations and improve the quality of output

![](https://i.imgur.com/TTXp5Hx.png)

 * At the bare minimum, you need to define vowels (#1) for the name generator to work. Then you'll want to throw in a few consonants (#2) so the output resembles names. Everything else is optional (#3).