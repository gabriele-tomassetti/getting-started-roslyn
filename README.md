# Getting started with Roslyn

This is an example on how to use Roslyn. It's fairly easy example, but it touches the three parts of using the *.NET Compiler Platform*: 
- syntax analysis
- semantic analysis
- syntax rewriting

What we do is to check that every *int* variable is initialized, including the one declared like *var* by looking at the symbol of every variable. If it is not initialized, we initialized it to 42. If it's already initialized, but it's initialization value it's not 42, we change it to 42.

**You can read an article on the example on [Getting started Roslyn](https://tomassetti.me/getting-started-with-roslyn-transforming-c-code/)**

This code is licensed under the [MIT License](LICENSE.md). Thanks to [Nik Sultana](https://github.com/niksu) for having pointed out the lack of a license.

**There is also a [Getting Started Rosylin version ported to F# by Nik Sultana](https://github.com/niksu/getting_started_roslyn_fsharp)**
