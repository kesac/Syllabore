
## What is the DefaultSyllableGenerator?
 * For convenience, an instance of `NameGenerator` can be used without any additional setup or customization
 * It will use `DefaultSyllableGenerator` as its default syllable provider
 * The vowel-consonant pool of a `DefaultSyllableGenerator` are as follows:

| Category | Values |
| -------- | ------ | 
| Leading Consonants | ```B D L M P R S T``` |
| Leading Consonant Sequences | ```ST PH BR``` |
| Vowels | ```A E I O``` |
| Vowel Sequences | None |
| Trailing Consonants | None |
| Trailing Consonant Sequences | None |
| Ending Consonants | ```D L M N R S T X``` |
| Ending Consonant Sequences | ```ST RN LN``` |

### Probabilities
| Probability that... | Value |
| ------------------- | ----- |
| Leading Consonant Exists | 95% |
| Leading Consonant Becomes Sequences | 25% |
| Vowel Exists | 100% |
| Vowel Becomes Sequence | 25% |
| Ending Consonant Exists | 50% |
| Ending Consonant Becomes Sequence | 25% |